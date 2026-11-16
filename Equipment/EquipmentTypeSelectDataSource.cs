using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>
/// Источник типов оборудования для <see cref="ClaySelect{TItem, TValue}"/>.
/// </summary>
public sealed class EquipmentTypeSelectDataSource : IClaySelectDataSource<EquipmentType, int?>
{
    private readonly DbManager _db;

    /// <summary>
    /// Создаёт источник с менеджером подключения текущего circuit.
    /// </summary>
    public EquipmentTypeSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<EquipmentType>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<EquipmentType>(SQLQueries.SELECT_ТипыОборудованияДляВыбора, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();

        return new ClaySelectResult<EquipmentType>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<EquipmentType?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<EquipmentType>(SQLQueries.SELECT_ТипОборудованияПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
