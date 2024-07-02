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
public sealed class MagicNumbers
{
    private readonly PseudoRandomNumberGenerator _generator = new();
    
    public static Bitboard SetOccupancy(int index, int bitsInMask, Bitboard attackMask)
    {
        var attackMaskCopy = attackMask.Copy();
        var occupancy = Bitboard.EmptyBitboard();
        var indexBitboard = new Bitboard(Convert.ToUInt64(index));

        for (var i = 0; i < bitsInMask; ++i)
        {
            var square = attackMaskCopy.GetLeastSignificantBitSetIndex();
            attackMaskCopy.UnsetBit((Board.Square)square);
            if (indexBitboard.GetBit((Board.Square)i) == 1)
            {
                occupancy.SetBit((Board.Square)square);
            }
        }
        return occupancy;
    }
    
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

    public ulong FindMagicNumber(int square, int relevantBits, bool bishop)
    {
        var occupancies = new Bitboard[4096];
        var attacks = new Bitboard[4096];
        var usedAttacks = new Bitboard[4096];
        var attackMask = bishop ? 
            Lookup.CalculateBishopRelevantOccupancyBitboard(square) :
            Lookup.CalculateRookRelevantOccupancyBitboard(square);
        var occupancyIndices = 1 << relevantBits;
        
        for (var i = 0; i < occupancyIndices; ++i)
        {
            occupancies[i] = SetOccupancy(i, relevantBits, attackMask);
            attacks[i] = bishop ? Lookup.CalculateBishopAttacks(square, occupancies[i]) :
                Lookup.CalculateRookAttacks(square, occupancies[i]);
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

    public void InitMagicNumbers()
    {
        for (int square = 0; square < 64; ++square)
        {
            // bishop magic numbers
            var mn = FindMagicNumber(square, Lookup.bishopRelevantBits[square], true );
            Console.WriteLine(mn);
        }
    }
}
