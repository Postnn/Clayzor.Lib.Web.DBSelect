using System.Text;
using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;
using Dapper;

namespace Clayzor.Lib.Web.DBSelect.Phones;

/// <summary>Свободные работающие розетки с учётом расположения станции или чернового оборудования.</summary>
public sealed class PhoneSocketSelectDataSource(DbManager db) : IClaySelectDataSource<PhoneSocketLookup, int?>
{
    /// <summary>Выбранная станция.</summary>
    public int? StationId { get; set; }

    /// <summary>Текущая розетка, включаемая даже при занятости.</summary>
    public int? SocketId { get; set; }

    /// <summary>В черновике выбрано оборудование.</summary>
    public bool HasDraftEquipment { get; set; }

    /// <summary>Расположения оборудования из черновика формы.</summary>
    public IReadOnlyCollection<int> EquipmentLocationIds { get; set; } = [];

    /// <inheritdoc />
    public async Task<ClaySelectResult<PhoneSocketLookup>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (StationId is null) return new ClaySelectResult<PhoneSocketLookup>();
        var parameters = new DynamicParameters();
        parameters.Add("@SocketId", SocketId);
        parameters.Add("@Search", request.SearchText);
        parameters.Add("@LimitPlusOne", request.Limit + 1);
        StringBuilder sql;
        var locationIds = EquipmentLocationIds.Where(id => id > 0).Distinct().ToList();
        if (locationIds.Count > 0)
        {
            sql = new StringBuilder(SQLQueries.SELECT_ТелефонРозеткиОборудования);
            var index = 0;
            var names = locationIds.Select(id =>
            {
                var name = $"@LocationId{index++}";
                parameters.Add(name, id);
                return name;
            });
            sql.Append(" AND S.КодРасположения IN (").Append(string.Join(",", names)).Append(')');
        }
        else
        {
            if (HasDraftEquipment) return new ClaySelectResult<PhoneSocketLookup>();
            var officeId = await db.ExecuteScalarAsync<int?>(SQLQueries.SELECT_ТелефонОфисСтанции,
                new { StationId = StationId.Value }, commandType: System.Data.CommandType.Text);
            cancellationToken.ThrowIfCancellationRequested();
            if (officeId is null) return new ClaySelectResult<PhoneSocketLookup>();
            parameters.Add("@OfficeLocationId", officeId.Value);
            sql = new StringBuilder(SQLQueries.SELECT_ТелефонРозеткиВОфисе);
        }
        sql.Append(" ORDER BY Название");
        var rows = await db.QueryAsync<PhoneSocketLookup>(sql.ToString(), parameters);
        cancellationToken.ThrowIfCancellationRequested();
        return PhoneSelectQuery.Result(rows, request.Limit);
    }

    /// <inheritdoc />
    public async Task<PhoneSocketLookup?> GetByIdAsync(int? id, CancellationToken cancellationToken = default)
    {
        if (id is null) return null;
        cancellationToken.ThrowIfCancellationRequested();
        var rows = await db.QueryAsync<PhoneSocketLookup>(SQLQueries.SELECT_ТелефонРозеткаПоИдентификатору, new { Id = id.Value });
        cancellationToken.ThrowIfCancellationRequested();
        return rows.FirstOrDefault();
    }
}
