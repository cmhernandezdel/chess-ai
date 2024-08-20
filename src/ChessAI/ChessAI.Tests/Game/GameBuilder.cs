using ChessAI.Model;
using ChessGame = ChessAI.Game.Game;

namespace ChessAI.Tests.Game;

public sealed class GameBuilder
{
    private Side _sideToMove = Side.White;
    private Square _enPassant = Square.None;
    private CastlingRights _castling = 0;
    private int _halfMovesSinceLastCaptureOrPawnAdvance = 0;
    private int _fullMoves = 0;
    
    private Bitboard _whiteKing = Bitboard.EmptyBitboard();
    private Bitboard _whiteQueens = Bitboard.EmptyBitboard();
    private Bitboard _whiteBishops = Bitboard.EmptyBitboard();
    private Bitboard _whiteKnights = Bitboard.EmptyBitboard();
    private Bitboard _whiteRooks = Bitboard.EmptyBitboard();
    private Bitboard _whitePawns = Bitboard.EmptyBitboard();

    private Bitboard _blackKing = Bitboard.EmptyBitboard();
    private Bitboard _blackQueens = Bitboard.EmptyBitboard();
    private Bitboard _blackBishops = Bitboard.EmptyBitboard();
    private Bitboard _blackKnights = Bitboard.EmptyBitboard();
    private Bitboard _blackRooks = Bitboard.EmptyBitboard();
    private Bitboard _blackPawns = Bitboard.EmptyBitboard();
    
    private readonly ChessGame _game = new();

    public ChessGame Build()
    {
        _game.WhiteKing = _whiteKing;
        _game.WhiteQueens = _whiteQueens;
        _game.WhiteRooks = _whiteRooks;
        _game.WhiteBishops = _whiteBishops;
        _game.WhiteKnights = _whiteKnights;
        _game.WhitePawns = _whitePawns;
        
        _game.BlackKing = _blackKing;
        _game.BlackQueens = _blackQueens;
        _game.BlackRooks = _blackRooks;
        _game.BlackBishops = _blackBishops;
        _game.BlackKnights = _blackKnights;
        _game.BlackPawns = _blackPawns;
        
        _game.SideToMove = _sideToMove;
        _game.EnPassant = _enPassant;
        _game.Castling = _castling;
        _game.HalfMovesSinceLastCaptureOrPawnAdvance = _halfMovesSinceLastCaptureOrPawnAdvance;
        _game.FullMoves = _fullMoves;

        return _game;
    }

    public GameBuilder WithSideToMove(Side side)
    {
        _sideToMove = side;
        return this;
    }

    public GameBuilder WithEnPassant(Square enPassantSquare)
    {
        _enPassant = enPassantSquare;
        return this;
    }

    public GameBuilder WithHalfMovesSinceLastCaptureOrPawnAdvance(int halfMovesSinceLastCaptureOrPawnAdvance)
    {
        _halfMovesSinceLastCaptureOrPawnAdvance = halfMovesSinceLastCaptureOrPawnAdvance;
        return this; 
    }

    public GameBuilder WithFullMoves(int fullMoves)
    {
        _fullMoves = fullMoves;
        return this;
    }

    public GameBuilder WithCastlingRights(CastlingRights castlingRights)
    {
        _castling = castlingRights;
        return this;
    }

    public GameBuilder WithWhiteKing(Square square)
    {
        _whiteKing = Bitboard.EmptyBitboard();
        _whiteKing.SetBit(square);
        return this;
    }

    public GameBuilder WithWhiteQueens(IEnumerable<Square> squares)
    {
        _whiteQueens = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _whiteQueens.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithWhiteRooks(IEnumerable<Square> squares)
    {
        _whiteRooks = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _whiteRooks.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithWhiteBishops(IEnumerable<Square> squares)
    {
        _whiteBishops = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _whiteBishops.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithWhiteKnights(IEnumerable<Square> squares)
    {
        _whiteKnights = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _whiteKnights.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithWhitePawns(IEnumerable<Square> squares)
    {
        _whitePawns = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _whitePawns.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithBlackKing(Square square)
    {
        _blackKing = Bitboard.EmptyBitboard();
        _blackKing.SetBit(square);
        return this;
    }

    public GameBuilder WithBlackQueens(IEnumerable<Square> squares)
    {
        _blackQueens = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _blackQueens.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithBlackRooks(IEnumerable<Square> squares)
    {
        _blackRooks = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _blackRooks.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithBlackBishops(IEnumerable<Square> squares)
    {
        _blackBishops = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _blackBishops.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithBlackKnights(IEnumerable<Square> squares)
    {
        _blackKnights = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _blackKnights.SetBit(square);
        }

        return this;
    }
    
    public GameBuilder WithBlackPawns(IEnumerable<Square> squares)
    {
        _blackPawns = Bitboard.EmptyBitboard();
        foreach (var square in squares)
        {
            _blackPawns.SetBit(square);
        }

        return this;
    }
}