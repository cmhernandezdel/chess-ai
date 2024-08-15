using ChessAI.Model;

namespace ChessAI.Game;

/// <summary>
/// Allows building games from FEN notation.
/// </summary>
public class FenBuilder
{
    private readonly Game _game = new();
    
    /// <summary>
    /// Returns a game represented by a FEN notation string.
    /// </summary>
    /// <param name="fen">The FEN string to parse</param>
    /// <returns>A Game instance.</returns>
    public Game Build(string fen)
    {
        var parts = fen.Split(' ');
        ParseFenPiecePlacement(parts[0]);
        ParseSideToMove(parts[1]);
        ParseCastlingAbility(parts[2]);
        ParseEnPassantSquare(parts[3]);
        ParseHalfMovesSinceLastCaptureOrPawnAdvance(parts[4]);
        ParseFullMoves(parts[5]);
        return _game;
    }

    private void ParseFenPiecePlacement(string piecePlacementFen)
    {
        var file = 0;
        var rank = 0;
        foreach (var c in piecePlacementFen)
        {
            switch (c)
            {
                case >= 'a' and <= 'z' or >= 'A' and <= 'Z':
                {
                    var square = (Square)(rank * Board.NumberOfRanks + file);
                    var pieceIndex = (int)Board.AsciiRepresentation.GetValue(c);
                    _game.PieceBitboards[pieceIndex].SetBit(square);
                    file++;
                    break;
                }
                case >= '0' and <= '9':
                    file += Convert.ToInt32(c);
                    break;
            }

            rank += file / Board.NumberOfFiles;
            file %= Board.NumberOfFiles;
        }
    }

    private void ParseSideToMove(string sideToMoveFen)
    {
        _game.SideToMove = sideToMoveFen[0] == 'w' ? Side.White : Side.Black;
    }

    private void ParseCastlingAbility(string castlingAbilityFen)
    {
        foreach (var c in castlingAbilityFen)
        {
            switch (c)
            {
                case 'K':
                    _game.Castling |= CastlingRights.WhiteKingSide;
                    break;
                case 'Q':
                    _game.Castling |= CastlingRights.WhiteQueenSide;
                    break;
                case 'k':
                    _game.Castling |= CastlingRights.BlackKingSide;
                    break;
                case 'q':
                    _game.Castling |= CastlingRights.BlackQueenSide;
                    break;
                case '-':
                    _game.Castling = CastlingRights.None;
                    break;
            }
        }
    }

    private void ParseEnPassantSquare(string enPassantSquareFen)
    {
        if (enPassantSquareFen == "-")
        {
            _game.EnPassant = Square.None;
        }
        
        Enum.TryParse(enPassantSquareFen, out Square enPassant);
        _game.EnPassant = enPassant;
    }

    private void ParseHalfMovesSinceLastCaptureOrPawnAdvance(string halfMovesFen)
    {
        _game.HalfMovesSinceLastCaptureOrPawnAdvance = Convert.ToInt32(halfMovesFen);
    }

    private void ParseFullMoves(string fullMovesFen)
    {
        _game.FullMoves = Convert.ToInt32(fullMovesFen);
    }
}