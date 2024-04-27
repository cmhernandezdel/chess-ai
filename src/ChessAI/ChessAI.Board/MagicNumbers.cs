namespace ChessAI.Board;

public sealed class MagicNumbers
{
    public static Bitboard SetOccupancy(int index, int bitsInMask, Bitboard attackMask)
    {
        Bitboard occupancy = Bitboard.EmptyBitboard();
        Bitboard indexBB = new(Convert.ToUInt64(index));

        for (int i = 0; i < bitsInMask; ++i)
        {
            int square = attackMask.GetLeastSignificantBitSetIndex();
            attackMask.UnsetBit((Board.Square)square);
            if (indexBB.GetBit((Board.Square)i) == 1)
            {
                occupancy.SetBit((Board.Square)square);
            }
        }
        return occupancy;
    }
}
