namespace Kamsoft.ContentParser.Api.Domain.Interfaces;

public interface IContentParser
{
    string SupportedType { get; }
    IReadOnlyList<Dictionary<string, string>> Parse(string rawText);
}
