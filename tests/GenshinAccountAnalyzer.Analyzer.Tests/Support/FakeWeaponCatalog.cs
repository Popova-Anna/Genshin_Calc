using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Enums;

namespace GenshinAccountAnalyzer.Analyzer.Tests.Support;

/// <summary>In-memory <see cref="IWeaponCatalog"/> for analyzer tests.</summary>
internal sealed class FakeWeaponCatalog : IWeaponCatalog
{
    private readonly List<WeaponInfo> _weapons = [];

    public FakeWeaponCatalog Add(WeaponInfo weapon)
    {
        _weapons.Add(weapon);
        return this;
    }

    public WeaponInfo? Get(int weaponId) => _weapons.FirstOrDefault(w => w.Id == weaponId);

    public IReadOnlyList<WeaponInfo> GetByType(WeaponType type) =>
        _weapons.Where(w => w.Type == type).ToList();
}
