using ChessAI.Shared;

namespace ChessAI.Model;

public static class Board
{
    public const int NumberOfRanks = 8;
    public const int NumberOfFiles = 8;
    public const int NumberOfSquares = NumberOfRanks * NumberOfFiles;

    public const int NumberOfSides = 2;
    
    public static readonly BidirectionalFrozenDictionary<Piece, char> AsciiRepresentation = new(
    [
        (Piece.None, '.'),
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
}