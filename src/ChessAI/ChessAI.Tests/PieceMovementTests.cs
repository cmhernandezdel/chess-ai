using ChessAI.Board.Pieces;

namespace ChessAI.Tests;

public sealed class PieceMovementTests
{
    public static IEnumerable<object[]> DoublePushTestData =>
        new List<object[]>
    {
        new object[] { 0x000000000000ff00, 0x0000ffffffff0000, 0x00000000ff000000 }, // initial position
        new object[] { 0x000000001000ef00, 0x4010dfefebff1020, 0x00000000eb000000 }, // position after 1. e4 Nf6 2. Bc4
        new object[] { 0x00000000c00f300, 0x0018eff7f3df0c40, 0x00000000d3000000 }, // position after 1. d4 d5 2. c4 e6 3. Nf3
        new object[] { 0x0000000088006700, 0x048c3bff779f9842, 0x0000000007000000 } // position after 1. e4 c6 2. d4 d5 3. Nc3 dxe4 4. Nxe4 Bf5 5. Ng3 Bg6 6. h4 h6 7. Nf3
    };
       
    [Theory]
    [MemberData(nameof(DoublePushTestData))]
    public void Test_GetWhitePawnsDoublePushes(ulong startingPosition, ulong emptySquares, ulong expectedPosition)
    {
        ulong finalPosition = Pawn.GetWhiteDoublePushes(startingPosition, emptySquares);
        Assert.Equal(expectedPosition, finalPosition);
    }
}