# SPGraphConnector - SharePoint Data Import Tool

## Overview

SPGraphConnector is a .NET 8 console application that imports SharePoint data extracted from **Microsoft Graph Data Connect** into a SQL Server database for analysis and querying. This tool processes datasets containing SharePoint sites, groups, and permissions, making them easily queryable via SQL.

?? **When do you need this project?**
- ? You want to import your own Graph Data Connect exports
- ? You created an empty database and want to use the included sample data
- ? **NOT needed if you restored from .bak or .bacpac** - those backups already contain processed data!

## What It Does

This application reads line-delimited JSON files exported from Microsoft Graph Data Connect and imports them into a normalized SQL Server database with the following structure:

- **SharePointSites** - Site metadata, storage metrics, Teams integration, sensitivity labels
- **RootWeb** - Root web information including site titles
- **Owner** - Site owners
- **SecondaryContact** - Secondary site contacts
- **SharePointGroups** - SharePoint security groups
- **GroupMembers** - Group membership details
- **SharePointPermissions** - Permission details for files and folders
- **SharedWith** - Sharing information

## Prerequisites

- .NET 8 SDK (required)
- SQL Server (any version) or SQL Server LocalDB (included with Visual Studio)
- **Either:**
  - SharePoint dataset files from Microsoft Graph Data Connect (if importing your own data), OR
  - The included sample files (already in the repository for testing)

**Sample files included in repository:**
- `BasicDataSet_v0.SharePointSites_v1.txt`
- `BasicDataSet_v0.SharePointGroups_v1.txt`
- `BasicDataSet_v0.SharePointPermissions_v1.txt`

## Getting Started

### Important: Do You Need to Run This Project?

**Choose your scenario:**

**Scenario A: You restored from backup (.bak or .bacpac)**
- ? Database is ready with sample data
- ? **Do NOT run this project**
- ?? Go directly to securitybot project

**Scenario B: You created an empty database**
- ? Run this project to import the included sample data
- ?? Continue with steps below

**Scenario C: You have your own Graph Data Connect exports**
- ? Replace the .txt files with your own
- ? Run this project to import your data
- ?? Continue with steps below

### 1. Database Setup

#### Option A: Create Empty Database (For importing data)

Run the SQL script to create the database and schema:

```bash
sqlcmd -S (localdb)\MSSQLLocalDB -i CreateDatabase.sql
```

Or open `CreateDatabase.sql` in SQL Server Management Studio (SSMS) or Azure Data Studio and execute it.

#### Option B: Restore from SQL Server Backup (.bak)

?? **If you use this option, you do NOT need to run SPGraphConnector!**

The backup file contains pre-processed sample data (4.74 MB backup from July 29, 2024):

**Using SQL Server Management Studio:**
1. Right-click **Databases** ? **Restore Database**
2. Select **Device** ? Click **...** ? **Add**
3. Browse to `SPGraphConnector.bak` (in the repository root)
4. Click **OK**
5. In the **Options** page, you may need to modify the file paths if restoring to a different location
6. Click **OK** to restore

**Using sqlcmd (Command Line):**
```bash
sqlcmd -S (localdb)\MSSQLLocalDB -Q "RESTORE DATABASE SPGraphConnector FROM DISK='C:\path\to\repo\SPGraphConnector.bak' WITH REPLACE, MOVE 'SPGraphConnector' TO 'C:\path\to\data\SPGraphConnector.mdf', MOVE 'SPGraphConnector_log' TO 'C:\path\to\data\SPGraphConnector_log.ldf'"
```

**Using PowerShell:**
```powershell
Invoke-Sqlcmd -ServerInstance "(localdb)\MSSQLLocalDB" -Query "RESTORE DATABASE SPGraphConnector FROM DISK='C:\path\to\repo\SPGraphConnector.bak' WITH REPLACE"
```

? **After restore, skip to SecurityBot** - no need to run SPGraphConnector!

#### Option C: Import Azure SQL Database Package (.bacpac)

