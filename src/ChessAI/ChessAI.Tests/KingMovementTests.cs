using ChessAI.Board.Pieces;

namespace ChessAI.Tests;

public sealed class KingMovementTests
{
    public static IEnumerable<object[]> TestData =>
        new List<object[]>
    {
        new object[] { 0x10, 0xffffffff0000, 0x0 }, // initial position
        new object[] { 0x8000000, 0xfffffffff7ffffff, 0x1c141c0000 }, // king on d4, all other squares empty
        new object[] { 0x1000000, 0xfffffffffeffffff, 0x302030000 }, // king on a4, all other squares empty
        new object[] { 0x80000000, 0xffffffff7fffffff, 0xc040c00000 }, // king on h4, all other squares empty
        new object[] { 0x800000000000000, 0xf7ffffffffffffff, 0x141c000000000000 }, // king on d8, all other squares empty
        new object[] { 0x8, 0xfffffffffffffff7, 0x1c14 }, // king on d1, all other squares empty
        new object[] { 0x1, 0xfffffffffffffffe, 0x302 }, // king on a1, all other squares empty
        new object[] { 0x80, 0xffffffffffffff7f, 0xc040 }, // king on h1, all other squares empty
        new object[] { 0x100000000000000, 0xfeffffffffffffff, 0x203000000000000 }, // king on a8, all other squares empty
        new object[] { 0x8000000000000000, 0x7fffffffffffffff, 0x40c0000000000000 }, // king on h8, all other squares empty
        new object[] { 0x8000000, 0xfffffffbe7ffffff, 0x18041c0000 }, // king on d4, c5 and e4 occupied
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void TestKingMovement(ulong kingInitialPosition, ulong targetSquares, ulong kingExpectedPosition)
    {
        ulong moves = King.GetMoves(kingInitialPosition, targetSquares);
        Assert.Equal(kingExpectedPosition, moves);
    }
}
