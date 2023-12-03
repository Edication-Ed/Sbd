using Npgsql;
using System.Security.Cryptography;

namespace Food_Delivery.Models
{
    public static class constants
    {
        public const string cookie_loggeduser_id = "cookie_loggeduser_keycode-first";
        public const string cookie_loggeduser_passcode = "cookie_loggeduser_keycode-second";
        public enum statuses
        {
            admin = 3,
            user = 1,
            delivery_man = 2
        }
        public static String[] default_controller = { "Home", "User", "Cur" };

        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));

                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static async Task<Userlogin?> getUser(String username, String hashed_pass)
        {
            Userlogin user = new();

            var con_str = Program.builder.Configuration.GetConnectionString("DefoultConnection");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(con_str);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand($"select * from userlogin where username = '{username}' and passcode = '{hashed_pass}'", conn))
            await using (var read = await cmd.ExecuteReaderAsync())
            {
                bool red = await read.ReadAsync();
                if (red && read.HasRows)
                {
                    user.Id = read.GetInt32(0);
                    user.Username = read.GetString(1);
                    user.Passcode = read.GetString(2);
                    user.Status = read.GetInt32(3);
                    user.Additionalid = read.GetInt32(4);
                }
                else
                {
                    await dataSource.DisposeAsync();
                    return null;
                }
            }
            await dataSource.DisposeAsync();
            return user;
        }

        public static async Task<Userlogin?> getUserById(String id, String hashed_pass) {
            Userlogin user = new();

            var con_str = Program.builder.Configuration.GetConnectionString("DefoultConnection");

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(con_str);
            var dataSource = dataSourceBuilder.Build();

            var conn = await dataSource.OpenConnectionAsync();

            await using (var cmd = new NpgsqlCommand($"select * from userlogin where id = {id} and passcode = '{hashed_pass}'", conn))
            await using (var read = await cmd.ExecuteReaderAsync())
            {
                bool red = await read.ReadAsync();
                if (red && read.HasRows)
                {
                    user.Id = read.GetInt32(0);
                    user.Username = read.GetString(1);
                    user.Passcode = read.GetString(2);
                    user.Status = read.GetInt32(3);
                    user.Additionalid = read.GetInt32(4);
                } else {
                   await dataSource.DisposeAsync();
                    return null;
                }
            }
            await dataSource.DisposeAsync();
            return user;
        }
    }
}
