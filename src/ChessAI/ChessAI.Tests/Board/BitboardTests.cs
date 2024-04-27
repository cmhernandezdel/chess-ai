using ChessAI.Board;

namespace ChessAI.Tests.Board;

// Tool used to convert from decimal to binary
// https://www.rapidtables.com/convert/number/decimal-to-binary.html
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
}
