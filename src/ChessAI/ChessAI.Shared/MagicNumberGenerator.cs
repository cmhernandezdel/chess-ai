namespace ChessAI.Shared;

public sealed class MagicNumberGenerator
{
    public ulong GenerateMagicNumberCandidate()
    {
        var generator = new PseudoRandomNumberGenerator();
        return generator.Next_64() & generator.Next_64() & generator.Next_64();
    }
}