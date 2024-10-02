using ChessAI.Model;

namespace ChessAI.Attacks;

public sealed class Lookup
{
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

    public readonly Bitboard[,] PawnAttacks;
    public readonly Bitboard[] KnightAttacks;
    public readonly Bitboard[] KingAttacks;
    public readonly Bitboard[,] BishopAttacks;
    public readonly Bitboard[,] RookAttacks;
    
    private readonly ulong[] _bishopMagicNumbers;
    private readonly ulong[] _rookMagicNumbers;

    private readonly Bitboard[] _bishopMasks;
    private readonly Bitboard[] _rookMasks;

    public Lookup()
    {
        PawnAttacks = new Bitboard[Board.NumberOfSides, Board.NumberOfSquares];
        KnightAttacks = new Bitboard[Board.NumberOfSquares];
        KingAttacks = new Bitboard[Board.NumberOfSquares];
        BishopAttacks = new Bitboard[Board.NumberOfSquares, 512];
        RookAttacks = new Bitboard[Board.NumberOfSquares, 4096];
        _bishopMasks = new Bitboard[Board.NumberOfSquares];
        _rookMasks = new Bitboard[Board.NumberOfSquares];
        
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
        for (var square = 0; square < Board.NumberOfSquares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Square)square);

            PawnAttacks[(int)Side.White, square] = bb.RankUp().FileDown() | bb.RankUp().FileUp();
            PawnAttacks[(int)Side.Black, square] = bb.RankDown().FileDown() | bb.RankDown().FileUp();
        }
    }

    private void InitializeKnightAttacks()
    {
        for (var square = 0; square < Board.NumberOfSquares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Square)square);

            KnightAttacks[square] =
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
        for (var square = 0; square < Board.NumberOfSquares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Square)square);

            KingAttacks[square] =
                bb.RankDown() | bb.RankUp() | bb.FileDown() | bb.FileUp() |
                bb.RankDown().FileDown() | bb.RankDown().FileUp() |
                bb.RankUp().FileDown() | bb.RankUp().FileUp();
        }
    }

    // See: https://www.chessprogramming.org/Magic_Bitboards
    private void InitializeBishopAttacks()
    {
        for (var square = 0; square < Board.NumberOfSquares; square++)
        {
            _bishopMasks[square] = CalculateBishopRelevantOccupancyBitboard(square);
            var attackMask = _bishopMasks[square];
            var relevantBitsCount = attackMask.CountBits();
            var occupancyIndices = 1 << relevantBitsCount;

            for (var index = 0; index < occupancyIndices; index++)
            {
                var occupancy = attackMask.SetOccupancy(index, relevantBitsCount);
                var magicIndex =
                    Convert.ToInt32(((occupancy * _bishopMagicNumbers[square]) >> (Board.NumberOfSquares - BishopRelevantBits[square]))
                        .Value);
                BishopAttacks[square, magicIndex] = CalculateBishopAttacks(square, occupancy);
            }
        }
    }

    private void InitializeRookAttacks()
    {
        for (var square = 0; square < Board.NumberOfSquares; square++)
        {
            _rookMasks[square] = CalculateRookRelevantOccupancyBitboard(square);
            var attackMask = _rookMasks[square];
            var relevantBitsCount = attackMask.CountBits();
            var occupancyIndices = 1 << relevantBitsCount;

            for (var index = 0; index < occupancyIndices; index++)
            {
                var occupancy = attackMask.SetOccupancy(index, relevantBitsCount);
                var magicIndex =
                    Convert.ToInt32(((occupancy * _rookMagicNumbers[square]) >> (Board.NumberOfSquares - RookRelevantBits[square]))
                        .Value);
                RookAttacks[square, magicIndex] = CalculateRookAttacks(square, occupancy);
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
        var targetRank = square / Board.NumberOfFiles;
        var targetFile = square % Board.NumberOfFiles;

        // For each diagonal, initialize
        int rank = targetRank + 1, file = targetFile + 1;
        while (rank <= 6 && file <= 6)
        {
            bb |= mask << (rank * Board.NumberOfFiles + file);
            rank++;
            file++;
        }

        rank = targetRank - 1;
        file = targetFile + 1;
        while (rank >= 1 && file <= 6)
        {
            bb |= mask << (rank * Board.NumberOfFiles + file);
            rank--;
            file++;
        }

        rank = targetRank + 1;
        file = targetFile - 1;
        while (rank <= 6 && file >= 1)
        {
            bb |= mask << (rank * Board.NumberOfFiles + file);
            rank++;
            file--;
        }

        rank = targetRank - 1;
        file = targetFile - 1;
        while (rank >= 1 && file >= 1)
        {
            bb |= mask << (rank * Board.NumberOfFiles + file);
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
        var targetRank = square / Board.NumberOfFiles;
        var targetFile = square % Board.NumberOfFiles;

        // For each line, initialize
        var rank = targetRank + 1;
        while (rank <= 6)
        {
            bb |= mask << (rank * Board.NumberOfFiles + targetFile);
            rank++;
        }

        rank = targetRank - 1;
        while (rank >= 1)
        {
            bb |= mask << (rank * Board.NumberOfFiles + targetFile);
            rank--;
        }

        var file = targetFile + 1;
        while (file <= 6)
        {
            bb |= mask << (targetRank * Board.NumberOfFiles + file);
            file++;
        }

        file = targetFile - 1;
        while (file >= 1)
        {
            bb |= mask << (targetRank * Board.NumberOfFiles + file);
            file--;
        }

        return bb;
    }

    // Calculate the bishop attacks given an occupancy table
    public static Bitboard CalculateBishopAttacks(int bishopSquare, Bitboard occupancy)
    {
        var bb = Bitboard.EmptyBitboard();
        var mask = new Bitboard(1);
        var targetRank = bishopSquare / Board.NumberOfFiles;
        var targetFile = bishopSquare % Board.NumberOfFiles;

        // For each diagonal, initialize
        int rank = targetRank + 1, file = targetFile + 1;
        while (rank <= 7 && file <= 7)
        {
            var square = rank * Board.NumberOfFiles + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
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
            var square = rank * Board.NumberOfFiles + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
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
            var square = rank * Board.NumberOfFiles + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
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
            var square = rank * Board.NumberOfFiles + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
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
        var targetRank = rookSquare / Board.NumberOfRanks;
        var targetFile = rookSquare % Board.NumberOfRanks;

        // For each line, initialize
        var rank = targetRank + 1;
        while (rank <= 6)
        {
            var square = rank * Board.NumberOfRanks + targetFile;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
            {
                break;
            }

            rank++;
        }

        rank = targetRank - 1;
        while (rank >= 1)
        {
            var square = rank * Board.NumberOfRanks + targetFile;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
            {
                break;
            }

            rank--;
        }

        var file = targetFile + 1;
        while (file <= 6)
        {
            var square = targetRank * Board.NumberOfRanks + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
            {
                break;
            }

            file++;
        }

        file = targetFile - 1;
        while (file >= 1)
        {
            var square = targetRank * Board.NumberOfRanks + file;
            bb |= mask << (square);

            if (occupancy.GetBit((Square)square) == 1)
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
        var occupancyIndex = Convert.ToInt32((occupancyCopy >> (Board.NumberOfSquares - BishopRelevantBits[square])).Value);
        return BishopAttacks[square, occupancyIndex];
    }
    
    public Bitboard GetRookAttacks(int square, Bitboard occupancy)
    {
        var occupancyCopyR = occupancy.Copy();
        var occupancyCopyB = occupancy.Copy();
        
        occupancyCopyR &= _rookMasks[square];
        occupancyCopyB &= _bishopMasks[square];
        
        occupancyCopyR *= _rookMagicNumbers[square];
        occupancyCopyB *= _bishopMagicNumbers[square];
        
        var occupancyIndexB = Convert.ToInt32((occupancyCopyB >> (Board.NumberOfSquares - BishopRelevantBits[square])).Value);
        var occupancyIndexR = Convert.ToInt32((occupancyCopyR >> (Board.NumberOfSquares - RookRelevantBits[square])).Value);
        return BishopAttacks[square, occupancyIndexB] | RookAttacks[square, occupancyIndexR];
    }

    public Bitboard GetQueenAttacks(int square, Bitboard occupancy)
    {
        var occupancyCopy = occupancy.Copy();
        occupancyCopy &= _rookMasks[square];
        occupancyCopy *= _rookMagicNumbers[square];
        var occupancyIndex = Convert.ToInt32((occupancyCopy >> (Board.NumberOfSquares - RookRelevantBits[square])).Value);
        return RookAttacks[square, occupancyIndex];
    }
}
