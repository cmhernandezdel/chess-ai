namespace ChessAI.Board.Pieces;

public static class Knight
{
    // targets can be empty squares for quiet moves or opposing side pieces for captures
    public static ulong GetMoves(ulong ownKnights, ulong targets, ulong ownPieces)
    {
        return 
            ((ownKnights.UpOneFile().UpOneFile().UpOneRank() & targets) |
            (ownKnights.UpOneFile().UpOneFile().DownOneRank() & targets) |
            (ownKnights.UpOneFile().UpOneRank().UpOneRank() & targets) |
            (ownKnights.UpOneFile().DownOneRank().DownOneRank() & targets) |
            (ownKnights.DownOneFile().DownOneFile().UpOneRank() & targets) |
            (ownKnights.DownOneFile().DownOneFile().DownOneRank() & targets) |
            (ownKnights.DownOneFile().UpOneRank().UpOneRank() & targets) |
            (ownKnights.DownOneFile().DownOneRank().DownOneRank() & targets)) & ~ownPieces;
    }
}
