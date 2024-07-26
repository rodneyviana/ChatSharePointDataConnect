using System;
using System.Data.SqlClient;
using System.IO;
using Newtonsoft.Json.Linq;

public class SharePointGroupsImporter
{
    private readonly string _connectionString;
    private readonly Stream _fileStream;

    public SharePointGroupsImporter(string connectionString, string filePath)
    {
        _connectionString = connectionString;
        _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }

    public SharePointGroupsImporter(string connectionString, Stream fileStream)
    {
        _connectionString = connectionString;
        _fileStream = fileStream;
    }

    public void Import()
    {
        using (StreamReader sr = new StreamReader(_fileStream))
        {
            string line;
            while ((line = sr.ReadLine()) != null)
            {

                JObject json;
                try
                {
                    json = JObject.Parse(line);
                } catch (Exception)
                {
                    Console.WriteLine("Error parsing line: " + line);
                    continue;
                }

                string ptenant = json["ptenant"]?.ToString() ?? Guid.Empty.ToString();
                string siteId = json["SiteId"]?.ToString() ?? Guid.Empty.ToString();
                int groupId = json["GroupId"]?.ToObject<int?>() ?? -1;
                string groupLinkId = json["GroupLinkId"]?.ToString() ?? Guid.Empty.ToString();
                string groupType = json["GroupType"]?.ToString() ?? string.Empty;
                string displayName = json["DisplayName"]?.ToString() ?? string.Empty;
                string ownerType = json["Owner"]?["Type"]?.ToString() ?? string.Empty;
                string ownerName = json["Owner"]?["Name"]?.ToString() ?? string.Empty;
                DateTime? snapshotDate = json["SnapshotDate"]?.ToObject<DateTime?>();

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Insert into SharePointGroups
                            string groupQuery = @"
                                INSERT INTO SharePointGroups (
                                    ptenant, SiteId, GroupId, GroupLinkId, GroupType, DisplayName, SnapshotDate
                                ) VALUES (
                                    @ptenant, @SiteId, @GroupId, @GroupLinkId, @GroupType, @DisplayName, @SnapshotDate
                                )";

                            using (SqlCommand cmd = new SqlCommand(groupQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ptenant", ptenant);
                                cmd.Parameters.AddWithValue("@SiteId", siteId);
                                cmd.Parameters.AddWithValue("@GroupId", groupId);
                                cmd.Parameters.AddWithValue("@GroupLinkId", groupLinkId);
                                cmd.Parameters.AddWithValue("@GroupType", groupType);
                                cmd.Parameters.AddWithValue("@DisplayName", displayName);
                                cmd.Parameters.AddWithValue("@SnapshotDate", (object)snapshotDate ?? DBNull.Value);

                                cmd.ExecuteNonQuery();
                            }

                            // Insert into GroupMembers
                            if (json != null && json["Members"] != null)
                            {
                                foreach (var member in json["Members"])
                                {
                                    string memberType = member["Type"]?.ToString() ?? string.Empty;
                                    string aadObjectId = member["AadObjectId"]?.ToString() ?? Guid.Empty.ToString();
                                    string memberName = member["Name"]?.ToString() ?? string.Empty;
                                    string email = member["Email"]?.ToString() ?? string.Empty;

                                    // check if the member is already in the database and if so skip
                                    string memberCheckQuery = @"
                                        SELECT COUNT(1) FROM GroupMembers
                                        WHERE SiteId = @SiteId AND GroupId = @GroupId AND MemberAadObjectId = @AadObjectId";
                                    using (SqlCommand cmd = new SqlCommand(memberCheckQuery, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@SiteId", siteId);
                                        cmd.Parameters.AddWithValue("@GroupId", groupId);
                                        cmd.Parameters.AddWithValue("@AadObjectId", aadObjectId);

                                        int count = (int)cmd.ExecuteScalar();
                                        if (count > 0)
                                        {
                                            continue;
                                        }
                                    }



                                    string memberQuery = @"
                                        INSERT INTO GroupMembers (
                                            SiteId, GroupId, MemberType, MemberAadObjectId, MemberName, MemberEmail
                                        ) VALUES (
                                            @SiteId, @GroupId, @MemberType, @AadObjectId, @MemberName, @Email
                                        )";

                                    using (SqlCommand cmd = new SqlCommand(memberQuery, conn, transaction))
                                    {
                                        cmd.Parameters.AddWithValue("@SiteId", siteId);
                                        cmd.Parameters.AddWithValue("@GroupId", groupId);
                                        cmd.Parameters.AddWithValue("@MemberType", memberType);
                                        cmd.Parameters.AddWithValue("@AadObjectId", aadObjectId);
                                        cmd.Parameters.AddWithValue("@MemberName", memberName);
                                        cmd.Parameters.AddWithValue("@Email", email);

                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
    }
}
