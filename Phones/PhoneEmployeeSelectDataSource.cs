using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Сотрудники с русским и английским поиском для формы телефона.</summary>
public sealed class PhoneEmployeeSelectDataSource(DbManager db) : IClaySelectDataSource<Employee, int?>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<Employee>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<Employee>(SQLQueries.SELECT_ТелефонСотрудникиДляВыбора,
            new { Search = request.SearchText, LimitPlusOne = request.Limit + 1 });
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<Employee>(SQLQueries.SELECT_СотрудникПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
