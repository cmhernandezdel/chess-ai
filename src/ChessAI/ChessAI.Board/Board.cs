using System.Text;
using ChessAI.Shared;

namespace ChessAI.Board;

// 8 00 01 02 03 04 05 06 07
// 7 08 09 10 11 12 13 14 15
// 6 16 17 18 19 20 21 22 23
// 5 24 25 26 27 28 29 30 31
// 4 32	33	34	35	36	37	38	39
// 3 40	41	42	43	44	45	46	47
// 2 48	49	50	51	52	53	54	55
// 1 56	57	58	59	60	61	62	63
// - A  B  C  D  E  F  G  H 
/// <summary>
/// Representation of the board.
/// Layout 0: https://gekomad.github.io/Cinnamon/BitboardCalculator/
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
        None = -1,
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

    private Bitboard WhiteKing = new(0x1000000000000000);
    private Bitboard WhiteQueens = new(0x800000000000000);
    private Bitboard WhiteBishops = new(0x2400000000000000);
    private Bitboard WhiteKnights = new(0x4200000000000000);
    private Bitboard WhiteRooks = new(0x8100000000000000);
    private Bitboard WhitePawns = new(0xff000000000000);

    private Bitboard BlackKing = new(0x0000000000000010);
    private Bitboard BlackQueens = new(0x0000000000000008);
    private Bitboard BlackBishops = new(0x0000000000000024);
    private Bitboard BlackKnights = new(0x0000000000000042);
    private Bitboard BlackRooks = new(0x0000000000000081);
    private Bitboard BlackPawns = new(0x000000000000ff00);

    private Bitboard[] PieceBitboards;

    private Bitboard WhiteOccupancy = new(0xffff000000000000);
    private Bitboard BlackOccupancy = new(0x000000000000ffff);
    private Bitboard BoardOccupancy = new(0xffff00000000ffff);

    public Side SideToMove = Side.White;
    public Square EnPassant = Square.NoSquare;
    public CastlingRights Castling = 0;

    private BidirectionalFrozenDictionary<Piece, char> AsciiRepresentation = new(
        [
            (Piece.WhitePawn, 'P'), 
            (Piece.WhiteKnight, 'N'), 
            (Piece.WhiteBishop, 'B'), 
            (Piece.WhiteRook, 'R'),
            (Piece.WhiteQueen, 'Q'),
            (Piece.WhiteKing, 'K'),
            (Piece.BlackPawn, 'p'),
            (Piece.BlackKnight, 'n'),
            (Piece.BlackBishop, 'b'),
            (Piece.BlackRook, 'r'),
            (Piece.BlackQueen, 'q'),
            (Piece.BlackKing, 'k')
        ]);

    public Board()
    {
        PieceBitboards =
        [
            WhitePawns, WhiteKnights, WhiteBishops, WhiteRooks, WhiteQueens, WhiteKing,
            BlackPawns, BlackKnights, BlackBishops, BlackRooks, BlackQueens, BlackKing
        ];
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (var rank = 0; rank < 8; rank++)
        {
            sb.Append($"{8 - rank}");
            for (var file = 0; file < 8; file++)
            {
                var square = rank * 8 + file;
                Piece piece = Piece.None;

                for (int i = 0; i < PieceBitboards.Length; i++)
                {
                    byte b = PieceBitboards[i].GetBit((Square)square);
                    if (b == 1)
                    {
                        piece = (Piece)i;
                    }
                }
                
                sb.Append($" {(piece == Piece.None ? '.' : AsciiRepresentation.GetValue(piece))}");
            }

            sb.AppendLine();
        }

        sb.Append("  a b c d e f g h");
        sb.AppendLine();

        return sb.ToString();
    }
}