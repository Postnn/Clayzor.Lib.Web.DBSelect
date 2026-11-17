using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник ресурсов из отдельной базы справочников.</summary>
public sealed class ResourceSelectDataSource : IClaySelectDataSource<ResourceLookup, int?>, IDisposable
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник и принимает владение переданным менеджером подключения.</summary>
    public ResourceSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<ResourceLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<ResourceLookup>(SQLQueries.SELECT_РесурсыДляВыбора, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<ResourceLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<ResourceLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null or <= 0)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<ResourceLookup>(SQLQueries.SELECT_РесурсПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }

    /// <summary>Освобождает отдельное подключение к базе ресурсов.</summary>
    public void Dispose() => _db.Dispose();
}
