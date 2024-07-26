using System;
using System.Data;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json.Linq;

public class SharePointSitesImporter
{
    private readonly string _connectionString;
    private readonly SqlConnection _connection;

    public SharePointSitesImporter(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SharePointSitesImporter(SqlConnection connection)
    {
        _connection = connection;
    }

    public void ImportData(string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            ImportData(stream);
        }
    }

    public void ImportData(Stream fileStream)
    {
        using (var reader = new StreamReader(fileStream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var jsonObject = JObject.Parse(line);
                InsertData(jsonObject);
            }
        }
    }

    private void InsertData(JObject jsonObject)
    {
        using (var connection = _connection ?? new SqlConnection(_connectionString))
        {
            connection.Open();

            using (var transaction = connection.BeginTransaction())
            {
                try
                {
                    InsertSharePointSite(jsonObject, connection, transaction);
                    InsertRootWeb(jsonObject["RootWeb"] as JObject, (Guid)jsonObject["Id"], connection, transaction);
                    InsertOwner(jsonObject["Owner"] as JObject, (Guid)jsonObject["Id"], connection, transaction);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }

    private void InsertSharePointSite(JObject jsonObject, SqlConnection connection, SqlTransaction transaction)
    {
        var command = new SqlCommand(@"
            INSERT INTO SharePointSites (
                ptenant, Id, Url, WebCount, StorageQuota, StorageUsed, 
                StorageMetricsMetadataSize, StorageMetricsTotalFileCount, 
                StorageMetricsTotalFileStreamSize, StorageMetricsTotalSize, 
                GroupId, GeoLocation, IsInRecycleBin, IsTeamsConnectedSite, 
                IsTeamsChannelSite, TeamsChannelType, IsHubSite, HubSiteId, 
                BlockAccessFromUnmanagedDevices, BlockDownloadOfAllFilesOnUnmanagedDevices, 
                BlockDownloadOfViewableFilesOnUnmanagedDevices, ShareByEmailEnabled, 
                ShareByLinkEnabled, IBMode, ReadLocked, ReadOnly, CreatedTime, 
                LastSecurityModifiedDate, SnapshotDate
            ) VALUES (
                @ptenant, @Id, @Url, @WebCount, @StorageQuota, @StorageUsed, 
                @StorageMetricsMetadataSize, @StorageMetricsTotalFileCount, 
                @StorageMetricsTotalFileStreamSize, @StorageMetricsTotalSize, 
                @GroupId, @GeoLocation, @IsInRecycleBin, @IsTeamsConnectedSite, 
                @IsTeamsChannelSite, @TeamsChannelType, @IsHubSite, @HubSiteId, 
                @BlockAccessFromUnmanagedDevices, @BlockDownloadOfAllFilesOnUnmanagedDevices, 
                @BlockDownloadOfViewableFilesOnUnmanagedDevices, @ShareByEmailEnabled, 
                @ShareByLinkEnabled, @IBMode, @ReadLocked, @ReadOnly, @CreatedTime, 
                @LastSecurityModifiedDate, @SnapshotDate
            )", connection, transaction);

        command.Parameters.AddWithValue("@ptenant", (string)jsonObject["ptenant"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@Id", (string)jsonObject["Id"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@Url", (string)jsonObject["Url"] ?? string.Empty);
        command.Parameters.AddWithValue("@WebCount", (int?)jsonObject["WebCount"] ?? -1);
        command.Parameters.AddWithValue("@StorageQuota", (long?)jsonObject["StorageQuota"] ?? -1);
        command.Parameters.AddWithValue("@StorageUsed", (long?)jsonObject["StorageUsed"] ?? -1);
        command.Parameters.AddWithValue("@StorageMetricsMetadataSize", (long?)jsonObject["StorageMetrics"]?["MetadataSize"] ?? -1);
        command.Parameters.AddWithValue("@StorageMetricsTotalFileCount", (long?)jsonObject["StorageMetrics"]?["TotalFileCount"] ?? -1);
        command.Parameters.AddWithValue("@StorageMetricsTotalFileStreamSize", (long?)jsonObject["StorageMetrics"]?["TotalFileStreamSize"] ?? -1);
        command.Parameters.AddWithValue("@StorageMetricsTotalSize", (long?)jsonObject["StorageMetrics"]?["TotalSize"] ?? -1);
        command.Parameters.AddWithValue("@GroupId", (string)jsonObject["GroupId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@GeoLocation", (string)jsonObject["GeoLocation"] ?? string.Empty);
        command.Parameters.AddWithValue("@IsInRecycleBin", (bool?)jsonObject["IsInRecycleBin"] ?? false);
        command.Parameters.AddWithValue("@IsTeamsConnectedSite", (bool?)jsonObject["IsTeamsConnectedSite"] ?? false);
        command.Parameters.AddWithValue("@IsTeamsChannelSite", (bool?)jsonObject["IsTeamsChannelSite"] ?? false);
        command.Parameters.AddWithValue("@TeamsChannelType", (string)jsonObject["TeamsChannelType"] ?? string.Empty);
        command.Parameters.AddWithValue("@IsHubSite", (bool?)jsonObject["IsHubSite"] ?? false);
        command.Parameters.AddWithValue("@HubSiteId", (string)jsonObject["HubSiteId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@BlockAccessFromUnmanagedDevices", (bool?)jsonObject["BlockAccessFromUnmanagedDevices"] ?? false);
        command.Parameters.AddWithValue("@BlockDownloadOfAllFilesOnUnmanagedDevices", (bool?)jsonObject["BlockDownloadOfAllFilesOnUnmanagedDevices"] ?? false);
        command.Parameters.AddWithValue("@BlockDownloadOfViewableFilesOnUnmanagedDevices", (bool?)jsonObject["BlockDownloadOfViewableFilesOnUnmanagedDevices"] ?? false);
        command.Parameters.AddWithValue("@ShareByEmailEnabled", (bool?)jsonObject["ShareByEmailEnabled"] ?? false);
        command.Parameters.AddWithValue("@ShareByLinkEnabled", (bool?)jsonObject["ShareByLinkEnabled"] ?? false);
        command.Parameters.AddWithValue("@IBMode", (string)jsonObject["IBMode"] ?? string.Empty);
        command.Parameters.AddWithValue("@ReadLocked", (bool?)jsonObject["ReadLocked"] ?? false);
        command.Parameters.AddWithValue("@ReadOnly", (bool?)jsonObject["ReadOnly"] ?? false);
        command.Parameters.AddWithValue("@CreatedTime", (DateTime?)jsonObject["CreatedTime"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.Parameters.AddWithValue("@LastSecurityModifiedDate", (DateTime?)jsonObject["LastSecurityModifiedDate"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.Parameters.AddWithValue("@SnapshotDate", (DateTime?)jsonObject["SnapshotDate"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);

        command.ExecuteNonQuery();
    }

    private void InsertRootWeb(JObject jsonObject, Guid siteId, SqlConnection connection, SqlTransaction transaction)
    {
        if (jsonObject == null) return;

        var command = new SqlCommand(@"
            INSERT INTO RootWeb (
                SiteId, Id, Title, WebTemplate, WebTemplateId, LastItemModifiedDate
            ) VALUES (
                @SiteId, @Id, @Title, @WebTemplate, @WebTemplateId, @LastItemModifiedDate
            )", connection, transaction);

        command.Parameters.AddWithValue("@SiteId", siteId);
        command.Parameters.AddWithValue("@Id", (string)jsonObject["Id"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@Title", (string)jsonObject["Title"] ?? string.Empty);
        command.Parameters.AddWithValue("@WebTemplate", (string)jsonObject["WebTemplate"] ?? string.Empty);
        command.Parameters.AddWithValue("@WebTemplateId", (int?)jsonObject["WebTemplateId"] ?? -1);
        command.Parameters.AddWithValue("@LastItemModifiedDate", (DateTime?)jsonObject["LastItemModifiedDate"] ?? System.Data.SqlTypes.SqlDateTime.MinValue);
        command.ExecuteNonQuery();
    }

    private void InsertOwner(JObject jsonObject, Guid siteId, SqlConnection connection, SqlTransaction transaction)
    {
        if (jsonObject == null) return;

        var command = new SqlCommand(@"
            INSERT INTO Owner (
                SiteId, AadObjectId, Email, Name
            ) VALUES (
                @SiteId, @AadObjectId, @Email, @Name
            )", connection, transaction);

        command.Parameters.AddWithValue("@SiteId", siteId);
        command.Parameters.AddWithValue("@AadObjectId", (string)jsonObject["AadObjectId"] ?? Guid.Empty.ToString());
        command.Parameters.AddWithValue("@Email", (string)jsonObject["Email"] ?? string.Empty);
        command.Parameters.AddWithValue("@Name", (string)jsonObject["Name"] ?? string.Empty);

        command.ExecuteNonQuery();
    }
}
