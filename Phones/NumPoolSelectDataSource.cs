using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Справочники ёмкостей и использования номера ТфОП.</summary>
public sealed class NumPoolSelectDataSource(DbManager db, NumPoolKind kind) : IClaySelectDataSource<NumPoolOption, int?>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<NumPoolOption>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default) =>
        PhoneSelectQuery.Result(await NumPoolData.SearchAsync(db, kind, request.SearchText, null, request.Limit + 1, cancellationToken), request.Limit);

    /// <inheritdoc />
    public async Task<NumPoolOption?> GetByIdAsync(int? id, CancellationToken cancellationToken = default) => id is > 0
        ? (await NumPoolData.SearchAsync(db, kind, "", id, 1, cancellationToken)).SingleOrDefault() : null;
}
