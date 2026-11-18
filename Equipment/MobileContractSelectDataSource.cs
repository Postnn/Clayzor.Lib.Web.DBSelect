using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник незавершённых договоров мобильной связи.</summary>
public sealed class MobileContractSelectDataSource : IClaySelectDataSource<MobileContractLookup, int?>
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник с менеджером текущего circuit.</summary>
    public MobileContractSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<MobileContractLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<MobileContractLookup>(SQLQueries.SELECT_ДоговорыМобильнойСвязи, new
        {
            Search = request.SearchText,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<MobileContractLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<MobileContractLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<MobileContractLookup>(
            SQLQueries.SELECT_ДоговорМобильнойСвязиПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
