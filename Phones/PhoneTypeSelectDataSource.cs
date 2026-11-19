using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Типы телефонных номеров с фильтром по станции и восстановлением несовместимого ID.</summary>
public sealed class PhoneTypeSelectDataSource(DbManager db) : IClaySelectDataSource<PhoneTypeLookup, int?>
{
    /// <summary>Выбранная станция; null показывает все типы.</summary>
    public int? StationId { get; set; }

    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneTypeLookup>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneTypeLookup>(SQLQueries.SELECT_ТелефонТипыДляВыбора,
            new { StationId, Search = request.SearchText, LimitPlusOne = request.Limit + 1 });
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<PhoneTypeLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneTypeLookup>(SQLQueries.SELECT_ТелефонТипПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
