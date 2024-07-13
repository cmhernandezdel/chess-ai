using ChessAI.Shared;

namespace ChessAI.Board;

public class Lookup
{
    private readonly int _sides = EnumExtensions.Count<Board.Side>();
    private readonly int _squares = EnumExtensions.Count<Board.Square>();

    private readonly Bitboard[,] _pawnAttacks;
    private readonly Bitboard[] _knightAttacks;
    private readonly Bitboard[] _kingAttacks;
    private readonly Bitboard[,] _bishopAttacks;
    private readonly Bitboard[,] _rookAttacks;

    // This means: if a bishop is on a given square, how many squares are relevant
    // (not counting the edges of the board), i.e. how many squares it can move to
    public static readonly int[] BishopRelevantBits =
    [
        6, 5, 5, 5, 5, 5, 5, 6,
        5, 5, 5, 5, 5, 5, 5, 5,
        5, 5, 7, 7, 7, 7, 5, 5,
        5, 5, 7, 9, 9, 7, 5, 5,
        5, 5, 7, 9, 9, 7, 5, 5,
        5, 5, 7, 7, 7, 7, 5, 5,
        5, 5, 5, 5, 5, 5, 5, 5,
        6, 5, 5, 5, 5, 5, 5, 6
    ];

    // The same but with the rook instead of the bishop
    public static readonly int[] RookRelevantBits =
    [
        12, 11, 11, 11, 11, 11, 11, 12,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        11, 10, 10, 10, 10, 10, 10, 11,
        12, 11, 11, 11, 11, 11, 11, 12,
    ];

    private readonly ulong[] _bishopMagicNumbers;
    private readonly ulong[] _rookMagicNumbers;

    private readonly Bitboard[] _bishopMasks;
    private readonly Bitboard[] _rookMasks;

    public Lookup()
    {
        _pawnAttacks = new Bitboard[_sides, _squares];
        _knightAttacks = new Bitboard[_squares];
        _kingAttacks = new Bitboard[_squares];
        _bishopAttacks = new Bitboard[_squares, 512];
        _rookAttacks = new Bitboard[_squares, 4096];
        _bishopMasks = new Bitboard[_squares];
        _rookMasks = new Bitboard[_squares];
        
        var magicNumbersGenerator = new MagicNumbersGenerator();
        var magicNumbers = magicNumbersGenerator.InitMagicNumbers();
        _bishopMagicNumbers = magicNumbers.Item1;
        _rookMagicNumbers = magicNumbers.Item2;
        
        InitializePawnAttacks();
        InitializeKnightAttacks();
        InitializeKingAttacks();
        InitializeBishopAttacks();
        InitializeRookAttacks();
    }

    private void InitializePawnAttacks()
    {
        for (var square = 0; square < _squares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Board.Square)square);

