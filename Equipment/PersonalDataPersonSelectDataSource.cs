using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник лиц ПДн, допустимых для договора и SIM.</summary>
public sealed class PersonalDataPersonSelectDataSource : IClaySelectDataSource<PersonLookup, int?>
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник с менеджером текущего circuit.</summary>
    public PersonalDataPersonSelectDataSource(DbManager db) => _db = db;

    /// <summary>Идентификатор оборудования.</summary>
    public int? EquipmentId { get; set; }

    /// <summary>Выбранный договор.</summary>
    public int? ContractId { get; set; }

    /// <summary>Тип лица заказчика.</summary>
    public int CustomerType { get; set; }

    /// <summary>Шаблоны технических тарифов из конфигурации.</summary>
    public string TechnicalTariffs { get; set; } = string.Empty;

    /// <inheritdoc />
    public async Task<ClaySelectResult<PersonLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryRequiredAsync<PersonLookup>(SQLQueries.SELECT_ЛицаПерсональныхДанных, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
            TechnicalTariffs,
            ContractId,
            EquipmentId,
            CustomerType,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<PersonLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<PersonLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryRequiredAsync<PersonLookup>(
            SQLQueries.SELECT_ЛицоПерсональныхДанныхПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
