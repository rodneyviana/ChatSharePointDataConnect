# SharePoint Data Connect AI Security Auditor

An AI-powered solution for analyzing SharePoint security, permissions, and site configurations using natural language queries. This project demonstrates how to combine Microsoft Graph Data Connect with AI (Ollama, OpenAI API, or Azure OpenAI) to create an intelligent security auditing tool.

![SecurityBot](SecurityBot.png)

## Overview

This solution consists of two .NET 8 projects that work together:

1. **SPGraphConnector** - Imports SharePoint data from Microsoft Graph Data Connect into SQL Server
2. **SecurityBot** - AI-powered chatbot that queries the database using natural language

**Use Cases:**
- Security auditing and compliance reporting
- Understanding SharePoint permissions and ownership
- Analyzing site storage and usage patterns
- Automated SharePoint administration queries
- Training and documentation for SharePoint governance

## Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (required - this is a .NET 8 project)
- SQL Server (any version) or LocalDB (included with Visual Studio)
- **One of the following AI providers:**
  - [Ollama](https://ollama.com) (recommended - free, local) **OR**
  - [Azure OpenAI](https://azure.microsoft.com/products/ai-services/openai-service) (cloud, paid) **OR**
  - [OpenAI API](https://platform.openai.com/) (cloud, paid)

### Installation Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/rodneyviana/ChatSharePointDataConnect.git
   cd SPGraphConnector
   ```

2. **Set up the database** (choose one method):

   **Option A: Restore from backup (RECOMMENDED - includes sample data)**

   Database is already populated with sample SharePoint data from Graph Data Connect

   ```bash
   # SQL Server backup (.bak) - 4.74 MB
   sqlcmd -S (localdb)\MSSQLLocalDB -Q "RESTORE DATABASE SPGraphConnector FROM DISK='SPGraphConnector.bak' WITH REPLACE"

   # OR Azure SQL package (.bacpac) - 0.05 MB
   SqlPackage.exe /Action:Import /sf:"SPGraphConnector-2024-7-29-17-10.bacpac" /tsn:"(localdb)\MSSQLLocalDB" /tdn:"SPGraphConnector"
   ```

   **After restore:** Database is ready! Skip to step 3 - **no need to run SPGraphConnector project**

   **Option B: Create database and import from included sample files**

   ```bash
   sqlcmd -S (localdb)\MSSQLLocalDB -i SPGraphConnector/CreateDatabase.sql
   cd SPGraphConnector
   dotnet run  # Imports sample .txt files into database
   ```

   **Option C: Use your own Graph Data Connect exports**

   ```bash
   sqlcmd -S (localdb)\MSSQLLocalDB -i SPGraphConnector/CreateDatabase.sql
   # Replace the .txt files in SPGraphConnector/ with your own exports
   cd SPGraphConnector
   dotnet run  # Imports your data into database
   ```

3. **Set up AI provider:**
   ```bash
   # Install and configure Ollama
   ollama pull llama3.2

   # OR configure OpenAI API / Azure OpenAI in .env file
   ```

4. **Configure SecurityBot:**
   ```bash
   cd securitybot
   cp .env.example .env
   # Edit .env with your AI provider settings
   ```

5. **Run the chatbot:**
   ```bash
   dotnet run
   ```

6. **Start asking questions:**
   ```
   User > Show me the top 3 sites by storage usage
   User > Who owns the engineering site?
   User > List all sites with Teams integration
   ```

## Project Structure

```
SPGraphConnector/
|-- SPGraphConnector/              # Project 1: Data Import Tool
|   |-- Program.cs                 # Main import logic
|   |-- SharePointSitesImporter.cs
|   |-- SharePointGroupsImporter.cs
|   |-- SharePointPermissionsImporter.cs
|   |-- CreateDatabase.sql         # Database schema
|   |-- BasicDataSet_v0.SharePointSites_v1.txt        # Sample data
|   |-- BasicDataSet_v0.SharePointGroups_v1.txt       # Sample data
|   +-- BasicDataSet_v0.SharePointPermissions_v1.txt  # Sample data
|
|-- securitybot/                   # Project 2: AI Chatbot
|   |-- Program.cs                 # Chat loop and AI integration
|   |-- SqlQueryPlugin.cs          # Function calling plugin
|   |-- tablesdefinition.txt       # AI system instructions
|   |-- .env.example               # Configuration template
|   |-- OLLAMA_FUNCTION_CALLING.md # Ollama setup guide
|   +-- README.md                  # Detailed project docs
|
|-- SPGraphConnector.bak           # SQL Server backup (4.74 MB)
|-- SPGraphConnector-2024-7-29-17-10.bacpac  # Azure SQL backup (0.05 MB)
|-- SecurityBot.png                # Screenshot
+-- README.md                      # This file
```

## Project 1: SPGraphConnector

**Purpose:** Import SharePoint data from Microsoft Graph Data Connect into SQL Server

**[!] When to use this project:**
- If you want to import your own Graph Data Connect exports
- If you created an empty database and want to use the included sample data
- **NOT needed if you restored from .bak or .bacpac** - those already contain processed data!

**What it does:**
- Reads line-delimited JSON files from Graph Data Connect exports
- Parses SharePoint sites, groups, and permissions data
- Imports into a normalized SQL Server database
- Handles transaction safety and error recovery

**Key Features:**
- Imports sites, owners, groups, members, and permissions
- Transaction-based processing for data integrity
- Handles complex nested JSON structures
- Easy-to-query database schema
- Sample data files included in repository

**Quick Start:**
```bash
cd SPGraphConnector
# Only run this if you need to import data
# NOT needed if you restored from backup files
dotnet run
```

**[Documentation ?](SPGraphConnector/README.md)**

## Project 2: SecurityBot

**Purpose:** AI-powered chatbot for querying SharePoint data using natural language

**[*] This is the main project you'll use!** After database setup, start here.

**What it does:**
- Accepts natural language questions about SharePoint
- Automatically generates SQL queries using AI
- Executes queries safely against the database
- Returns formatted, human-readable results

**Key Features:**
- Natural language processing
- Automatic SQL query generation
- Function calling with Semantic Kernel
- Works with Ollama (local/free), OpenAI API, or Azure OpenAI
- Conversational with context awareness
- Query validation and safety checks

**Example Questions:**
```
"Show me the largest sites"
"Which sites is john@contoso.com an owner of?"
"List all Teams-connected sites"
"What are the external sharing settings?"
"Show me sites with sensitivity labels"
```

**Quick Start:**
```bash
cd securitybot
cp .env.example .env
# Configure .env with Ollama, OpenAI API, or Azure OpenAI settings
dotnet run
```

**[Documentation ?](securitybot/README.md)**

## When to Use Each Project

### Use SecurityBot (Project 2) when:
- [x] You restored the database from .bak or .bacpac backup
- [x] You want to query SharePoint data with natural language
- [x] You're ready to test the AI chatbot

**--> Start here after restoring the database backup!**

### Use SPGraphConnector (Project 1) when:
- [x] You have your own Graph Data Connect exports to import
- [x] You created an empty database and want to use the included sample files
- [x] You want to refresh the database with new data

**--> Skip this if you restored from backup - data is already loaded!**

## Database Backups

Two backup files are included in the repository with **pre-processed sample data**:

### SPGraphConnector.bak (4.74 MB)
- **Type:** SQL Server native backup
- **Use for:** Any version of SQL Server, LocalDB, SQL Server Express
- **Contains:** Fully processed SharePoint data (no need to run SPGraphConnector)
- **Last updated:** July 29, 2024
- **Purpose:** **[RECOMMENDED]** Fastest way to get started - restore and immediately use SecurityBot!

**Restore instructions:**
```bash
# Using sqlcmd
sqlcmd -S (localdb)\MSSQLLocalDB -Q "RESTORE DATABASE SPGraphConnector FROM DISK='SPGraphConnector.bak' WITH REPLACE"

# Using PowerShell
Invoke-Sqlcmd -ServerInstance "(localdb)\MSSQLLocalDB" -Query "RESTORE DATABASE SPGraphConnector FROM DISK='SPGraphConnector.bak' WITH REPLACE"
```

**Using SQL Server Management Studio:**
1. Right-click **Databases** --> **Restore Database**
2. Select **Device** --> Browse to `SPGraphConnector.bak`
3. Click **OK** to restore

**[DONE]** After restore: Database is ready! Skip to SecurityBot configuration (no need to run SPGraphConnector)

### SPGraphConnector-2024-7-29-17-10.bacpac (0.05 MB)
- **Type:** Data-tier Application Package (portable)
- **Use for:** Azure SQL Database, any SQL Server version, cross-platform
- **Contains:** Same pre-processed data as .bak (no need to run SPGraphConnector)
- **Last updated:** July 29, 2024
- **Purpose:** Alternative format for cross-version compatibility and Azure SQL

**Import instructions:**
```bash
# Using SqlPackage.exe
SqlPackage.exe /Action:Import /sf:"SPGraphConnector-2024-7-29-17-10.bacpac" /tsn:"(localdb)\MSSQLLocalDB" /tdn:"SPGraphConnector"

# For Azure SQL Database
SqlPackage.exe /Action:Import /sf:"SPGraphConnector-2024-7-29-17-10.bacpac" /tsn:"yourserver.database.windows.net" /tdn:"SPGraphConnector" /tu:"username" /tp:"password"
```

**Using SQL Server Management Studio:**
1. Right-click **Databases** --> **Import Data-tier Application**
2. Browse to `SPGraphConnector-2024-7-29-17-10.bacpac`
3. Follow the wizard

**Using Azure Data Studio:**
1. Right-click server --> **Data-tier Application wizard**
2. Select **Import bacpac**
3. Follow the wizard

**[DONE]** After import: Database is ready! Skip to SecurityBot configuration (no need to run SPGraphConnector)

## Technology Stack

- **Framework:** .NET 8 (required)
- **AI SDK:** Microsoft Semantic Kernel 1.74.0
- **AI Providers:** Ollama (local/free), OpenAI API, or Azure OpenAI (both cloud/paid)
- **Database:** SQL Server (any version) / LocalDB / Azure SQL
- **Data Processing:** Newtonsoft.Json for Graph Data Connect parsing
- **Configuration:** DotNetEnv for environment management

## Configuration

**Note:** This project supports three AI providers - choose the one that best fits your needs:
- **Ollama** - Free, local, privacy-focused
- **OpenAI API** - Cloud-based, pay-per-use, latest models
- **Azure OpenAI** - Enterprise cloud, integration with Azure services

### Ollama Setup (Recommended - Free & Local)

1. Install Ollama from [ollama.com](https://ollama.com)
2. Pull a function-calling model:
   ```bash
   ollama pull llama3.2
   ```
3. Configure `securitybot/.env`:
   ```env
   MODELID="llama3.2"
   ENDPOINT="http://localhost:11434/v1"
   APIKEY="ollama"
   CONNECTIONSTRING="Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;"
   ```

### OpenAI API Setup

1. Get API key from [OpenAI Platform](https://platform.openai.com/)
2. Configure `securitybot/.env`:
   ```env
   MODELID="gpt-4o"
   ENDPOINT="https://api.openai.com/v1"
   APIKEY="your-openai-api-key"
   CONNECTIONSTRING="Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;"
   ```

### Azure OpenAI Setup

1. Create Azure OpenAI resource in Azure Portal
2. Deploy a model (GPT-4, GPT-4o recommended)
3. Configure `securitybot/.env`:
   ```env
   MODELID="your-deployment-name"
   ENDPOINT="https://your-resource.openai.azure.com/"
   APIKEY="your-api-key"
   CONNECTIONSTRING="Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;"
   ```

## Database Schema

The database includes the following main tables:

| Table | Purpose |
|-------|---------|
| **SharePointSites** | Site metadata, storage, Teams integration, security settings |
| **RootWeb** | Site titles and web template information |
| **Owner** | Site owners with Azure AD Object IDs |
| **SecondaryContact** | Additional site contacts |
| **SharePointGroups** | SharePoint security groups per site |
| **GroupMembers** | Group membership details |
| **SharePointPermissions** | File/folder level permissions |
| **SharedWith** | Detailed sharing information |

The schema is intentionally denormalized for query performance and simplicity.

## Security Considerations

**[!] Important Security Notes:**

- **SQL Injection:** The AI generates SQL queries. Review queries in production.
- **Read-Only Recommended:** Create a SQL user with SELECT-only permissions
- **Query Validation:** Enable `IsSafeQuery()` in `SqlQueryPlugin.cs` to block dangerous operations
- **Data Privacy:** 
  - Ollama keeps all data local (no cloud communication)
  - OpenAI API and Azure OpenAI send queries to the cloud
- **API Costs:** OpenAI API and Azure OpenAI charge per token; Ollama is free

## Learning Resources

### Microsoft Graph Data Connect
- [Overview](https://learn.microsoft.com/graph/data-connect-concept-overview)
- [Get Started](https://learn.microsoft.com/graph/data-connect-get-started)
- [SharePoint Dataset Schemas](https://github.com/microsoftgraph/dataconnect-solutions/tree/main/datasetschemas)

### AI & Semantic Kernel
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Function Calling Guide](https://learn.microsoft.com/semantic-kernel/concepts/ai-services/chat-completion/function-calling/)
- [Ollama Documentation](https://ollama.com/docs)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [Azure OpenAI Service](https://learn.microsoft.com/azure/ai-services/openai/)

## Contributing

This project is a proof of concept demonstrating:
- Microsoft Graph Data Connect integration
- AI-powered natural language SQL query generation
- Semantic Kernel function calling
- Local vs. cloud AI provider comparison

Feel free to fork and adapt for your needs!

## License

This project is provided as-is for educational and demonstration purposes.

## Acknowledgments

- Built with [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel)
- SharePoint schemas from [Microsoft Graph Data Connect](https://github.com/microsoftgraph/dataconnect-solutions)
- AI models: Meta's Llama, Alibaba's Qwen, and OpenAI's GPT

## Support

For issues and questions:
- Review individual project README files ([SPGraphConnector](SPGraphConnector/README.md), [securitybot](securitybot/README.md))
- Check [OLLAMA_FUNCTION_CALLING.md](securitybot/OLLAMA_FUNCTION_CALLING.md) for function calling issues
- Open an issue on GitHub

---

**Built with .NET 8, Semantic Kernel, and AI**
