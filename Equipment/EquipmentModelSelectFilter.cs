namespace Clayzor.Lib.Web.DBSelect.Equipment;

/// <summary>
/// Прикладные ограничения поиска моделей оборудования.
/// </summary>
public sealed class EquipmentModelSelectFilter
{
    /// <summary>
    /// Тип оборудования, модели которого доступны для выбора.
    /// Пустое значение запрещает выбор модели до выбора типа.
    /// </summary>
    public int? EquipmentTypeId { get; set; }
}
