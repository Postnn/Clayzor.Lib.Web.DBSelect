using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник оборудования для связи UPS или сервера.</summary>
public sealed class UpsEquipmentSelectDataSource : IClaySelectDataSource<UpsEquipmentLookup, int?>
{
    private readonly DbManager _db;
    private readonly bool _upsOnly;

    /// <summary>Создаёт источник с признаком выбора только UPS.</summary>
    public UpsEquipmentSelectDataSource(DbManager db, bool upsOnly)
    {
        _db = db;
        _upsOnly = upsOnly;
    }

    /// <inheritdoc />
    public async Task<ClaySelectResult<UpsEquipmentLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<UpsEquipmentLookup>(SQLQueries.SELECT_ОборудованиеДляUPS, new
        {
            UpsOnly = _upsOnly ? 1 : 0,
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<UpsEquipmentLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<UpsEquipmentLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<UpsEquipmentLookup>(
            SQLQueries.SELECT_ОборудованиеДляUPSПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
