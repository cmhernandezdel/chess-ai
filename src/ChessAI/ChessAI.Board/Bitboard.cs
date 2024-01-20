using System.Text;

namespace ChessAI.Board;

/// <summary>
/// Representation of a bitboard. This is a wrapper around an unsigned long,
/// but it has methods for bitwise operations. A bitboard is nothing else but
/// the representation of the positions of a certain type of piece in the board.
/// https://www.chessprogramming.org/General_Setwise_Operations#Bit_by_Square
/// </summary>
public class Bitboard(ulong value)
{
    public ulong Value { get; private set; } = value;

    // -- FACTORY METHODS --
    public static Bitboard EmptyBitboard() => new(0);

    // -- OPERATORS --
    public static Bitboard operator &(Bitboard a, Bitboard b) => new(a.Value & b.Value);
    public static Bitboard operator |(Bitboard a, Bitboard b) => new(a.Value | b.Value);
    public static Bitboard operator ~(Bitboard a) => new(~a.Value);
    public static Bitboard operator <<(Bitboard a, int b) => new(a.Value << b);
    public static Bitboard operator >>(Bitboard a, int b) => new(a.Value >> b);

    public static bool operator ==(Bitboard a, Bitboard b) => a.Equals(b);
    public static bool operator !=(Bitboard a, Bitboard b) => !a.Equals(b);

    public static implicit operator string(Bitboard a) => a.ToString();

    // -- BITWISE OPERATORS --
    public byte GetBit(Board.Square square) => GetBit((int)square);
    public void SetBit(Board.Square square) => SetBit((int)square);
    public void UnsetBit(Board.Square square) => UnsetBit((int)square);

    // -- MASKS --
    public static Bitboard A_FILE() => new(0x101010101010101);
    public static Bitboard H_FILE() => new(0x8080808080808080);

    // -- ONE-STEPS --
    // https://www.chessprogramming.org/General_Setwise_Operations#OneStepOnly
    public Bitboard RankUp() => this >> 8;
    public Bitboard RankDown() => this << 8;
    public Bitboard FileUp() => (this << 1) & ~A_FILE();
    public Bitboard FileDown() => (this >> 1) & ~H_FILE();
    public Bitboard RankUpFileDown() => (this << 7) & ~H_FILE();
    public Bitboard RankUpFileUp() => (this << 9) & ~A_FILE();
    public Bitboard RankDownFileDown() => (this >> 9) & ~H_FILE();
    public Bitboard RankDownFileUp() => (this >> 7) & ~A_FILE();

    private byte GetBit(int squareIndex)
    {
        ulong a = (ulong)1 << squareIndex;
        ulong val = Value & a;
        return val > 0 ? (byte)1 : (byte)0;
    }

    private void SetBit(int squareIndex)
    {
        Value |= (ulong)1 << squareIndex;
    }

    private void UnsetBit(int squareIndex)
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

    public override bool Equals(object? obj)
    {
        return obj is Bitboard other &&
            other.Value == Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}