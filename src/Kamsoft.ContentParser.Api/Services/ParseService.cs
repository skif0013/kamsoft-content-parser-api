using System.Text;
using Kamsoft.ContentParser.Api.Domain;
using Kamsoft.ContentParser.Api.Domain.Interfaces;
using Kamsoft.ContentParser.Api.Domain.Models;

namespace Kamsoft.ContentParser.Api.Services;

public class ParseService
{
    private readonly IContentParserStrategyFactory _parserFactory;

    public ParseService(IContentParserStrategyFactory parserFactory)
    {
        _parserFactory = parserFactory;
    }

    public Result<ParseResponse> Parse(ParseRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return Result<ParseResponse>.Failure("The 'content' field is required.");

        var parserResult = _parserFactory.GetParser(request.Type);
        if (!parserResult.IsSuccess)
            return Result<ParseResponse>.Failure(parserResult.Error!);

        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.Content));
        }
        catch (FormatException)
        {
            return Result<ParseResponse>.Failure("Invalid Base64 data.");
        }

        IReadOnlyList<Dictionary<string, string>> rows;
        try
        {
            rows = parserResult.Value!.Parse(decoded);
        }
        catch (FormatException ex)
        {
            return Result<ParseResponse>.Failure(ex.Message);
        }

        return Result<ParseResponse>.Success(new ParseResponse
        {
            Success = true,
            RowCount = rows.Count,
            Data = rows
        });
    }
}
