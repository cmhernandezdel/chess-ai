namespace ChessAI.Board;

// Ref: https://www.chessprogramming.org/Square_Mapping_Considerations#LittleEndianRankFileMapping
// Ref: https://www.chessprogramming.org/General_Setwise_Operations
public static class Movements
{
    private const ulong aFileMask = 0x0101010101010101;
    private const ulong hFileMask = 0x8080808080808080;

    // Move from xN to x(N+1) where x is a file (A-H) and N is a rank (1-8)
    public static ulong UpOneRank(ulong board) => board << 8;

    // Move from xN to x(N-1) where x is a file (A-H) and N is a rank (1-8)
    public static ulong DownOneRank(ulong board) => board >> 8;

    // Move from xN to (x+1)N where x is a file (A-H) and N is a rank (1-8)
    public static ulong UpOneFile(ulong board) => (board << 1) & ~aFileMask;

    // Move from xN to (x-1)N where x is a file (A-H) and N is a rank (1-8)
    public static ulong DownOneFile(ulong board) => (board >> 1) & ~hFileMask;

    public static ulong UpOneRankUpOneFile(ulong board) => (board << 9) & ~aFileMask;

    public static ulong DownOneRankUpOneFile(ulong board) => (board >> 7) & ~aFileMask;

    public static ulong UpOneRankDownOneFile(ulong board) => (board << 7) & ~hFileMask;

    public static ulong DownOneRankDownOneFile(ulong board) => (board >> 9) & ~hFileMask;
}
