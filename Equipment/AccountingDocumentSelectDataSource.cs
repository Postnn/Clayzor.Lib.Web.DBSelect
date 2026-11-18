using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник документов поступления или списания бухгалтерского учёта.</summary>
public sealed class AccountingDocumentSelectDataSource : IClaySelectDataSource<DocumentLookup, int?>, IDisposable
{
    private readonly DbManager _db;
    private readonly bool _writeOff;

    /// <summary>Создаёт источник документов и принимает владение подключением.</summary>
    public AccountingDocumentSelectDataSource(DbManager db, bool writeOff)
    {
        _db = db;
        _writeOff = writeOff;
    }

    /// <summary>Бухгалтерия-владелец для фильтрации документов.</summary>
    public int? PersonId { get; set; }

    /// <inheritdoc />
    public async Task<ClaySelectResult<DocumentLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var sql = _writeOff ? SQLQueries.SELECT_ДокументыСписанияБухУчета : SQLQueries.SELECT_ДокументыПоступленияБухУчета;
        var rows = (await _db.QueryAsync<DocumentLookup>(sql, new
        {
            Search = request.SearchText,
            PersonId,
            LimitPlusOne = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<DocumentLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<DocumentLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<DocumentLookup>(
            SQLQueries.SELECT_ДокументБухУчетаПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }

    /// <summary>Освобождает отдельное подключение к документам.</summary>
    public void Dispose() => _db.Dispose();
}
