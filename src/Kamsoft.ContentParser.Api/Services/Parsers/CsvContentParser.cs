using Kamsoft.ContentParser.Api.Domain.Interfaces;

namespace Kamsoft.ContentParser.Api.Services.Parsers;

public class CsvContentParser : IContentParser
{
    public string SupportedType => "CSV";

    public IReadOnlyList<Dictionary<string, string>> Parse(string rawText)
    {
        var lines = rawText.Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            throw new FormatException("CSV must contain a header row and at least one data row.");

        var headers = ParseCsvLine(lines[0]);
        var rows = new List<Dictionary<string, string>>();

        for (var i = 1; i < lines.Length; i++)
        {
            var values = ParseCsvLine(lines[i]);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var j = 0; j < headers.Count; j++)
            {
                dict[headers[j]] = j < values.Count ? values[j] : string.Empty;
            }

            rows.Add(dict);
        }

        return rows;
    }

    private static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (inQuotes)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    inQuotes = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                    inQuotes = true;
                else if (c == ',')
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                    current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result;
    }
}
