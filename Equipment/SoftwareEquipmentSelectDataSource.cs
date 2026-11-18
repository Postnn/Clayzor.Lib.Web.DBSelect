using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник программного оборудования с типами больше 200.</summary>
public sealed class SoftwareEquipmentSelectDataSource : IClaySelectDataSource<SoftwareEquipmentLookup, int?>
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник с менеджером текущего circuit.</summary>
    public SoftwareEquipmentSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<SoftwareEquipmentLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<SoftwareEquipmentLookup>(SQLQueries.SELECT_ПрограммноеОборудование, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<SoftwareEquipmentLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<SoftwareEquipmentLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<SoftwareEquipmentLookup>(
            SQLQueries.SELECT_ПрограммноеОборудованиеПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
