using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Persons;

/// <summary>Источник лиц из отдельной базы справочников.</summary>
public sealed class PersonSelectDataSource : IClaySelectDataSource<PersonLookup, int?>, IDisposable
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник и принимает владение переданным менеджером подключения.</summary>
    public PersonSelectDataSource(DbManager db) => _db = db;

    /// <inheritdoc />
    public async Task<ClaySelectResult<PersonLookup>> SearchAsync(
        ClaySelectSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = (await _db.QueryStoredProcAsync<PersonLookup>(SQLQueries.SP_Лица_Поиск, new
        {
            Search = request.SearchText,
            PersonWhereSearch = 1,
            PersonHowSearch = 1,
            PersonSelectTop = request.Limit + 1,
        })).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return new ClaySelectResult<PersonLookup>
        {
            Items = rows.Take(request.Limit).ToList(),
            HasMore = rows.Count > request.Limit,
        };
    }

    /// <inheritdoc />
    public async Task<PersonLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null or <= 0)
            return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await _db.QueryAsync<PersonLookup>(SQLQueries.SELECT_ЛицоПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }

    /// <summary>Освобождает отдельное подключение к базе справочников.</summary>
    public void Dispose() => _db.Dispose();
}
