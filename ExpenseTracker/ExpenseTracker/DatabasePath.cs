namespace ExpenseTracker
{
    public static class DatabasePath
    {
        private static string DbName => "ExpenseTracker.db";

        public static string GetPath()
        {
            string pathDbSqlite = string.Empty;

            if(DeviceInfo.Platform == DevicePlatform.Android)
            {
                pathDbSqlite = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                pathDbSqlite = Path.Combine(pathDbSqlite, DbName);
            }

            else if(DeviceInfo.Platform == DevicePlatform.iOS)
            {
                pathDbSqlite = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                pathDbSqlite = Path.Combine(pathDbSqlite, "..", "Library", DbName);
            }

            return pathDbSqlite;
        }
    }
}
