using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using Microsoft.SemanticKernel;
using DotNetEnv;

public class SqlQueryPlugin
{
    private readonly string _connectionString;

    public SqlQueryPlugin()
    {
        Env.Load();
        var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
        _connectionString = connectionString;
    }

    private DataTable ExecuteQuery(string query)
    {
        DataTable dataTable = new DataTable();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);

            connection.Open();
            adapter.Fill(dataTable);
            connection.Close();
        }

        return dataTable;
    }

    // Return true if the query is a SELECT statement and if it does not contain any SQL injection
    private bool IsSafeQuery(string query)
    {
        string[] unsafeKeywords = { "DROP", "DELETE", "UPDATE", "INSERT", "TRUNCATE", "ALTER", "CREATE" };
        string upperQuery = query.ToUpper();

        if (unsafeKeywords.Any(keyword => upperQuery.Contains(keyword)))
        {
            return false;
        }

        return true;
    }


    // Define a kernel function that runs a SQL query and returns the result as a JSON string
    [KernelFunction("run_sql_query")]
    [Description("Runs a SQL query and returns the result as a JSON string")]
    [return: Description("JSON string representing the query result")]
    public async Task<string> RunSqlQueryAsync(string query)
    {
        // Check if the query is safe
        //if (!IsSafeQuery(query))
        //{
        //    return "{\"error\": \"Unsafe query\"}";
        //}

        Console.WriteLine("\nRunning query: " + query);
        DataTable queryResult = ExecuteQuery(query);



        // Convert DataTable to JSON string
        string jsonResult = ConvertDataTableToJson(queryResult);
        return jsonResult;
    }

    private string ConvertDataTableToJson(DataTable dataTable)
    {
        var data = dataTable.Rows.OfType<DataRow>()
    .Select(row => dataTable.Columns.OfType<DataColumn>()
    .ToDictionary(col => col.ColumnName, col => row[col]));
        return JsonSerializer.Serialize(data);
        //return JsonSerializer.Serialize(dataTable);
    }
}
