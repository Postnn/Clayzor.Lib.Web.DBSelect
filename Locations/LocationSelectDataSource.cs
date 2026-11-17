using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Locations;

/// <summary>Источник рабочих расположений оборудования.</summary>
public sealed class LocationSelectDataSource : IClaySelectDataSource<LocationLookup, int?>
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник с подключением текущего Blazor circuit.</summary>
    public LocationSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<LocationLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<LocationLookup>(SQLQueries.SELECT_РасположенияДляВыбора, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<LocationLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<LocationLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null or <= 0)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<LocationLookup>(SQLQueries.SELECT_РасположениеПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
