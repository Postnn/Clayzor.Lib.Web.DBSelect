using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>
/// Источник моделей оборудования с фильтрацией по выбранному типу.
/// </summary>
public sealed class EquipmentModelSelectDataSource : IClaySelectDataSource<EquipmentModel, int?>
{
    private readonly DbManager _db;

    /// <summary>
    /// Создаёт источник с менеджером подключения текущего circuit.
    /// </summary>
    public EquipmentModelSelectDataSource(DbManager db) => _db = db;

    /// <summary>Прикладные ограничения текущей формы.</summary>
    public EquipmentModelSelectFilter Filter { get; } = new();

    /// <inheritdoc />
    public async Task<ClaySelectResult<EquipmentModel>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        if (Filter.EquipmentTypeId is null)
            return new ClaySelectResult<EquipmentModel>();

        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<EquipmentModel>(SQLQueries.SELECT_МоделиОборудованияДляВыбора, new
        {
            EquipmentTypeId = Filter.EquipmentTypeId.Value,
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();

        return new ClaySelectResult<EquipmentModel>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<EquipmentModel?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<EquipmentModel>(SQLQueries.SELECT_МодельОборудованияПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
