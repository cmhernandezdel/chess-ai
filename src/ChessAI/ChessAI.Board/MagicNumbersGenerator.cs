using ChessAI.Shared;

namespace ChessAI.Board;

/// <summary>
/// This class contains all the logic to generate the magic numbers to be used as hash keys for the lookup tables.
/// Since sliding pieces move generation is time expensive, we store them in a lookup table.
/// Since there are 12 relevant squares at most for a sliding piece (the rook, in this case), we get 2^12 configurations,
/// so there are 4096 entries in the lookup tables. Bishop require a little bit less than that, because there are at most
/// 9 relevant squares. <br/>
/// See: https://analog-hors.github.io/site/magic-bitboards/
/// </summary>
public sealed class MagicNumbersGenerator
{
    private readonly PseudoRandomNumberGenerator _generator = new();
    
    
    
    /// <summary>
    /// Generates a magic number candidate. <br/>
    /// The best way to find a magic number is by trial and error, so we will generate a lot of candidates
    /// and only use those who are valid.
    /// </summary>
    /// <returns>A magic number candidate.</returns>
    private ulong GenerateMagicNumberCandidate()
    {
        return _generator.Next_64() & _generator.Next_64() & _generator.Next_64();
    }

    /// <summary>
    /// Tries to find a valid magic number for bishop lookup table.
    /// </summary>
    /// <param name="square">The square to find the magic number for.</param>
    /// <returns>The magic number.</returns>
    private ulong FindBishopMagicNumberForSquare(int square)
    {
        var occupancies = new Bitboard[4096];
        var attacks = new Bitboard[4096];
        var usedAttacks = new Bitboard[4096];
        var relevantBits = Lookup.BishopRelevantBits[square];
        var attackMask = Lookup.CalculateBishopRelevantOccupancyBitboard(square);
        var occupancyIndices = 1 << relevantBits;
        
        for (var i = 0; i < occupancyIndices; ++i)
        {
            occupancies[i] = attackMask.SetOccupancy(i, relevantBits);
            attacks[i] = Lookup.CalculateBishopAttacks(square, occupancies[i]);
        }

        for (var randomCount = 0; randomCount < 100000000; ++randomCount)
        {
            var magicNumber = GenerateMagicNumberCandidate();
            var x = attackMask * magicNumber;
            x &= new Bitboard(0xFF00000000000000);
            if (x.CountBits() < 6) continue;
            var failed = false;

            Array.Fill(usedAttacks, Bitboard.EmptyBitboard());
            for (var index = 0; index < occupancyIndices; ++index)
            {
                if (failed) break;
                var magicIndex = Convert.ToInt32(((occupancies[index] * magicNumber) >> (64 - relevantBits)).Value);
                if (usedAttacks[magicIndex] == Bitboard.EmptyBitboard())
                {
                    usedAttacks[magicIndex] = attacks[index];
                }
                else if (usedAttacks[magicIndex] != attacks[index])
                {
                    failed = true;
                }
            }

            if (!failed) return magicNumber;
        }
        
        return 0ul;
    }

    /// <summary>
    /// Tries to find a valid magic number for rook lookup table.
    /// </summary>
    /// <param name="square">The square to find the magic number for.</param>
    /// <returns>The magic number.</returns>
    private ulong FindRookMagicNumberForSquare(int square)
    {
        var occupancies = new Bitboard[4096];
        var attacks = new Bitboard[4096];
        var usedAttacks = new Bitboard[4096];
        var relevantBits = Lookup.RookRelevantBits[square];
        var attackMask = Lookup.CalculateRookRelevantOccupancyBitboard(square);
        var occupancyIndices = 1 << relevantBits;
        
        for (var i = 0; i < occupancyIndices; ++i)
        {
            occupancies[i] = attackMask.SetOccupancy(i, relevantBits);
            attacks[i] = Lookup.CalculateRookAttacks(square, occupancies[i]);
        }

        for (var randomCount = 0; randomCount < 100000000; ++randomCount)
        {
            var magicNumber = GenerateMagicNumberCandidate();
            var x = attackMask * magicNumber;
            x &= new Bitboard(0xFF00000000000000);
            if (x.CountBits() < 6) continue;
            var failed = false;

            Array.Fill(usedAttacks, Bitboard.EmptyBitboard());
            for (var index = 0; index < occupancyIndices; ++index)
            {
                if (failed) break;
                var magicIndex = Convert.ToInt32(((occupancies[index] * magicNumber) >> (64 - relevantBits)).Value);
                if (usedAttacks[magicIndex] == Bitboard.EmptyBitboard())
                {
                    usedAttacks[magicIndex] = attacks[index];
                }
                else if (usedAttacks[magicIndex] != attacks[index])
                {
                    failed = true;
                }
            }

            if (!failed) return magicNumber;
        }
        
        return 0ul;
    }

    /// <summary>
    /// Initializes the magic numbers for the lookup tables.
    /// You can call this function, but it is faster if you calculate them and then use them as constants.
    /// </summary>
    public Tuple<ulong[], ulong[]> InitMagicNumbers()
    {
        var bishopMagicNumbers = new ulong[64];
        var rookMagicNumbers = new ulong[64];
        for (var square = 0; square < 64; ++square)
        {
            // bishop magic numbers
            bishopMagicNumbers[square] = FindBishopMagicNumberForSquare(square);
            rookMagicNumbers[square] = FindRookMagicNumberForSquare(square);
        }
        return new Tuple<ulong[], ulong[]>(bishopMagicNumbers, rookMagicNumbers);
    }
}
