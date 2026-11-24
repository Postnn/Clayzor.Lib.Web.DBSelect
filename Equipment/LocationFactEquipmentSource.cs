using System.Globalization;
using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Поиск оборудования для инвентаризации независимо от его текущего места.</summary>
public sealed class LocationFactEquipmentSource(DbManager db) : IClaySelectDataSource<LocationFactMatch, int?>
{
    /// <summary>Ограничивает выдачу IT-оборудованием.</summary>
    public bool ItOnly { get; set; }

    /// <inheritdoc />
    public async Task<ClaySelectResult<LocationFactMatch>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        var matches = (await LocationFactData.SearchAsync(db, request.SearchText, ItOnly, request.Limit + 1, null, cancellationToken)).ToList();
        return new() { Items = matches.Take(request.Limit).ToList(), HasMore = matches.Count > request.Limit };
    }

    /// <inheritdoc />
    public async Task<LocationFactMatch?> GetByIdAsync(int? id, CancellationToken cancellationToken = default) =>
        id is > 0 ? (await LocationFactData.SearchAsync(db, id.Value.ToString(CultureInfo.InvariantCulture), ItOnly, 1, id, cancellationToken)).SingleOrDefault() : null;
}
