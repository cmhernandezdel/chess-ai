using System.Text;
using ChessAI.Model;

namespace ChessAI.Game;

// 8 00 01 02 03 04 05 06 07
// 7 08 09 10 11 12 13 14 15
// 6 16 17 18 19 20 21 22 23
// 5 24 25 26 27 28 29 30 31
// 4 32 33 34 35 36 37 38 39
// 3 40 41 42 43 44 45 46 47
// 2 48 49 50 51 52 53 54 55
// 1 56 57 58 59 60 61 62 63
// - A  B  C  D  E  F  G  H 

/// <summary>
/// Representation of the board.
/// Layout 0: https://gekomad.github.io/Cinnamon/BitboardCalculator/
/// </summary>
public sealed class Game
{
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

    public Bitboard[] PieceBitboards;

    private Bitboard WhiteOccupancy = new(0xffff000000000000);
    private Bitboard BlackOccupancy = new(0x000000000000ffff);
    private Bitboard BoardOccupancy = new(0xffff00000000ffff);

    public Side SideToMove = Side.White;
    public Square EnPassant = Square.None;
    public CastlingRights Castling = 0;
    public int HalfMovesSinceLastCaptureOrPawnAdvance = 0;
    public int FullMoves = 0;

    public Game()
    {
        PieceBitboards =
        [
            WhitePawns, WhiteKnights, WhiteBishops, WhiteRooks, WhiteQueens, WhiteKing,
            BlackPawns, BlackKnights, BlackBishops, BlackRooks, BlackQueens, BlackKing
        ];
    }

    public static Game FromFenString(string fen)
    {
        var game = new FenBuilder().Build(fen);
        return game;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (var rank = 0; rank < Board.NumberOfRanks; rank++)
        {
            sb.Append($"{Board.NumberOfRanks - rank}");
            for (var file = 0; file < Board.NumberOfFiles; file++)
            {
                var square = rank * Board.NumberOfRanks + file;
                Piece piece = Piece.None;

                for (int i = 0; i < PieceBitboards.Length; i++)
                {
                    byte b = PieceBitboards[i].GetBit((Square)square);
                    if (b == 1)
                    {
                        piece = (Piece)i;
                    }
                }

                sb.Append($" {(piece == Piece.None ? '.' : Board.AsciiRepresentation.GetValue(piece))}");
            }

            sb.AppendLine();
        }

        sb.Append("  a b c d e f g h");
        sb.AppendLine();

        return sb.ToString();
    }
    
}