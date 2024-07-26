// See https://aka.ms/new-console-template for more information
var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=SPGraphConnector;Integrated Security=True;Connect Timeout=30;Encrypt=False;";
SharePointSitesImporter sharePointSitesImporter = new SharePointSitesImporter(connectionString);
sharePointSitesImporter.ImportData("BasicDataSet_v0.SharePointSites_v1.txt");
SharePointGroupsImporter sharePointGroupsImporter = new SharePointGroupsImporter(connectionString, "BasicDataSet_v0.SharePointGroups_v1.txt");
sharePointGroupsImporter.Import();
SharePointPermissionsImporter sharePointPermissionsImporter = new SharePointPermissionsImporter(connectionString);
sharePointPermissionsImporter.ImportFromFile("BasicDataSet_v0.SharePointPermissions_v1.txt");
