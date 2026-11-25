using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Inventory;

/// <summary>Поиск дежурных инженеров со статистикой календарного месяца.</summary>
public sealed class DutyEngineerDataSource(DbManager db, DateOnly month) : IClaySelectDataSource<DutyEngineer, int?>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<DutyEngineer>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        var limit = Math.Clamp(request.Limit, 1, 100);
        var items = await DutyData.FindEngineersAsync(db, month, request.SearchText, null, limit + 1, cancellationToken);
        return new ClaySelectResult<DutyEngineer> { Items = items.Take(limit).ToList(), HasMore = items.Count > limit };
    }

    /// <inheritdoc />
    public async Task<DutyEngineer?> GetByIdAsync(int? id, CancellationToken cancellationToken = default) =>
        id is > 0 ? (await DutyData.FindEngineersAsync(db, month, null, id, 1, cancellationToken)).SingleOrDefault() : null;
}

