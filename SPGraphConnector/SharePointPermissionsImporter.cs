using System;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json.Linq;

public class SharePointPermissionsImporter
{
    private readonly string _connectionString;
    private readonly SqlConnection _connection;

    public SharePointPermissionsImporter(string connectionString)
    {
        _connectionString = connectionString;
        _connection = new SqlConnection(_connectionString);
    }

    public SharePointPermissionsImporter(SqlConnection connection)
    {
        _connection = connection;
    }

    public void ImportFromFile(string filePath)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            ImportFromStream(fileStream);
        }
    }

    public void ImportFromStream(Stream fileStream)
    {
        using (var reader = new StreamReader(fileStream))
        {
            string line;
            _connection.Open();
            while ((line = reader.ReadLine()) != null)
            {
                var jsonObject = JObject.Parse(line);
                var id = InsertSharePointPermission(jsonObject);
                InsertSharedWith(id, jsonObject);
            }
            _connection.Close();
        }
    }

    private Guid InsertSharePointPermission(JObject jsonObject)
    {
        var command = new SqlCommand(@"
            INSERT INTO SharePointPermission (
                Id, ptenant, SiteId, WebId, ItemType, ItemURL, FileExtension, RoleDefinition, 
                LinkId, ScopeId, LinkScope, SnapshotDate, ShareCreatedBy, ShareCreatedTime, 
                ShareLastModifiedBy, ShareLastModifiedTime, ShareExpirationTime, Operation
            ) VALUES (
                @Id, @ptenant, @SiteId, @WebId, @ItemType, @ItemURL, @FileExtension, @RoleDefinition, 
                @LinkId, @ScopeId, @LinkScope, @SnapshotDate, @ShareCreatedBy, @ShareCreatedTime, 
                @ShareLastModifiedBy, @ShareLastModifiedTime, @ShareExpirationTime, @Operation
            )", _connection);
        var guid = Guid.NewGuid();
        command.Parameters.AddWithValue("@Id", guid);
        command.Parameters.AddWithValue("@ptenant", (string)jsonObject["ptenant"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@SiteId", (string)jsonObject["SiteId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@WebId", (string)jsonObject["WebId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@ItemType", (string)jsonObject["ItemType"] ?? string.Empty);
        command.Parameters.AddWithValue("@ItemURL", (string)jsonObject["ItemURL"] ?? string.Empty);
        command.Parameters.AddWithValue("@FileExtension", (string)jsonObject["FileExtension"] ?? string.Empty);
        command.Parameters.AddWithValue("@RoleDefinition", (string)jsonObject["RoleDefinition"] ?? string.Empty);
        command.Parameters.AddWithValue("@LinkId", (string)jsonObject["LinkId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@ScopeId", (string)jsonObject["ScopeId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@LinkScope", (string)jsonObject["LinkScope"] ?? string.Empty);
        command.Parameters.AddWithValue("@SnapshotDate", (DateTime?)jsonObject["SnapshotDate"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.Parameters.AddWithValue("@ShareCreatedBy", (string)jsonObject["ShareCreatedBy"] ?? string.Empty);
        command.Parameters.AddWithValue("@ShareCreatedTime", (DateTime?)jsonObject["ShareCreatedTime"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.Parameters.AddWithValue("@ShareLastModifiedBy", (string)jsonObject["ShareLastModifiedBy"] ?? string.Empty);
        command.Parameters.AddWithValue("@ShareLastModifiedTime", (DateTime?)jsonObject["ShareLastModifiedTime"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.Parameters.AddWithValue("@ShareExpirationTime", (DateTime?)jsonObject["ShareExpirationTime"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.Parameters.AddWithValue("@Operation", (string?)jsonObject["Operation"] ?? string.Empty);

        command.ExecuteNonQuery();
        return guid;
    }

    private void InsertSharedWith(Guid PermissionId, JObject jsonObject)
    {
        var sharedWithArray = jsonObject["SharedWith"] as JArray;
        if (sharedWithArray == null) return;

        foreach (var item in sharedWithArray)
        {
            var command = new SqlCommand(@"
                INSERT INTO SharedWith (
                    Id, SharePointPermissionId, Type, Name, EmailAddress
                ) VALUES (
                   @Id, @SharePointPermissionId, @Type, @Name, @EmailAddress
                )", _connection);

            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@SharePointPermissionId", PermissionId);
            command.Parameters.AddWithValue("@Type", (string?)item["Type"] ?? string.Empty);
            command.Parameters.AddWithValue("@Name", (string?)item["Name"] ?? string.Empty);
            command.Parameters.AddWithValue("@EmailAddress", (string?)item["EmailAddress"] ?? string.Empty);

            command.ExecuteNonQuery();
        }
    }
}
