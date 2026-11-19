using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;
using Dapper;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Оборудование из fn_ОборудованиеДляТелНомера с типизированным составным ключом.</summary>
public sealed class PhoneEquipmentSelectDataSource(DbManager db) : IClaySelectDataSource<PhoneEquipmentLookup, PhoneEquipmentKey>
{
    /// <summary>Тип номера.</summary>
    public int? PhoneTypeId { get; set; }

    /// <summary>Станция.</summary>
    public int? StationId { get; set; }

    /// <summary>Розетка.</summary>
    public int? SocketId { get; set; }

    /// <summary>Сотрудник.</summary>
    public int? EmployeeId { get; set; }

    /// <summary>Текущий телефон; 0 соответствует поведению поиска legacy.</summary>
    public int PhoneId { get; set; }

    /// <summary>Уже связанное оборудование.</summary>
    public IReadOnlyCollection<int> ExcludedEquipmentIds { get; set; } = [];

    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneEquipmentLookup>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (PhoneTypeId is null || StationId is null) return new ClaySelectResult<PhoneEquipmentLookup>();
        var sql = PhoneSelectQuery.BeforeOrderBy(SQLQueries.SELECT_ТелефонОборудованиеДляВыбора);
        var parameters = new DynamicParameters();
        parameters.Add("@PhoneTypeId", PhoneTypeId.Value);
        parameters.Add("@StationId", StationId.Value);
        parameters.Add("@SocketId", SocketId);
        parameters.Add("@EmployeeId", EmployeeId);
        parameters.Add("@PhoneId", PhoneId);
        parameters.Add("@Search", request.SearchText);
        parameters.Add("@LimitPlusOne", request.Limit + 1);
        PhoneSelectQuery.Exclude(sql, parameters, "F.КодОборудования", ExcludedEquipmentIds);
        sql.Append(" ORDER BY Название");
        var rows = await db.QueryAsync<PhoneEquipmentLookup>(sql.ToString(), parameters);
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<PhoneEquipmentLookup?> GetByIdAsync(PhoneEquipmentKey? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneEquipmentLookup>(SQLQueries.SELECT_ТелефонОборудованиеПоИдентификатору,
            new { id.EquipmentId, id.ConnectionEquipmentId });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
