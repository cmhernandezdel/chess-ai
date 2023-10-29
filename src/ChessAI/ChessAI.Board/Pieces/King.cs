namespace ChessAI.Board.Pieces;

public static class King
{
    // targets can be empty squares for quiet moves or opposing side pieces for captures
    public static ulong GetMoves(ulong ownKing, ulong targets)
    {
        return (ownKing.UpOneRank() | ownKing.DownOneRank() | ownKing.UpOneFile() | ownKing.DownOneFile() |
            ownKing.UpOneRank().UpOneFile() | ownKing.UpOneRank().DownOneFile() | ownKing.DownOneRank().DownOneFile() | ownKing.DownOneRank().UpOneFile())
            & targets;
    }
}
