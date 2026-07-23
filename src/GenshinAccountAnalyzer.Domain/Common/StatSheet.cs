using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Domain.Common;

/// <summary>
/// An immutable collection of final, computed character statistics keyed by <see cref="StatType"/>.
/// Missing stats resolve to <c>0</c> so callers never have to null-check.
/// </summary>
public sealed class StatSheet
{
    private readonly IReadOnlyDictionary<StatType, double> _values;

    /// <summary>Initializes a new <see cref="StatSheet"/> from a set of stat values.</summary>
    /// <param name="values">The stat values. Duplicate keys keep the last value provided.</param>
    public StatSheet(IReadOnlyDictionary<StatType, double> values)
    {
        _values = values ?? throw new ArgumentNullException(nameof(values));
    }

    /// <summary>An empty sheet where every stat resolves to zero.</summary>
    public static StatSheet Empty { get; } = new(new Dictionary<StatType, double>());

    /// <summary>Gets the value for <paramref name="type"/>, or <c>0</c> when it is not present.</summary>
    /// <param name="type">The stat to read.</param>
    public double this[StatType type] => _values.TryGetValue(type, out double value) ? value : 0d;

    /// <summary>Gets the value for <paramref name="type"/>, or <c>0</c> when it is not present.</summary>
    /// <param name="type">The stat to read.</param>
    /// <returns>The stored value or <c>0</c>.</returns>
    public double Get(StatType type) => this[type];

    /// <summary>Returns <see langword="true"/> when the sheet explicitly contains <paramref name="type"/>.</summary>
    /// <param name="type">The stat to test for.</param>
    public bool Contains(StatType type) => _values.ContainsKey(type);

    /// <summary>All explicitly present stat entries.</summary>
    public IEnumerable<Stat> Entries => _values.Select(kv => new Stat(kv.Key, kv.Value));
}
