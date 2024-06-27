namespace ChessAI.Shared;

// See: https://en.wikipedia.org/wiki/Xorshift
public sealed class PseudoRandomNumberGenerator
{
    private uint Seed = 1804289383;

    public uint Next()
    {
        var number = Seed;
        number ^= number << 13;
        number ^= number >> 17;
        number ^= number << 5;
        Seed = number;
        return number;
    }
}