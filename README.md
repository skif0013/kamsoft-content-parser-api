# Content Parser API

Generic data parser for ASP.NET Core Web API. Supports CSV and INTERNAL_JSON formats with Base64-encoded payloads.

## Requirements

- .NET 9 SDK or newer

## Installation

### Restore dependencies

```bash
dotnet restore
```

### Run the application

```bash
dotnet run --project src/Kamsoft.ContentParser.Api
```

The API will be available at: `http://localhost:5000`

### Swagger UI

Once running, open: http://localhost:5000/swagger (or the HTTPS port shown in console)

### Run tests

```bash
dotnet test
```

Expected result: **32 tests passed** ✅

## Usage

### Endpoint

**POST** `/api/v1/parse-content`

**Header:** `Content-Type: application/json`

### Example 1: INTERNAL_JSON

**Request:**
```json
{
  "type": "INTERNAL_JSON",
  "content": "W3siaWQiOjEsInByb2R1Y3QiOiJMYXB0b3AiLCJwcmljZSI6MTIwMH0seyJpZCI6MiwicHJvZHVjdCI6Ik1vdXNlIiwicHJpY2UiOjUwfV0="
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "rowCount": 2,
  "data": [
    {
      "id": "1",
      "product": "Laptop",
      "price": "1200"
    },
    {
      "id": "2",
      "product": "Mouse",
      "price": "50"
    }
  ]
}
```

### Example 2: CSV

**Request:**
```json
{
  "type": "CSV",
  "content": "TmFtZSxBZ2UKSm9obiwyMApKYW5lLDI1"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "rowCount": 2,
  "data": [
    {
      "Name": "John",
      "Age": "20"
    },
    {
      "Name": "Jane",
      "Age": "25"
    }
  ]
}
```

## Supported Formats

### CSV (Comma-Separated Values)
- Requires header row + at least one data row
- Supports quoted values with escaped quotes
- Example: `Name,Age\nJohn,30\nJane,25`

### INTERNAL_JSON
- Array of objects with string key-value pairs
- Example: `[{"name":"John","age":"30"}]`

## Technical Details

### Payload Format

```json
{
  "type": "CSV" | "INTERNAL_JSON",
  "content": "<Base64-encoded data>"
}
```

- **type** - Content format (enum: CSV or INTERNAL_JSON)
- **content** - Raw data encoded in Base64

### Response Format

```json
{
  "success": true,
  "rowCount": <number>,
  "data": [
    { "<key>": "<value>", ... },
    ...
  ]
}
```

### Error Handling

Returns `400 Bad Request` with error message:
- "The 'content' field is required." - Missing content
- "Invalid Base64 data." - Malformed Base64
- "Unsupported type: X. Supported types: CSV, INTERNAL_JSON" - Unknown type
- "CSV must contain a header row and at least one data row." - Invalid CSV structure
- "Invalid JSON format: ..." - Malformed JSON

## Project Structure

```
src/Kamsoft.ContentParser.Api/
├── Controllers/
│   └── ParseController.cs              # HTTP endpoint
├── Domain/
│   ├── Enums/
│   │   └── ContentType.cs              # CSV, INTERNAL_JSON enum
│   ├── Interfaces/
│   │   ├── IContentParser.cs
│   │   └── IContentParserStrategyFactory.cs
│   ├── Models/
│   │   ├── ParseRequest.cs
│   │   └── ParseResponse.cs
│   └── Result.cs                       # Result<T> pattern
├── Services/
│   ├── ContentParserStrategyFactory.cs # Parser factory
│   ├── ParseService.cs                 # Business logic
│   └── Parsers/
│       ├── CsvContentParser.cs
│       └── InternalJsonContentParser.cs
└── Program.cs                          # DI setup
```

## Implementation Details

### Processing Steps

1. **Validate** - Check if content is provided
2. **Get Parser** - Retrieve appropriate parser based on type
3. **Decode** - Convert Base64 to text
4. **Parse** - Format-specific parsing (CSV or JSON)
5. **Return** - Unified response format

### Design Patterns

- **Strategy Pattern** - Parser implementations
- **Factory Pattern** - Parser selection
- **Result Pattern** - Functional error handling
- **Dependency Injection** - Service composition

## Testing

The project includes 32 comprehensive tests:
- HTTP integration tests (8)
- Parser unit tests (16)
- Factory tests (2)
- Service tests (6)

Run with verbose output:
```bash
dotnet test --verbosity detailed
```

## Security Considerations

- Input validation on all endpoints
- Proper error messages without exposing internals
- Base64 validation before processing
- JSON deserialization with safe options
- Type-safe enums for supported formats

## Build and Publish

### Debug Build
```bash
dotnet build
```

### Release Build
```bash
dotnet build -c Release
```

### Publish
```bash
dotnet publish -c Release -o ./publish
```

---
