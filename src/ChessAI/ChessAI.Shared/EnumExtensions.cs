namespace ChessAI.Shared;

public static class EnumExtensions
{
    public static int Count<T>() where T : struct, IConvertible
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be of type enum.");
        }

        return Enum.GetNames(typeof(T)).Length;
    }
}
