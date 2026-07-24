using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DotNetApiStarterKit.Data.Database
{
    public class SystemDbContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SystemDbContext(IConfiguration configuration)
        {
            this._configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;

        }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(this._connectionString);
        }
    }
}
