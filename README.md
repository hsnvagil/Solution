# 🏡 Booking API – Available Homes Query System
This is a clean and modular .NET 9 Web API that returns homes fully available during a given date range. The application supports both in-memory and JSON-based data sources and follows the Clean Architecture principles.

## 🧱 Architecture Overview
The solution is divided into multiple layers:
```pgsql
Solution/
├── src/
│   ├── App.API             → Entry point (Minimal API), endpoints, middleware
│   ├── App.Application     → UseCases (CQRS), validators, mappers
│   ├── App.Core            → Entities, interfaces, shared models
│   └── App.Infrastructure  → Data providers (JSON, InMemory), logging
│
└── tests/
    └── App.Test            → Integration tests using WebApplicationFactory
```
### Key Technologies
* ✅ .NET 9
* ✅ Clean Architecture
* ✅ MediatR (CQRS Pattern)
* ✅ FluentValidation
* ✅ JSON + InMemory data source
* ✅ Integration Testing (XUnit + FluentAssertions)

## 🚀 How to Run the Application

### 1. Clone the repository

```bash
git clone https://github.com/hsnvagil/solution.git
cd solution
```

### 2. Build the project

```bash
dotnet build
```

### 3. Run the API

```bash
dotnet run --project src/APP.API
```

### 4. Access the API

```bash
GET http://localhost:5000/api/available-homes?startDate=2025-07-18&endDate=2025-07-20
```

## ✅ How to Test the Application

```bash
dotnet test tests/APP.Test
```

## 🔎 Filtering Logic
Endpoint: 
`/api/available-homes?startDate=yyyy-MM-dd&endDate=yyyy-MM-dd`

### Logic:
* User supplies a start and end date.
* Dates are validated using FluentValidation to ensure:
  * Proper format (`yyyy-MM-dd`)
  * `startData <= endDate`
* The `GetAvailableHomesQueryHandler` retrieves all homes from the configured provider (either In-Memory or JSON).
* It then constructs the full date range between the provided dates.
* Each home is included in the result only if it is available on every single date in that range.
```csharp
var filtered = homes
    .Where(kvp => dateRange.All(d => kvp.Value.Contains(d)))
    .ToList();
```

## 🗂️ Data Providers
The application supports two data providers:
1. InMemoryHomeDataProvider (default for testing/demo):
   * Hardcoded home data stored in memory on app startup.
   * Sample data:
     ```csharp
     new Home { Id = 123, Name = "Home 1" } → [2025-07-18, 2025-07-19, 2025-07-20]
     ```
2. JsonHomeDataProvider:
   * Reads data from `homes.json` file.
   * Filters out past dates to keep availability fresh.
You can switch between providers in the `App.Infrastructure/ConfigureServices.cs` file.

## 🛡️ Request & Response Logging
The API implements centralized logging via middleware:
* Captures request method, path, body, and timestamp
* Captures response body and status code
* Logs are written using `RequestResponseLogger` in `App.Infrastructure.Logger`

Pipeline setup in `Program.cs`:
```csharp
app.UseMiddleware<RequestResponseLoggerMiddleware>();
```
This provides full transparency for debugging and monitoring API behavior.


## 📂 Sample Request
```h
GET /api/available-homes?startDate=2025-07-18&endDate=2025-07-20
```
Response: 
```json
[
  {
    "homeId": 123,
    "homeName": "Home 1",
    "availableSlots": [
      "2025-07-18",
      "2025-07-19",
      "2025-07-20"
    ]
  }
]
```

## 🧪 Validation Rules
Implemented via FluentValidation:
* `startDate` and `endDate` must not be empty.
* Format must be `yyyy-MM-dd`.
* `startDate` must be earlier than or equal to `endDate`.
* `startDate` must not be earlier than today's date (past dates are not allowed).
