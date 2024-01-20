using System.Text;

namespace ChessAI.Board;

/// <summary>
/// Representation of a bitboard. This is a wrapper around an unsigned long,
/// but it has methods for bitwise operations.
/// https://www.chessprogramming.org/General_Setwise_Operations#Bit_by_Square
/// </summary>
public sealed class Bitboard
{
    public ulong Value { get; private set; }

    public Bitboard(ulong value)
    {
        Value = value;
    }

    public byte GetBit(Board.Square square) => GetBit((int)square);

    public void SetBit(Board.Square square) => SetBit((int)square);

    public void UnsetBit(Board.Square square) => UnsetBit((int)square);

    public byte GetBit(int squareIndex)
    {
        ulong a = (ulong)1 << squareIndex;
        ulong val = Value & a;
        return val > 0 ? (byte)1 : (byte)0;
    }

    public void SetBit(int squareIndex)
    {
        Value |= (ulong)1 << squareIndex;
    }

    public void UnsetBit(int squareIndex)
    {
        Value &= ~((ulong)1 << squareIndex);
    }

    public override string ToString()
    {
        StringBuilder sb = new("\n");

        for (int rank = 0; rank < 8; rank++)
        {
            for (int file = 0; file < 8; file++)
            {
                int square = rank * 8 + file;
                if (file == 0) sb.AppendFormat("  {0} ", 8 - rank);
                sb.AppendFormat(" {0}", GetBit(square));
            }
            sb.Append('\n');
        }

        sb.Append("\n     a b c d e f g h\n");
        sb.Append('\n');
        sb.AppendFormat("Bitboard: {0}", Value);
        return sb.ToString();
    }
}