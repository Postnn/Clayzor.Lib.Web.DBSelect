using System.Text;
using Clayzor.Lib.Web.Controls.Components.Select;
using Dapper;

namespace Clayzor.Lib.Web.DBSelect.Phones;

internal static class PhoneSelectQuery
{
    internal static void Exclude(StringBuilder sql, DynamicParameters parameters, string column,
        IReadOnlyCollection<int> ids, string prefix = "Excluded")
    {
        var index = 0;
        foreach (var id in ids.Distinct())
        {
            var name = $"@{prefix}{index++}";
            parameters.Add(name, id);
            sql.Append($" AND {column} <> {name}");
        }
    }

    internal static ClaySelectResult<T> Result<T>(IEnumerable<T> source, int limit) where T : class
    {
        var rows = source.Take(limit + 1).ToList();
        return new ClaySelectResult<T> { Items = rows.Take(limit).ToList(), HasMore = rows.Count > limit };
    }
}