            _pawnAttacks[(int)Board.Side.White, square] = bb.RankUp().FileDown() | bb.RankUp().FileUp();
            _pawnAttacks[(int)Board.Side.Black, square] = bb.RankDown().FileDown() | bb.RankDown().FileUp();
        }
    }

    private void InitializeKnightAttacks()
    {
        for (var square = 0; square < _squares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Board.Square)square);

            _knightAttacks[square] =
                bb.RankUp().RankUp().FileDown() |
                bb.RankUp().RankUp().FileUp() |
                bb.RankDown().RankDown().FileDown() |
                bb.RankDown().RankDown().FileUp() |
                bb.RankUp().FileUp().FileUp() |
                bb.RankUp().FileDown().FileDown() |
                bb.RankDown().FileUp().FileUp() |
                bb.RankDown().FileDown().FileDown();
        }
    }

    private void InitializeKingAttacks()
    {
        for (var square = 0; square < _squares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Board.Square)square);

            _kingAttacks[square] =
                bb.RankDown() | bb.RankUp() | bb.FileDown() | bb.FileUp() |
                bb.RankDown().FileDown() | bb.RankDown().FileUp() |
                bb.RankUp().FileDown() | bb.RankUp().FileUp();
        }
    }

    // See: https://www.chessprogramming.org/Magic_Bitboards
    private void InitializeBishopAttacks()
    {
        for (var square = 0; square < _squares; square++)
        {
            _bishopMasks[square] = CalculateBishopRelevantOccupancyBitboard(square);
            var attackMask = _bishopMasks[square];
            var relevantBitsCount = attackMask.CountBits();
            var occupancyIndices = 1 << relevantBitsCount;

            for (var index = 0; index < occupancyIndices; index++)
            {
                var occupancy = attackMask.SetOccupancy(index, relevantBitsCount);
                var magicIndex =
                    Convert.ToInt32(((occupancy * _bishopMagicNumbers[square]) >> (64 - BishopRelevantBits[square]))
                        .Value);
                _bishopAttacks[square, magicIndex] = CalculateBishopAttacks(square, occupancy);
            }
        }
    }

    private void InitializeRookAttacks()
    {
        for (var square = 0; square < _squares; square++)
        {
            _rookMasks[square] = CalculateRookRelevantOccupancyBitboard(square);
            var attackMask = _rookMasks[square];
            var relevantBitsCount = attackMask.CountBits();
            var occupancyIndices = 1 << relevantBitsCount;

            for (var index = 0; index < occupancyIndices; index++)
            {
                var occupancy = attackMask.SetOccupancy(index, relevantBitsCount);
                var magicIndex =
                    Convert.ToInt32(((occupancy * _rookMagicNumbers[square]) >> (64 - RookRelevantBits[square]))
                        .Value);
                _rookAttacks[square, magicIndex] = CalculateRookAttacks(square, occupancy);
            }
        }
    }

    // Given the position of the bishop, calculate the relevant occupancy squares
    // These squares exclude anything in ranks 1 and 8 and anything in files a and h
    // because no matter what stands there, they are always under attack if nothing blocks it
    // (i.e., we do not care about their occupancy)
    public static Bitboard CalculateBishopRelevantOccupancyBitboard(int square)
    {
        var bb = Bitboard.EmptyBitboard();
        var mask = new Bitboard(1);
        var targetRank = square / 8;
        var targetFile = square % 8;

        // For each diagonal, initialize
        int rank = targetRank + 1, file = targetFile + 1;
        while (rank <= 6 && file <= 6)
        {
            bb |= mask << (rank * 8 + file);
            rank++;
            file++;
        }

        rank = targetRank - 1;
        file = targetFile + 1;
        while (rank >= 1 && file <= 6)
        {
            bb |= mask << (rank * 8 + file);
            rank--;
            file++;
        }

        rank = targetRank + 1;
        file = targetFile - 1;
        while (rank <= 6 && file >= 1)
        {
            bb |= mask << (rank * 8 + file);
            rank++;
            file--;
        }

        rank = targetRank - 1;
        file = targetFile - 1;
        while (rank >= 1 && file >= 1)
        {
            bb |= mask << (rank * 8 + file);
            rank--;
            file--;
        }

        return bb;
    }

    // Given the position of the rook, calculate the relevant occupancy squares
    // These squares exclude anything in ranks 1 and 8 and anything in files a and h
    // because no matter what stands there, they are always under attack if nothing blocks it
    // (i.e., we do not care about their occupancy)
    public static Bitboard CalculateRookRelevantOccupancyBitboard(int square)
    {
        var bb = Bitboard.EmptyBitboard();
        var mask = new Bitboard(1);
        var targetRank = square / 8;
        var targetFile = square % 8;

        // For each line, initialize
        var rank = targetRank + 1;
        while (rank <= 6)
        {
            bb |= mask << (rank * 8 + targetFile);
            rank++;
        }

        rank = targetRank - 1;
        while (rank >= 1)
        {
            bb |= mask << (rank * 8 + targetFile);
            rank--;
        }

        var file = targetFile + 1;
        while (file <= 6)
        {
            bb |= mask << (targetRank * 8 + file);
            file++;
        }

        file = targetFile - 1;
        while (file >= 1)
        {
            bb |= mask << (targetRank * 8 + file);
            file--;
        }

        return bb;
    }

    // Calculate the bishop attacks given an occupancy table
    public static Bitboard CalculateBishopAttacks(int bishopSquare, Bitboard occupancy)
    {
        var bb = Bitboard.EmptyBitboard();
        var mask = new Bitboard(1);
        var targetRank = bishopSquare / 8;
        var targetFile = bishopSquare % 8;

        // For each diagonal, initialize
        int rank = targetRank + 1, file = targetFile + 1;
        while (rank <= 7 && file <= 7)
        {
            var square = rank * 8 + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            rank++;
            file++;
        }

        rank = targetRank - 1;
        file = targetFile + 1;
        while (rank >= 0 && file <= 7)
        {
            var square = rank * 8 + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            rank--;
            file++;
        }

        rank = targetRank + 1;
        file = targetFile - 1;
        while (rank <= 7 && file >= 0)
        {
            var square = rank * 8 + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            rank++;
            file--;
        }

        rank = targetRank - 1;
        file = targetFile - 1;
        while (rank >= 0 && file >= 0)
        {
            var square = rank * 8 + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            rank--;
            file--;
        }

        return bb;
    }

    // Calculate the rook attacks given an occupancy table
    public static Bitboard CalculateRookAttacks(int rookSquare, Bitboard occupancy)
    {
        var bb = Bitboard.EmptyBitboard();
        var mask = new Bitboard(1);
        var targetRank = rookSquare / 8;
        var targetFile = rookSquare % 8;

        // For each line, initialize
        var rank = targetRank + 1;
        while (rank <= 6)
        {
            var square = rank * 8 + targetFile;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            rank++;
        }

        rank = targetRank - 1;
        while (rank >= 1)
        {
            var square = rank * 8 + targetFile;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            rank--;
        }

        var file = targetFile + 1;
        while (file <= 6)
        {
            var square = targetRank * 8 + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            file++;
        }

        file = targetFile - 1;
        while (file >= 1)
        {
            var square = targetRank * 8 + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Board.Square)square) == 1)
            {
                break;
            }

            file--;
        }

        return bb;
    }

    public Bitboard GetBishopAttacks(int square, Bitboard occupancy)
    {
        var occupancyCopy = occupancy.Copy();
        occupancyCopy &= _bishopMasks[square];
        occupancyCopy *= _bishopMagicNumbers[square];
        var occupancyIndex = Convert.ToInt32((occupancyCopy >> (64 - BishopRelevantBits[square])).Value);
        return _bishopAttacks[square, occupancyIndex];
    }
    
    public Bitboard GetRookAttacks(int square, Bitboard occupancy)
    {
        var occupancyCopy = occupancy.Copy();
        occupancyCopy &= _rookMasks[square];
        occupancyCopy *= _rookMagicNumbers[square];
        var occupancyIndex = Convert.ToInt32((occupancyCopy >> (64 - RookRelevantBits[square])).Value);
        return _rookAttacks[square, occupancyIndex];
    }
}
