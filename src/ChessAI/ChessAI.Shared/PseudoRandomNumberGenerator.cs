namespace ChessAI.Shared;

/// <summary>
/// Pseudo random number generator based on the Xorshift algorithm.<br/>
/// See: https://en.wikipedia.org/wiki/Xorshift
/// </summary>
public sealed class PseudoRandomNumberGenerator
{
    // This seed or state makes sure that every time we get the same numbers
    private uint Seed = 1804289383;

    /// <summary>
    /// Generates a random 32-bit number.
    /// </summary>
    /// <returns>A random 32-bit number.</returns>
    public uint Next_32()
    {
        var number = Seed;
        number ^= number << 13;
        number ^= number >> 17;
        number ^= number << 5;
        Seed = number;
        return number;
    }

    /// <summary>
    /// Generates a random 64-bit number.
    /// </summary>
    /// <returns>A random 64-bit number.</returns>
    public ulong Next_64()
    {
        // Generate 4 random 32-bit numbers and keep only the lower 16 bits
        ulong[] numbers =
        [
            (ulong)Next_32() & 0xFFFF, 
            (ulong)Next_32() & 0xFFFF,
            (ulong)Next_32() & 0xFFFF,
            (ulong)Next_32() & 0xFFFF
        ];
        
        // Combine the 4 16-bit numbers to form a 64-bit number
        return numbers[0] | (numbers[1] << 16) | (numbers[2] << 32) | (numbers[3] << 48);
    }
}