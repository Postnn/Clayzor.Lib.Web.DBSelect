using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Станции без обратного фильтра по типу, с сортировкой legacy Sort.</summary>
public sealed class PhoneStationSelectDataSource(DbManager db) : IClaySelectDataSource<PhoneStationLookup, int?>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneStationLookup>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneStationLookup>(SQLQueries.SELECT_ТелефонСтанцииДляВыбора,
            new { Search = request.SearchText, LimitPlusOne = request.Limit + 1 });
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<PhoneStationLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneStationLookup>(SQLQueries.SELECT_ТелефонСтанцияПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
