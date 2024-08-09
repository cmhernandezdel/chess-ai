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
        NoSquare
    }
    
    public enum Piece
    {
        WhitePawn, WhiteKnight, WhiteBishop, WhiteRook, WhiteQueen, WhiteKing,
        BlackPawn, BlackKnight, BlackBishop, BlackRook, BlackQueen, BlackKing
    }

    public enum Side
    {
        White,
        Black,
        Both
    }

    [Flags]
    public enum CastlingRights
    {
        WhiteKingSide = 1,
        WhiteQueenSide = 2,
        BlackKingSide = 4,
        BlackQueenSide = 8
    }

    private Bitboard WhiteKing = new(0x0000000000000010);
    private Bitboard WhiteQueens = new(0x0000000000000008);
    private Bitboard WhiteBishops = new(0x0000000000000024);
    private Bitboard WhiteKnights = new(0x0000000000000042);
    private Bitboard WhiteRooks = new(0x0000000000000081);
    private Bitboard WhitePawns = new(0x000000000000ff00);

    private Bitboard BlackKing = new(0x1000000000000000);
    private Bitboard BlackQueens = new(0x800000000000000);
    private Bitboard BlackBishops = new(0x2400000000000000);
    private Bitboard BlackKnights = new(0x4200000000000000);
    private Bitboard BlackRooks = new(0x8100000000000000);
    private Bitboard BlackPawns = new(0x00ff000000000000);

    private Bitboard WhiteOccupancy = new(0x000000000000ffff);
    private Bitboard BlackOccupancy = new(0xffff000000000000);
    private Bitboard BoardOccupancy = new(0xffff00000000ffff);

    public Side SideToMove = Side.White;
    public Square EnPassant = Square.NoSquare;
    public CastlingRights Castling = 0;
}