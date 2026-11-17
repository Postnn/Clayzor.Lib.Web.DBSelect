using Clayzor.Lib.DALC;
using Clayzor.Lib.Entities;
using Clayzor.Lib.Entities.Inventory;

namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>Источник типов памяти для фиксированного списка формы модели.</summary>
public sealed class MemoryTypeSelectDataSource
{
    private readonly DbManager _db;

    /// <summary>Создаёт источник с менеджером подключения текущего circuit.</summary>
    public MemoryTypeSelectDataSource(DbManager db) => _db = db;

    /// <summary>Загружает типы памяти в алфавитном порядке.</summary>
    public async Task<IReadOnlyList<MemoryType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var items = (await _db.QueryAsync<MemoryType>(SQLQueries.SELECT_ТипыПамяти)).ToList();
        cancellationToken.ThrowIfCancellationRequested();
        return items;
    }
}
