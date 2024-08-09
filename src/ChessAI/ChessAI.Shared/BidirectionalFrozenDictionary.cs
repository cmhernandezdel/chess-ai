using System.Collections.Frozen;

namespace ChessAI.Shared;

public sealed class BidirectionalFrozenDictionary<T1, T2> 
    where T1 : notnull 
    where T2 : notnull
{
    private readonly FrozenDictionary<T1, T2> _dict1;
    private readonly FrozenDictionary<T2, T1> _dict2;

    public BidirectionalFrozenDictionary(IEnumerable<(T1, T2)> source)
    {
        _dict1 = source.ToFrozenDictionary(t => t.Item1, t => t.Item2);
        _dict2 = source.ToFrozenDictionary(t => t.Item2, t => t.Item1);
    }

    public T2 GetValue(T1 key)
    {
        return _dict1[key];
    }

    public T1 GetValue(T2 key)
    {
        return _dict2[key];
    }
}