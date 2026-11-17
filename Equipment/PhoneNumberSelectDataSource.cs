using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник телефонных номеров с серверным поиском и сведениями АТС.</summary>
public sealed class PhoneNumberSelectDataSource : IClaySelectDataSource<PhoneNumberLookup, int?>
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник с подключением текущего circuit.</summary>
    public PhoneNumberSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneNumberLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<PhoneNumberLookup>(SQLQueries.SELECT_ТелефонныеНомераДляВыбора, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<PhoneNumberLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<PhoneNumberLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<PhoneNumberLookup>(
            SQLQueries.SELECT_ТелефонныйНомерПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
