using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Locations;

/// <summary>Работающие свободные розетки офиса оборудования с включением текущей занятой розетки.</summary>
public sealed class NetSocketSelectDataSource(DbManager db) : IClaySelectDataSource<NetSocketLookup, int?>
{
    /// <summary>Текущая розетка, включаемая в список даже при занятости.</summary>
    public int? CurrentSocketId { get; set; }

    /// <summary>Офис последней записи расположения оборудования; NULL не подменяется ID порта.</summary>
    public int? OfficeLocationId { get; set; }

    /// <inheritdoc />
    public async Task<ClaySelectResult<NetSocketLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await db.QueryRequiredAsync<NetSocketLookup>(SQLQueries.SELECT_РозеткиДляСети, new
        {
            CurrentSocketId,
            OfficeLocationId,
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        }, cancellationToken)).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<NetSocketLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<NetSocketLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        var rows = await db.QueryRequiredAsync<NetSocketLookup>(
            SQLQueries.SELECT_РозеткаДляСетиПоИдентификатору, new { Id = id.Value }, cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
