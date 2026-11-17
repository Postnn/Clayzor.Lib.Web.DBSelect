using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Documents;

/// <summary>Источник документов из отдельной базы документооборота.</summary>
public sealed class DocumentSelectDataSource : IClaySelectDataSource<DocumentLookup, int?>, IDisposable
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник и принимает владение переданным менеджером подключения.</summary>
    public DocumentSelectDataSource(DbManager db) => _db = db;

    /// <summary>Фильтр документов по связанному лицу.</summary>
    public int? PersonId { get; set; }

    /// <inheritdoc />
    public async Task<ClaySelectResult<DocumentLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryAsync<DocumentLookup>(SQLQueries.SELECT_ДокументыДляВыбора, new
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
        if (id is null or <= 0)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<DocumentLookup>(SQLQueries.SELECT_ДокументПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }

    /// <summary>Возвращает поставщика, если среди лиц документа кроме владельца есть ровно одно лицо.</summary>
    public async Task<int?> GetSingleShipperIdAsync(int documentId, int ownerId)
    {
        var rows = await _db.QueryAsync<int>(SQLQueries.SELECT_ПоставщикиДокумента, new
        {
            DocumentId = documentId,
            OwnerId = ownerId,
        });
        var persons = rows.Take(2).ToList();
        return persons.Count == 1 ? persons[0] : null;
    }

    /// <summary>Освобождает отдельное подключение к базе документов.</summary>
    public void Dispose() => _db.Dispose();
}
