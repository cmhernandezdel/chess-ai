using ChessAI.Shared;

namespace ChessAI.Board;

public class Lookup
{
    private readonly int sides = EnumExtensions.Count<Board.Side>();
    private readonly int squares = EnumExtensions.Count<Board.Square>();

    public readonly Bitboard[,] pawnAttacks;
    public readonly Bitboard[] knightAttacks;
    public readonly Bitboard[] kingAttacks;

    // This means: if a bishop is on a given square, how many squares are relevant
    // (not counting the edges of the board), i.e. how many squares it can move to
    public static readonly int[] bishopRelevantBits =
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
    public static readonly int[] rookRelevantBits =
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

    public readonly ulong[] bishopMagicNumbers;
    public readonly ulong[] rookMagicNumbers;

    public Lookup()
    {
        pawnAttacks = new Bitboard[sides, squares];
        knightAttacks = new Bitboard[squares];
        kingAttacks = new Bitboard[squares];
        InitializePawnAttacks();
        InitializeKnightAttacks();
        InitializeKingAttacks();
        var magicNumbersGenerator = new MagicNumbersGenerator();
        var magicNumbers = magicNumbersGenerator.InitMagicNumbers();
        bishopMagicNumbers = magicNumbers.Item1;
        rookMagicNumbers = magicNumbers.Item2;
    }

    private void InitializePawnAttacks()
    {
        for (int square = 0; square < squares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Board.Square)square);

            pawnAttacks[(int)Board.Side.White, square] = bb.RankUp().FileDown() | bb.RankUp().FileUp();
            pawnAttacks[(int)Board.Side.Black, square] = bb.RankDown().FileDown() | bb.RankDown().FileUp();
        }
    }

    private void InitializeKnightAttacks()
    {
        for (int square = 0; square < squares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Board.Square)square);

            knightAttacks[square] =
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
        for (int square = 0; square < squares; square++)
        {
            var bb = Bitboard.EmptyBitboard();
            bb.SetBit((Board.Square)square);

            kingAttacks[square] =
                bb.RankDown() | bb.RankUp() | bb.FileDown() | bb.FileUp() |
                bb.RankDown().FileDown() | bb.RankDown().FileUp() |
                bb.RankUp().FileDown() | bb.RankUp().FileUp();
        }
    }

    // See: https://www.chessprogramming.org/Magic_Bitboards
    public Bitboard InitializeBishopAttacks()
    {
        var blocks = Bitboard.EmptyBitboard();
        return CalculateBishopAttacks((int)Board.Square.d4, blocks);
    }

    public Bitboard InitializeRookAttacks()
    {
        var blocks = Bitboard.EmptyBitboard();
        return CalculateRookAttacks((int)Board.Square.a1, blocks);
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
        int rank = targetRank + 1;
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

        int file = targetFile + 1;
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
        int rank = targetRank + 1;
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

        int file = targetFile + 1;
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
}
