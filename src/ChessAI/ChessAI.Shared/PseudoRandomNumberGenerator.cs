namespace ChessAI.Shared;

// See: https://en.wikipedia.org/wiki/Xorshift
public sealed class PseudoRandomNumberGenerator
{
    private uint Seed = 1804289383;

    public uint Next_32()
    {
        var number = Seed;
        number ^= number << 13;
        number ^= number >> 17;
        number ^= number << 5;
        Seed = number;
        return number;
    }

    public ulong Next_64()
    {
        ulong[] numbers =
        [
            Next_32() & 0xFFFF, 
            Next_32() & 0xFFFF,
            Next_32() & 0xFFFF,
            Next_32() & 0xFFFF
        ];
        
        return numbers[0] | (numbers[1] << 16) | (numbers[2] << 32) | (numbers[3] << 48);
    }
}