namespace ChessAI.Board;

/// <summary>
/// Representation of the board.
/// Layout 2: https://gekomad.github.io/Cinnamon/BitboardCalculator/
/// </summary>
public sealed class Board
{
    public enum Square
    {
        a8, b8, c8, d8, e8, f8, g8, h8,
        a7, b7, c7, d7, e7, f7, g7, h7,
        a6, b6, c6, d6, e6, f6, g6, h6,
        a5, b5, c5, d5, e5, f5, g5, h5,
        a4, b4, c4, d4, e4, f4, g4, h4,
        a3, b3, c3, d3, e3, f3, g3, h3,
        a2, b2, c2, d2, e2, f2, g2, h2,
        a1, b1, c1, d1, e1, f1, g1, h1,
    };

    public enum Side
    {
        White = 0,
        Black = 1
    }

    private ulong WhiteKing;
    private ulong WhiteQueens;
    private ulong WhiteBishops;
    private ulong WhiteKnights;
    private ulong WhiteRooks;
    public ulong WhitePawns;

    private ulong BlackKing;
    private ulong BlackQueens;
    private ulong BlackBishops;
    private ulong BlackKnights;
    private ulong BlackRooks;
    private ulong BlackPawns;

    public const ulong RANK_1 = 0x00000000000000ff;
    public const ulong RANK_2 = 0x000000000000ff00;
    public const ulong RANK_3 = 0x0000000000ff0000;
    public const ulong RANK_4 = 0x00000000ff000000;
    public const ulong RANK_5 = 0x000000ff00000000;
    public const ulong RANK_6 = 0x0000ff0000000000;
    public const ulong RANK_7 = 0x00ff000000000000;
    public const ulong RANK_8 = 0xff00000000000000;

    // Ref: https://gekomad.github.io/Cinnamon/BitboardCalculator/
    // Layout 2: little endian
    public Board()
    {
        WhiteKing = 0x0000000000000010;
        WhiteQueens = 0x0000000000000008;
        WhiteBishops = 0x0000000000000024;
        WhiteKnights = 0x0000000000000042;
        WhiteRooks = 0x0000000000000081;
        WhitePawns = 0x000000000000ff00;

        BlackKing = 0x1000000000000000;
        BlackQueens = 0x800000000000000;
        BlackBishops = 0x2400000000000000;
        BlackKnights = 0x4200000000000000;
        BlackRooks = 0x8100000000000000;
        BlackPawns = 0x00ff000000000000;
    }

    public ulong GetOccupiedSquares()
    {
        return GetWhiteOccupiedSquares() | GetBlackOccupiedSquares();
    }

    public ulong GetWhiteOccupiedSquares()
    {
        return WhiteKing | WhiteQueens | WhiteBishops |
            WhiteKnights | WhiteRooks | WhitePawns;
    }

    public ulong GetBlackOccupiedSquares()
    {
        return BlackKing | BlackQueens | BlackBishops |
            BlackKnights | BlackRooks | BlackPawns;
    }

    public ulong GetEmptySquares()
    {
        return ~GetOccupiedSquares();
    }
}