?? **If you use this option, you do NOT need to run SPGraphConnector!**

The .bacpac file contains the same pre-processed data as the .bak file (0.05 MB export):

**Using SQL Server Management Studio:**
1. Right-click **Databases** ? **Import Data-tier Application**
2. Click **Next** ? Browse to `SPGraphConnector-2024-7-29-17-10.bacpac` (in repository root)
3. Provide database name: `SPGraphConnector`
4. Click **Next** ? **Finish**

**Using SqlPackage.exe (Command Line):**
```bash
SqlPackage.exe /Action:Import /sf:"C:\path\to\repo\SPGraphConnector-2024-7-29-17-10.bacpac" /tsn:"(localdb)\MSSQLLocalDB" /tdn:"SPGraphConnector"
```

**Using Azure Data Studio:**
1. Right-click server ? **Data-tier Application wizard**
2. Select **Import bacpac**
3. Browse to the `.bacpac` file
4. Follow the wizard

**For Azure SQL Database:**
```bash
SqlPackage.exe /Action:Import /sf:"SPGraphConnector-2024-7-29-17-10.bacpac" /tsn:"yourserver.database.windows.net" /tdn:"SPGraphConnector" /tu:"username" /tp:"password"
```

? **After import, skip to SecurityBot** - no need to run SPGraphConnector!

### 2. Configure Connection String (if needed)

Open `Program.cs` and modify the connection string if needed:

```csharp
var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
```

**Common connection strings:**

- **LocalDB (default):**  
  `Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;`

- **SQL Server with Windows Authentication:**  
  `Data Source=SERVERNAME;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;`

- **SQL Server with SQL Authentication:**  
  `Data Source=SERVERNAME;Initial Catalog=SPGraphConnector;User Id=username;Password=password;Connect Timeout=30;Encrypt=False;`

- **Azure SQL Database:**  
  `Data Source=yourserver.database.windows.net;Initial Catalog=SPGraphConnector;User Id=username;Password=password;Encrypt=True;`

### 3. Prepare Your Data Files

**Option A: Use included sample data (for testing)**
- ? Sample files are already in the repository
- ? No action needed - proceed to step 4

**Option B: Use your own Graph Data Connect exports**
- Export data from **Microsoft Graph Data Connect** using these datasets:
  - `BasicDataSet_v0.SharePointSites_v1`
  - `BasicDataSet_v0.SharePointGroups_v1`
  - `BasicDataSet_v0.SharePointPermissions_v1`
- Replace the `.txt` files in the project directory with your exports

