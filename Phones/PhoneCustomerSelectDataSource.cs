using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Лица-заказчики, а не произвольные лица справочника.</summary>
public sealed class PhoneCustomerSelectDataSource(DbManager db) : IClaySelectDataSource<PersonLookup, int?>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<PersonLookup>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PersonLookup>(SQLQueries.SELECT_ТелефонЗаказчикиДляВыбора,
            new { Search = request.SearchText, LimitPlusOne = request.Limit + 1 });
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<PersonLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PersonLookup>(SQLQueries.SELECT_ТелефонЗаказчикПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
