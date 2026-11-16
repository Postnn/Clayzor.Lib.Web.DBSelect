using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Employees;

/// <summary>
/// Источник сотрудников с поиском по русскому и английскому имени.
/// </summary>
public sealed class EmployeeSelectDataSource : IClaySelectDataSource<Employee, int?>
{
    private readonly DbManager _db;

    /// <summary>
    /// Создаёт источник с менеджером подключения текущего circuit.
    /// </summary>
    public EmployeeSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<Employee>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<Employee>(SQLQueries.SELECT_СотрудникиДляВыбора, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();

        return new ClaySelectResult<Employee>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<Employee>(SQLQueries.SELECT_СотрудникПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
