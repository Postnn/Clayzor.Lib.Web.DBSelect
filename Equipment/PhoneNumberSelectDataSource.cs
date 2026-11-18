using System.Text;
using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник телефонных номеров с серверным поиском и сведениями АТС.</summary>
public sealed class PhoneNumberSelectDataSource : IClaySelectDataSource<PhoneNumberLookup, int?>
{
    private readonly DbManager _db;

    /// <summary>Текущее расположение оборудования.</summary>
    public int? LocationId { get; set; }

    /// <summary>Текущий ответственный сотрудник.</summary>
    public int? EmployeeId { get; set; }

    /// <summary>Тип оборудования.</summary>
    public int? EquipmentTypeId { get; set; }

    /// <summary>Основной номер текущего оборудования.</summary>
    public int? PhoneIdBase { get; set; }

    /// <summary>Текущее оборудование.</summary>
    public int? EquipmentId { get; set; }

    /// <summary>Номера, уже присутствующие в редактируемом списке.</summary>
    public IReadOnlyCollection<int> ExcludedPhoneIds { get; set; } = [];

    /// <summary>Создаёт источник с подключением текущего circuit.</summary>
    public PhoneNumberSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneNumberLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var parameters = new Dapper.DynamicParameters();
        parameters.Add("@LocationId", LocationId);
        parameters.Add("@EmployeeId", EmployeeId);
        parameters.Add("@EquipmentTypeId", EquipmentTypeId);
        parameters.Add("@PhoneIdBase", PhoneIdBase);
        parameters.Add("@EquipmentId", EquipmentId);
        parameters.Add("@Search", request.SearchText);
        parameters.Add("@LimitPlusOne", request.Limit + 1);
        var sql = new StringBuilder(SQLQueries.SELECT_ТелефонныеНомераДляВыбора);
        PhoneNumberQuery.AddExcludedIds(sql, parameters, ExcludedPhoneIds);
        var rows = (await _db.QueryAsync<PhoneNumberLookup>(sql.ToString(), parameters)).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<PhoneNumberLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<PhoneNumberLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<PhoneNumberLookup>(
            SQLQueries.SELECT_ТелефонныйНомерПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
