using System.Data;
using Microsoft.Data.SqlClient;
using EduVision.Models;

namespace EduVision.Data
{
    public class AccountRepository
    {
        private readonly string _connectionString;

        public AccountRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool EmailExists(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Users WHERE Email = @Email", conn);

            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();
            int count = (int)cmd.ExecuteScalar();
            return count > 0;
        }

        public int InsertUser(ApplicationUser user)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                INSERT INTO Users (FullName, Email, PasswordHash, PasswordSalt)
                VALUES (@FullName, @Email, @PasswordHash, @PasswordSalt);
                SELECT SCOPE_IDENTITY();", conn);

            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.Add("@PasswordHash", SqlDbType.VarBinary, 256).Value = user.PasswordHash;
            cmd.Parameters.Add("@PasswordSalt", SqlDbType.VarBinary, 128).Value = user.PasswordSalt;

            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public ApplicationUser GetUserByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(@"
                SELECT TOP 1 Id, FullName, Email, PasswordHash, PasswordSalt
                FROM Users WHERE Email = @Email", conn);

            cmd.Parameters.AddWithValue("@Email", email);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new ApplicationUser
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PasswordHash = (byte[])reader["PasswordHash"],
                PasswordSalt = (byte[])reader["PasswordSalt"]
            };
        }
    }
}
