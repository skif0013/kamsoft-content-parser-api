namespace Kamsoft.ContentParser.Api.Domain.Models;

public sealed record ParseResponse
{
    public bool Success { get; init; }
    public int RowCount { get; init; }
    public IReadOnlyList<Dictionary<string, string>> Data { get; init; } = [];
}
