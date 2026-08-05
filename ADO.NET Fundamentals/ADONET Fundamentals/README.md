# ADO.NET Fundamentals — Store DAL

A practical implementation of a **Data Access Layer (DAL)** using raw **ADO.NET** with SQL Server.

---

## Projects

### `Store.DAL`
A class library that provides data access for a simple store domain.

**Domain Models:**
| Model | Fields |
|---|---|
| `Product` | `Id`, `Name`, `Description`, `Weight`, `Height`, `Width`, `Length` |
| `Order` | `Id`, `Status`, `CreatedDate`, `UpdatedDate`, `ProductId` |

**Order Statuses:** `NotStarted`, `Loading`, `InProgress`, `Arrived`, `Unloading`, `Cancelled`, `Done`

**Repositories:**
| Repository | Capabilities |
|---|---|
| `ProductRepository` | Full CRUD + fetch all products |
| `OrderRepository` | Full CRUD + filtered fetch + bulk delete |

**Order filtering** (via stored procedure) supports: month, year, status, productId — all optional and combinable.

**SQL Scripts** (`Store.DAL/Scripts/`):
- `CreateTables.sql` — creates `Product` and `Order` tables
- `StoredProcedures.sql` — creates `GetOrders` and `DeleteOrders` stored procedures

**Tech:** .NET 9 · ADO.NET · Microsoft.Data.SqlClient · SQL Server

---

### `Store.DAL.Tests`
Integration test project that verifies all DAL functionality against a real SQL Server **LocalDB** instance.

- A shared `DatabaseFixture` creates a fresh `StoreDalTests` database before the test session and drops it after
- Tables are cleared between each test to ensure isolation
- Uses `xUnit` with `[Collection]` to share a single DB instance across all test classes

**Test coverage:**
| Test Class | Tests |
|---|---|
| `ProductRepositoryTests` | Create, GetById (exists / not exists), GetAll, Update, Delete |
| `OrderRepositoryTests` | Create, GetById (exists / not exists), Update, Delete, GetOrders (no filter / by status / month / year / productId), DeleteBulk (by status / month / productId) |

**Tech:** .NET 9 · xUnit · FluentAssertions · Microsoft.Data.SqlClient · SQL Server LocalDB

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (included with Visual Studio)

## Running the Tests

```bash
dotnet test Store.DAL.Tests/Store.DAL.Tests.csproj
```

## Database Setup

To set up the schema manually on your own SQL Server instance, run the scripts in order:

```bash
# 1. Create tables
sqlcmd -S <server> -d <database> -i Store.DAL/Scripts/CreateTables.sql

# 2. Create stored procedures
sqlcmd -S <server> -d <database> -i Store.DAL/Scripts/StoredProcedures.sql
```
