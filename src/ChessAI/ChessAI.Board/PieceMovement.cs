namespace ChessAI.Board;

public static class PieceMovement
{
    public static ulong GetWhitePawnPromotions(ulong whitePawns, Board board)
    {
        ulong whitePawnsInSeventhRank = whitePawns & Board.RANK_7;
        ulong pushPromotions = Movements.UpOneRank(whitePawnsInSeventhRank) & board.GetEmptySquares();
        ulong capturePromotionsLeft = Movements.UpOneRankDownOneFile(whitePawnsInSeventhRank) & board.GetBlackOccupiedSquares();
        ulong capturePromotionsRight = Movements.UpOneRankUpOneFile(whitePawnsInSeventhRank) & board.GetBlackOccupiedSquares();

        return pushPromotions & capturePromotionsLeft & capturePromotionsRight;
    }
}
