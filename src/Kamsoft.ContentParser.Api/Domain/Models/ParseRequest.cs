using Kamsoft.ContentParser.Api.Domain.Enums;

namespace Kamsoft.ContentParser.Api.Domain.Models;

public sealed record ParseRequest
{
    public ContentType Type { get; init; }
    public string Content { get; init; } = string.Empty;
}
