using Dapper;
using Microsoft.Data.SqlClient;
using WebApi.Models;

namespace WebApi.Infrastructure
{
    public class DataAccess : IDisposable
    {
        private SqlConnection connection;
        public DataAccess(IConfiguration configuration) 
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
                connection = null;
            }
        }

        public bool RegisterUsers(string email, string password, string role)
        {
            var accountCount = connection.ExecuteScalar<int>
            (
                "SELECT Count(1) FROM [UserAccount] WHERE [Email] = @email", new { email = email }
            );

            if (accountCount > 0) return false;

            var sql = "INSERT INTO [UserAccount] (Email, Password, Role) VALUES (@email, @password, @role)";
            var result = connection.Execute(sql, new {email = email, password = password, role = role});

            return result > 0;
        }

        public UserAccount? FindUserByEmail(string email)
        {
            var sql = "SELECT * FROM [UserAccount] WHERE [Email] = @email";
            return connection.QueryFirstOrDefault<UserAccount>(sql, new { email = email });
        }

        public bool InsertRefreshToken(RefreshToken refreshToken, string email)
        {
            var sql = "INSERT INTO [RefreshToken] (Token, CreatedDate, Expires, Enabled, Email) VALUES (@token, @createddate, @expires, @enabled, @email)";

            var result = connection.Execute(sql, new
            {
                refreshToken.Token,
                refreshToken.CreateDate,
                refreshToken.Expires,
                refreshToken.Enabled,
                email
            });

            return result > 0;
        }

        public bool DisabelUserTokenByEmail(string email)
        {
            var sql = "UPDATE [REFRESHTOKEN] SET [Enabled] = 0 WHERE [EMAIL] = @email";
            var result = connection.Execute(sql, new {email});
            return result > 0;
        }

        public bool DisabelUserToken(string token)
        {
            var sql = "UPDATE [REFRESHTOKEN] SET [Enabled] = 0 WHERE [TOKEN] = @token";
            var result = connection.Execute(sql, new { token });
            return result > 0;
        }

        public bool IsRefreshTokenValid(string token)
        {
            var sql = "SELECT COUNT(1) FROM RefreshToken  WHERE [TOKEN] = @token AND [Enabled] = 1 AND [Expires] >= CAST(GETDATE() AS DATE)";
            var result = connection.ExecuteScalar<int>(sql, new { token });
            return result > 0;
        }

        public UserAccount? FindUserByToken(string token)
        {
            var sql = "SELECT [UserAccount].*  FROM [RefreshToken] JOIN [UserAccount] ON [RefreshToken].Email = [UserAccount].Email WHERE [Token] = @token";
            return connection.QueryFirstOrDefault<UserAccount>(sql, new { token });
        }
    }
}
