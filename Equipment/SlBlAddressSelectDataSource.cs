using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities.Inventory;
using Clayzor.Lib.Web.Controls.Components.Select;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Почтовый ящик со строковым ключом и подписью.</summary>
public sealed record SlBlAddress(string Address);

/// <summary>Почтовые адреса, доступные через штатное представление SlBl.</summary>
public sealed class SlBlAddressSelectDataSource(DbManager db) : IClaySelectDataSource<SlBlAddress, string>
{
    /// <inheritdoc />
    public async Task<ClaySelectResult<SlBlAddress>> SearchAsync(ClaySelectSearchRequest request, CancellationToken cancellationToken = default)
    {
        var rows = (await SlBlData.SearchAddressesAsync(db, request.SearchText, request.Limit + 1, cancellationToken)).ToArray();
        return new() { Items = rows.Take(request.Limit).Select(address => new SlBlAddress(address)).ToArray(), HasMore = rows.Length > request.Limit };
    }

    /// <inheritdoc />
    public async Task<SlBlAddress?> GetByIdAsync(string? id, CancellationToken cancellationToken = default) =>
        id is not null && await SlBlData.FindAddressAsync(db, id, cancellationToken) is { } found ? new(found) : null;
}
