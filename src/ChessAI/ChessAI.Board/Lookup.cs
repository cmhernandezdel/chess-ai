﻿using ChessAI.Shared;

namespace ChessAI.Board;

public class Lookup
{
    private readonly int sides = EnumExtensions.Count<Board.Side>();
    private readonly int squares = EnumExtensions.Count<Board.Square>();

    public readonly Bitboard[,] pawnAttacks;
    public readonly Bitboard[] knightAttacks;
    public readonly Bitboard[] kingAttacks;

    public Lookup()
    {
        pawnAttacks = new Bitboard[sides, squares];
        knightAttacks = new Bitboard[squares];
        kingAttacks = new Bitboard[squares];
        InitializePawnAttacks();
        InitializeKnightAttacks();
        InitializeKingAttacks();
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
        return CalculateBishopRelevantOccupancyBitboard((int)Board.Square.d4);
    }

    public Bitboard InitializeRookAttacks()
    {
        return CalculateRookRelevantOccupancyBitboard((int)Board.Square.d4);
    }

    // Given the position of the bishop, calculate the relevant occupancy squares
    // These squares exclude anything in ranks 1 and 8 and anything in files a and h
    // because no matter what stands there, they are always under attack if nothing blocks it
    // (i.e., we do not care about their occupancy)
    private Bitboard CalculateBishopRelevantOccupancyBitboard(int square)
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
    private Bitboard CalculateRookRelevantOccupancyBitboard(int square)
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


    // first index is the position of the piece with 0 being a1 and 63 being h8
    // second index is the occupancy of the rank, with the outer squares not mattering
    // occupancy index = (occupancy >> ((square & ~7) + 1)) & 63

    // to fill these, we put the rook in a1 through h1, we don't care about a and h files
    // and we represent whether the square is occupied or not by a bit (0/1), in reverse,
    // so i.e., if the rook is on A1 and C is occupied and the rest blank, the attacks will be
    // on b and c, so it is 0000 (h to e) - 0110 (d to a), so 0x06.
    // public readonly byte[][] FirstRankAttacks = new byte[8][]
    // {
    //     new byte[] {
    //         0xf7, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x1e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x3e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x1e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x7e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x1e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x3e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02,
    //         0x1e, 0x02, 0x06, 0x02, 0x0e, 0x02, 0x06, 0x02
    //     },
    //     new byte[] {
    //         0xfd, 0xfd, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x1d, 0x1d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x3d, 0x3d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x1d, 0x1d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x7d, 0x7d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x1d, 0x1d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x3d, 0x3d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05,
    //         0x1d, 0x1d, 0x05, 0x05, 0x0d, 0x0d, 0x05, 0x05
    //     },
    //     new byte[] {
    //         0xfb, 0xfa, 0xfb, 0xfa, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x1b, 0x1a, 0x1b, 0x1a, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x3b, 0x3a, 0x3b, 0x3a, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x1b, 0x1a, 0x1b, 0x1a, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x7b, 0x7a, 0x7b, 0x7a, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x1b, 0x1a, 0x1b, 0x1a, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x3b, 0x3a, 0x3b, 0x3a, 0x0b, 0x0a, 0x0b, 0x0a,
    //         0x1b, 0x1a, 0x1b, 0x1a, 0x0b, 0x0a, 0x0b, 0x0a
    //     },
    //     new byte[] {
    //         0xf7, 0xf6, 0xf4, 0xf4, 0xf7, 0xf6, 0xf4, 0xf4,
    //         0x17, 0x16, 0x14, 0x14, 0x17, 0x16, 0x14, 0x14,
    //         0x37, 0x36, 0x34, 0x34, 0x37, 0x36, 0x34, 0x34,
    //         0x17, 0x16, 0x14, 0x14, 0x17, 0x16, 0x14, 0x14,
    //         0x77, 0x76, 0x74, 0x74, 0x77, 0x76, 0x74, 0x74,
    //         0x17, 0x16, 0x14, 0x14, 0x17, 0x16, 0x14, 0x14,
    //         0x37, 0x36, 0x34, 0x34, 0x37, 0x36, 0x34, 0x34,
    //         0x17, 0x16, 0x14, 0x14, 0x17, 0x16, 0x14, 0x14
    //     },
    //     new byte[] {
    //         0xef, 0xee, 0xec, 0xec, 0xe8, 0xe8, 0xe8, 0xe8,
    //         0xef, 0xee, 0xec, 0xec, 0xe8, 0xe8, 0xe8, 0xe8,
    //         0x2f, 0x2e, 0x2c, 0x2c, 0x28, 0x28, 0x28, 0x28,
    //         0x2f, 0x2e, 0x2c, 0x2c, 0x28, 0x28, 0x28, 0x28,
    //         0x6f, 0x6e, 0x6c, 0x6c, 0x68, 0x68, 0x68, 0x68,
    //         0x6f, 0x6e, 0x6c, 0x6c, 0x68, 0x68, 0x68, 0x68,
    //         0x2f, 0x2e, 0x2c, 0x2c, 0x28, 0x28, 0x28, 0x28,
    //         0x2f, 0x2e, 0x2c, 0x2c, 0x28, 0x28, 0x28, 0x28
    //     },
    //     new byte[] {
    //         0xdf, 0xde, 0xdc, 0xdc, 0xd8, 0xd8, 0xd8, 0xd8,
    //         0xd0, 0xd0, 0xd0, 0xd0, 0xd0, 0xd0, 0xd0, 0xd0,
    //         0xd0, 0xd0, 0xd0, 0xd0, 0xd0, 0xd0, 0xd0, 0xd0,
    //         0xdf, 0xde, 0xdc, 0xdc, 0xd8, 0xd8, 0xd8, 0xd8,
    //         0x5f, 0x5e, 0x5c, 0x5c, 0x58, 0x58, 0x58, 0x58,
    //         0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50,
    //         0x5f, 0x5e, 0x5c, 0x5c, 0x58, 0x58, 0x58, 0x58,
    //         0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50, 0x50
    //     },
    //     new byte[] {
    //         0xbf, 0xbe, 0xbc, 0xbc, 0xb8, 0xb8, 0xb8, 0xb8,
    //         0xb0, 0xb0, 0xb0, 0xb0, 0xb0, 0xb0, 0xb0, 0xb0,
    //         0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0,
    //         0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0,
    //         0xbf, 0xbe, 0xbc, 0xbc, 0xb8, 0xb8, 0xb8, 0xb8,
    //         0xb0, 0xb0, 0xb0, 0xb0, 0xb0, 0xb0, 0xb0, 0xb0,
    //         0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0,
    //         0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0, 0xa0
    //     },
    //     new byte[] {
    //         0x7f, 0x7e, 0x7c, 0x7c, 0x78, 0x78, 0x78, 0x78,
    //         0x70, 0x70, 0x70, 0x70, 0x70, 0x70, 0x70, 0x70,
    //         0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0,
    //         0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0, 0xc0,
    //         0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
    //         0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
    //         0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40,
    //         0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40
    //     },
    // };
}
