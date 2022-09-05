using PowerServer.Core;
using SnapObjects.Data;
using SnapObjects.Data.Odbc;

namespace ServerAPIs
{
    [DataProvider(DatabaseType.SqlAnywhere, IsOdbc = true)]
    public class OdbcSqlAnywhereDataDriver : DataProvider
    {
        public override DataContext Create(string connectionString)
        {
            return new OdbcSqlAnywhereDataContext(new OdbcSqlAnywhereDataContextOptions(connectionString));
        }
    }
}
