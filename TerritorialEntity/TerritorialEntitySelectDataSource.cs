using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities.TerritorialEntity;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.TerritorialEntity;

/// <summary>Lookup территорий для телефонных кодов с ограничением по типам 2, 3, 4.</summary>
public sealed class TerritorialEntitySelectDataSource(DbManager db) : IClaySelectDataSource<TerritorialEntityLookup, int?>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<TerritorialEntityLookup>> SearchAsync(
        ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        var rows = (await TerritorialEntityData.SearchPhoneTerritoriesAsync(db, request.SearchText,
            request.Limit + 1, cancellationToken)).ToList();
        return new ClaySelectResult<TerritorialEntityLookup>
        {
            Items = rows.Take(request.Limit).ToList(), HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public Task<TerritorialEntityLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default) =>
        id is > 0 ? TerritorialEntityData.FindTerritoryAsync(db, id.Value, cancellationToken)
            : Task.FromResult<TerritorialEntityLookup?>(null);
}
