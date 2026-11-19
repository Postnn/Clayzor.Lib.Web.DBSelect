using System.Text;
using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;
using Dapper;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Режим бизнес-фильтра справочника телефонных номеров.</summary>
public enum PhoneDirectoryMode
{
    /// <summary>Члены PBX-группы на выбранной станции.</summary>
    Group,

    /// <summary>Только независимые ведущие номера.</summary>
    DrivenBy,

    /// <summary>Номера без ведущего и отображаемого.</summary>
    DisplayedFrom,
}

/// <summary>Телефонные номера с раздельными фильтрами группы, ведущих и отображаемых.</summary>
public sealed class PhoneDirectorySelectDataSource(DbManager db) : IClaySelectDataSource<PhoneDirectoryItem, int?>
{
    /// <summary>Режим фильтрации.</summary>
    public PhoneDirectoryMode Mode { get; set; }

    /// <summary>Обязательная для поиска станция.</summary>
    public int? StationId { get; set; }

    /// <summary>Текущий телефон и уже добавленные номера.</summary>
    public IReadOnlyCollection<int> ExcludedIds { get; set; } = [];

    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneDirectoryItem>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (StationId is null) return new ClaySelectResult<PhoneDirectoryItem>();
        var statement = Mode switch
        {
            PhoneDirectoryMode.Group => SQLQueries.SELECT_ТелефонЧленыГруппыДляВыбора,
            PhoneDirectoryMode.DrivenBy => SQLQueries.SELECT_ТелефонВедущиеДляВыбора,
            _ => SQLQueries.SELECT_ТелефонОтображаемыеДляВыбора,
        };
        var sql = new StringBuilder(statement);
        var parameters = new DynamicParameters();
        parameters.Add("@StationId", StationId.Value);
        parameters.Add("@Search", request.SearchText);
        parameters.Add("@LimitPlusOne", request.Limit + 1);
        PhoneSelectQuery.Exclude(sql, parameters, "P.КодТелефонногоНомера", ExcludedIds);
        sql.Append(" ORDER BY Телефон");
        var rows = await db.QueryAsync<PhoneDirectoryItem>(sql.ToString(), parameters);
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<PhoneDirectoryItem?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneDirectoryItem>(SQLQueries.SELECT_ТелефонСправочныйПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
