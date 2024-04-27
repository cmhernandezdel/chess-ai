using ChessAI.Board;
using t_square = ChessAI.Board.Board.Square;

namespace ChessAI.Tests.Board;

// Tool used to convert from decimal to binary:
// https://www.rapidtables.com/convert/number/decimal-to-binary.html

// Tool used to create bitboards (use layout 1):
// https://gekomad.github.io/Cinnamon/BitboardCalculator/
public sealed class BitboardTests
{    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(22, 3)]
    [InlineData(34123, 7)]
    public void CountBits_Returns_CorrectValue(ulong rawBitboard, int expected)
    {
        var bitboard = new Bitboard(rawBitboard);
        var bits = bitboard.CountBits();
        Assert.Equal(expected, bits);
    }

    [Theory]
    [InlineData(22, 1)]
    [InlineData(12893901, 0)]
    [InlineData(4300, 2)]
    public void GetLeastSignificantBitSetIndex_Returns_CorrectValue(ulong rawBitboard, int expected)
    {
        var bitboard = new Bitboard(rawBitboard);
        var index = bitboard.GetLeastSignificantBitSetIndex();
        Assert.Equal(expected, index);
    }

    [Theory]
    [InlineData(0x1c340028006c28, 62, 0x401c340028006c28)]
    [InlineData(0x5c546f28006c28, 16, 0x5c546f28016c28)]
    public void SetBit_Modifies_Bitboard_Correctly(ulong initialRawBitboard, int index, ulong expectedRawBitboard)
    {
        var initialBitboard = new Bitboard(initialRawBitboard);
        var expectedBitboard = new Bitboard(expectedRawBitboard);
        t_square square = (t_square)index;
        initialBitboard.SetBit(square);
        Assert.Equal(expectedBitboard, initialBitboard);
    }

    [Theory]
    [InlineData(0x8006c28, 3, 0x8006c20)]
    [InlineData(0x10304028006c28, 38, 0x10300028006c28)]
    public void UnsetBit_Modifies_Bitboard_Correctly(ulong initialRawBitboard, int index, ulong expectedRawBitboard)
    {
        var initialBitboard = new Bitboard(initialRawBitboard);
        var expectedBitboard = new Bitboard(expectedRawBitboard);
        t_square square = (t_square)index;
        initialBitboard.UnsetBit(square);
        Assert.Equal(expectedBitboard, initialBitboard);
    }
}
