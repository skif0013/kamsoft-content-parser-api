using System.Text.Json;
using Kamsoft.ContentParser.Api.Domain.Interfaces;

namespace Kamsoft.ContentParser.Api.Services.Parsers;

public class InternalJsonContentParser : IContentParser
{
    public string SupportedType => "INTERNAL_JSON";

    public IReadOnlyList<Dictionary<string, string>> Parse(string rawText)
    {
        var items = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(
            rawText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (items is null || items.Count == 0)
            throw new FormatException("JSON array is empty or null.");

        var rows = new List<Dictionary<string, string>>();

        foreach (var item in items)
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in item)
            {
                dict[kvp.Key] = kvp.Value.ToString();
            }
            rows.Add(dict);
        }

        return rows;
    }
}
