namespace ChessAI.Board.Pieces;

public static class Pawn
{
    public static ulong GetWhiteSinglePushes(ulong startingPosition, ulong emptySquares)
        => startingPosition.UpOneRank() & emptySquares;

    public static ulong GetWhiteDoublePushes(ulong startingPosition, ulong emptySquares)
        => GetWhiteSinglePushes(startingPosition, emptySquares).UpOneRank() & emptySquares & Board.RANK_4;

    public static ulong GetWhitePawnCaptures(ulong whitePawns, ulong blackPieces)
        => (whitePawns.UpOneRankUpOneFile() & blackPieces) | (whitePawns.UpOneRankDownOneFile() & blackPieces);

    public static ulong GetWhitePawnPromotions(ulong whitePawns, ulong emptySquares, ulong blackPieces)
        => (GetWhiteSinglePushes(whitePawns, emptySquares) | GetWhitePawnCaptures(whitePawns, blackPieces)) & Board.RANK_8;
}