**Resources:**
- [Microsoft Graph Data Connect Overview](https://learn.microsoft.com/graph/data-connect-concept-overview)
- [SharePoint Dataset Schemas](https://github.com/microsoftgraph/dataconnect-solutions/tree/main/datasetschemas)
- [Get started with Microsoft Graph Data Connect](https://learn.microsoft.com/graph/data-connect-get-started)

### 4. Update File Paths (only if using different filenames)

If your data files have different names, update `Program.cs`:

```csharp
SharePointSitesImporter sharePointSitesImporter = new SharePointSitesImporter(connectionString);
sharePointSitesImporter.ImportData("BasicDataSet_v0.SharePointSites_v1.txt");

SharePointGroupsImporter sharePointGroupsImporter = new SharePointGroupsImporter(connectionString, "BasicDataSet_v0.SharePointGroups_v1.txt");
sharePointGroupsImporter.Import();

SharePointPermissionsImporter sharePointPermissionsImporter = new SharePointPermissionsImporter(connectionString);
sharePointPermissionsImporter.ImportFromFile("BasicDataSet_v0.SharePointPermissions_v1.txt");
```

### 5. Run the Application

?? **Remember:** Only run this if you created an empty database or want to import your own data!

```bash
cd SPGraphConnector
dotnet run
```

The application will:
1. Read each line-delimited JSON file
2. Parse the JSON data
3. Insert records into the appropriate tables
4. Display progress in the console

## Project Structure

```
SPGraphConnector/
??? Program.cs                           # Entry point with file paths
??? SharePointSitesImporter.cs          # Imports site data
??? SharePointGroupsImporter.cs         # Imports group data
??? SharePointPermissionsImporter.cs    # Imports permissions data
??? CreateDatabase.sql                   # Database schema creation script
??? SPGraphConnector.csproj             # Project configuration
??? [Data Files]                         # Place your .txt files here
    ??? BasicDataSet_v0.SharePointSites_v1.txt
    ??? BasicDataSet_v0.SharePointGroups_v1.txt
    ??? BasicDataSet_v0.SharePointPermissions_v1.txt
```

## Database Schema

The database is intentionally denormalized for simplicity and query performance. Key tables:

### Primary Tables

| Table | Description | Key Fields |
|-------|-------------|------------|
| **SharePointSites** | Site metadata, storage, security | Id (PK), Url, Title (via RootWeb), StorageUsed |
| **RootWeb** | Root web information | SiteId (FK), Title, WebTemplate |
| **Owner** | Site owners | SiteId (FK), AadObjectId, Email, Name |
| **SecondaryContact** | Secondary contacts | SiteId (FK), AadObjectId, Email, Name |
| **SharePointGroups** | Security groups | SiteId (FK), GroupId (PK), Title |
| **GroupMembers** | Group membership | SiteId (FK), GroupId (FK), MemberAadObjectId |
| **SharePointPermissions** | File/folder permissions | Id (PK), SiteId, ObjectUrl |
| **SharedWith** | Sharing details | SharePointPermissionId (FK), Type, Name |

## Dependencies

- **Newtonsoft.Json** (v13.0.4) - JSON parsing
- **System.Data.SqlClient** (v4.9.1) - SQL Server connectivity

## Important Notes

?? **Transaction Safety**: Each JSON line is processed in a transaction. If an error occurs, the transaction is rolled back for that record.

?? **Data Format**: The input files must be line-delimited JSON (NDJSON format), where each line is a complete JSON object.

?? **Database Must Exist**: The database schema must be created before running the import (use `CreateDatabase.sql` or restore from backup).

?? **Duplicate Prevention**: The application does not check for duplicates. If you re-run the import with the same data, you may encounter primary key violations.

?? **Backup Files**: Two database backup files are included in the repository root:
- `SPGraphConnector.bak` (4.74 MB) - SQL Server native backup format
- `SPGraphConnector-2024-7-29-17-10.bacpac` (0.05 MB) - Data-tier application package (portable)

## Troubleshooting

**Error: Cannot open database "SPGraphConnector"**
- Ensure the database has been created using one of the setup methods above

**Error: Could not find file 'BasicDataSet_v0.SharePointSites_v1.txt'**
- Verify the data files are in the same directory as the executable
- Check file names match exactly (case-sensitive)
- These files should be copied to the output directory (configured in `.csproj`)

**Error: A network-related or instance-specific error occurred**
- Verify SQL Server/LocalDB is running
- For LocalDB: Run `sqllocaldb start MSSQLLocalDB`
- Check connection string is correct

**Error: Login failed for user**
- Verify authentication credentials
- Ensure user has appropriate database permissions (db_owner recommended for import)

**Error: Restore operation failed**
- Check file paths in RESTORE command
- Ensure destination folder exists and has write permissions
- Verify SQL Server version compatibility

## Next Steps

After importing the data, use the **securitybot** project to query this database using natural language with AI assistance.

## References

- [Microsoft Graph Data Connect Documentation](https://learn.microsoft.com/graph/data-connect-concept-overview)
- [SharePoint Dataset Schemas](https://github.com/microsoftgraph/dataconnect-solutions/tree/main/datasetschemas)
- [.NET 8 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [SQL Server Backup and Restore](https://learn.microsoft.com/sql/relational-databases/backup-restore/back-up-and-restore-of-sql-server-databases)
- [Data-tier Applications (BACPAC)](https://learn.microsoft.com/sql/relational-databases/data-tier-applications/data-tier-applications)
