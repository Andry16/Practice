using FtpServerApactheAggregator;
using MySql.Data.MySqlClient;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using static ModulesApactheAggregator.MD;

namespace DataBaseApactheAggregator
{
    public class DB
    {
        private MySqlConnection connection;
        private string serverdb;
        private string database;
        private string username;
        private string password;

        public DB()
        {
            serverdb = "server4.hosting.reg.ru";
            database = "u2096277_ApacheAggrigator";
            username = "u2096277_Admin";
            password = "m_aDmiN_N7__AdM1n_Dantes12315_oR2xU7xaW1zN1_";

            string connectionString = $"SERVER={serverdb};DATABASE={database};UID={username};PASSWORD={password}";
            connection = new MySqlConnection(connectionString);
        }

        public bool CheckConnection()
        {
            try
            {
                OpenConnection();

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Проверка подключение к базе данных... Успешно!");

                CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка 0x10 подключения к базе данных: {ex.Message}!");
                return false;
            }
        }

        public bool OpenConnection()
        {
            string connectionString = $"SERVER={serverdb};DATABASE={database};UID={username};PASSWORD={password}";

            connection = new MySqlConnection(connectionString);

            int maxRetryCount = 3;
            int currentRetry = 0;

            while (currentRetry < maxRetryCount)
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x10 подключения к базе данных: {ex.Message}!");
                    currentRetry++;
                }
            }

            return false;
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void DeleteUser(string login)
        {
            OpenConnection();

            string query = "DELETE FROM Data_Users WHERE Login = @login";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"\nПользователь с логином '{login}' успешно удален из таблицы Data_Users.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"\nПользователь с логином '{login}' не найден в таблице Data_Users.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x12 при удалении пользователя из таблицы Data_Users: {ex.Message}!");
                }
            }

            CloseConnection();
        }

        public void GetAllAdminUsers()
        {
            OpenConnection();

            string query = "SELECT Login, Password FROM Data_Users WHERE Admin_IS = 'Yes' AND Status_Admin = 'Valid'";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nСписок администраторов:\n");

                    while (reader.Read())
                    {
                        string login = reader.GetString(0);
                        string password = reader.GetString(1);
                        string encryptedPassword = EncryptPassword(password);

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Логин: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{login}, ");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Пароль: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{EncryptPassword(password)}\n");
                    }
                }
            }

            CloseConnection();
        }

        public void GetNonAdminUsers()
        {
            OpenConnection();

            string query = "SELECT Login, Password FROM Data_Users WHERE Admin_IS = 'No' AND Status_Admin = 'Not Valid'";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nСписок пользователей:\n");

                    while (reader.Read())
                    {
                        string login = reader.GetString(0);
                        string password = reader.GetString(1);
                        string encryptedPassword = EncryptPassword(password);

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Логин: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{login}, ");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Пароль: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{EncryptPassword(password)}\n");
                    }
                }
            }

            CloseConnection();
        }

        private string EncryptPassword(string password)
        {
            return new string('*', password.Length);
        }

        public void AddAdmin(string adminLogin, string newAdminLogin, string newAdminStatus)
        {
            OpenConnection();

            if (!IsAdmin(adminLogin))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nТекущий пользователь не является администратором! Добавление администратора запрещено!");

                CloseConnection();
                return;
            }

            if (newAdminStatus != "Valid" && newAdminStatus != "Not Valid")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nУказанное состояние администратора недействительно! Добавление администратора запрещено!");

                CloseConnection();
                return;
            }

            OpenConnection();

            string adminISValue = newAdminStatus == "Valid" ? "Yes" : "No";

            string query = "UPDATE Data_Users SET Admin_IS = @adminIS, Status_Admin = @status WHERE Login = @login";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@adminIS", adminISValue);
                command.Parameters.AddWithValue("@status", newAdminStatus);
                command.Parameters.AddWithValue("@login", newAdminLogin);

                try
                {
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("\nАдминистратор успешно добавлен или обновлен!");

                        CloseConnection();
                        return;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nОшибка 0x19 при обновлении или добавлении администратора!");

                        CloseConnection();
                        return;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x14 при выполнении запроса: {ex.Message}");

                    CloseConnection();
                    return;
                }
            }
        }

        public bool IsAdmin(string login)
        {
            OpenConnection();

            string query = "SELECT Admin_IS, Status_Admin FROM Data_Users WHERE Login = @login";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows && reader.Read())
                    {
                        string adminStatus = reader.GetString("Admin_IS").Trim();
                        string status = reader.GetString("Status_Admin").Trim();

                        CloseConnection();

                        if (adminStatus.Equals("Yes", StringComparison.OrdinalIgnoreCase) && status.Equals("Valid", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.WriteLine("\nВы вошли в аккаунт администратора!");

                            return true;
                        }
                        else
                        {
                            Console.WriteLine("\nВы вошли в аккаунт пользователя!");

                            return false;
                        }
                    }
                    else
                    {
                        CloseConnection();
                        return false;
                    }
                }
            }
        }

        public void InsertInformation(string internetProtocol, DateTime dataTime, string loginUser)
        {
            OpenConnection();

            string query = "INSERT INTO Insert_In_System (Internet_Protocol, Data_Time, Login_User) VALUES (@internetProtocol, @dataTime, @loginUser)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@internetProtocol", internetProtocol);
                command.Parameters.AddWithValue("@dataTime", dataTime);
                command.Parameters.AddWithValue("@loginUser", loginUser);
                command.ExecuteNonQuery();
            }

            CloseConnection();
        }

        public void GetAllRegistrationKeys()
        {
            OpenConnection();

            string query = "SELECT `Keys`, Who_Added FROM Registrations_Keys";

            using (var command = new MySqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\nСписок ключей регистрации:\n");

                    while (reader.Read())
                    {
                        string key = reader.GetString("Keys");
                        string addedBy = reader.GetString("Who_Added");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Ключ: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{key}, ");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Добавлено: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{addedBy}\n");
                    }
                }
            }

            CloseConnection();
        }

        public void DeleteRegistrationKey()
        {
            OpenConnection();

            GetAllRegistrationKeys();

            Console.Write("\nВведите ключ который нужно удалить: ");

            string key = Console.ReadLine();

            string query = "DELETE FROM Registrations_Keys WHERE `Keys` = @key";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@key", key);

                try
                {
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine($"\nКлюч регистрации '{key}' успешно удален из базы данных.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"\nОшибка 0x15: Не удалось найти ключ регистрации '{key}' в базе данных.");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x14 при удалении ключа регистрации из базы данных: {ex.Message}!");
                }
            }

            CloseConnection();
        }


        public void AddRegistrationKey()
        {
            OpenConnection();

            Console.ForegroundColor= ConsoleColor.DarkGreen;
            Console.WriteLine("\nУчтите, что новые регистрационные ключи будут работать только если их создал главный Администратор: Assering, или же они были добавлены системой!");

            string registrationKey = string.Empty;
            string login = string.Empty;

            while (string.IsNullOrEmpty(registrationKey) || string.IsNullOrEmpty(login))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите новый регистрационный ключ: ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;

                registrationKey = Console.ReadLine();

                if (!IsValidRegistrationKeyFormat(registrationKey))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x18: Введен некорректный формат регистрационного ключа. Попробуйте снова.");
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите ваш логин: ");
                Console.ForegroundColor = ConsoleColor.DarkCyan;

                login = Console.ReadLine();

                if (login != "Assering")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nВы не главный Администратор!");
                    return;
                }
            }

            string query = "INSERT INTO Registrations_Keys (`Keys`, Who_Added) VALUES (@registrationKey, @login)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@registrationKey", registrationKey);
                command.Parameters.AddWithValue("@login", login);

                try
                {
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("\nРегистрационный ключ успешно добавлен в базу данных!");

                        CloseConnection();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nОшибка 0x18 при добавлении регистрационного ключа в базу данных!");

                        CloseConnection();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x14 при выполнении запроса: {ex.Message}");

                    CloseConnection();
                }
            }

            CloseConnection();
        }

        private bool IsValidRegistrationKeyFormat(string registrationKey)
        {
            return Regex.IsMatch(registrationKey, @"^[a-zA-Z0-9_-]{4}-[a-zA-Z0-9_-]{4}-[a-zA-Z0-9_-]{4}-[a-zA-Z0-9_-]{4}$");
        }

        private bool IsValidRegistrationKey(string registrationKey)
        {
            if (registrationKey.Length != 19 || registrationKey[4] != '-' || registrationKey[9] != '-' || registrationKey[14] != '-')
                return false;

            return IsValidRegistrationKeyFormat(registrationKey);
        }

        private bool IsRegistrationKeyValid(string registrationKey)
        {
            OpenConnection();

            string query = "SELECT COUNT(*) FROM Registrations_Keys WHERE `Keys` = @registrationKey";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@registrationKey", registrationKey);

                try
                {
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    CloseConnection();
                    return count > 0;
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x14 при выполнении запроса: {ex.Message}");

                    CloseConnection();
                    return false;
                }
            }
        }

        private string GetWhoAdded(string registrationKey)
        {
            OpenConnection();

            string query = "SELECT Who_Added FROM Registrations_Keys WHERE `Keys` = @registrationKey";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@registrationKey", registrationKey);

                try
                {
                    object result = command.ExecuteScalar();

                    CloseConnection();
                    return result != null ? result.ToString() : "";
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x14 при выполнении запроса: {ex.Message}");

                    CloseConnection();
                    return "";
                }
            }
        }

        public bool RegisterUser()
        {
            OpenConnection();

            bool isValidKey = false;
            bool isValidFormat = false;
            bool isValidLogin = false;
            bool isValidPassword = false;
            string login = "";
            string password = "";
            string registrationKey = "";

            while (!isValidFormat || !isValidKey)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите ключ регистрации: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                registrationKey = Console.ReadLine();

                if (!IsValidRegistrationKeyFormat(registrationKey))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nНеправильный формат регистрационного ключа! Попробуйте заново!");
                    continue;
                }

                isValidFormat = true;

                if (!IsRegistrationKeyValid(registrationKey))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nНедействительный регистрационный ключ! Доступ к регистрации запрещен!\n\nЧтобы выйти нажмите любую клавишу!");
                    Console.ReadKey();

                    Environment.Exit(0);
                }
            }

            string whoAdded = GetWhoAdded(registrationKey);

            if (whoAdded != "Assering" && whoAdded != "RootAdmin")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nРегистрационный ключ выдан недопустимым источником! Доступ к регистрации запрещен!");

                CloseConnection();
                return false;
            }

            while (!isValidLogin || !isValidPassword)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите новый Логин: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                login = Console.ReadLine();

                if (login.Length > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x3 логин должен содержать не более 100 символов!");
                    continue;
                }

                isValidLogin = true;

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите новый Пароль: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                password = Console.ReadLine();

                if (password.Length > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x3 пароль должен содержать не более 100 символов!");
                    continue;
                }

                isValidPassword = true;
            }

            OpenConnection();

            string query = "INSERT INTO Data_Users (Login, Password, Admin_IS, Status_Admin) VALUES (@login, @password, @adminIS, @statusAdmin)";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@adminIS", "No");
                command.Parameters.AddWithValue("@statusAdmin", "Not Valid");

                try
                {
                    int affectedRows = command.ExecuteNonQuery();

                    if (affectedRows > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("\nРегистрация прошла успешно, данные сохранены в базе данных!");

                        return true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nОшибка 0x15 при регистрации, данные не были сохранены!");

                        return false;
                    }
                }
                catch (MySqlException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x14 при выполнении запроса: {ex.Message}");

                    return false;
                }
            }

            CloseConnection();
        }

        public bool AuthenticateUser(out string login)
        {
            bool isValidLogin = false;
            bool isValidPassword = false;
            login = "";
            string password = "";

            while (!isValidLogin || !isValidPassword)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите ваш Логин: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                string inputLogin = Console.ReadLine();

                if (inputLogin.Length > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x3 логин должен содержать не более 100 символов!");
                    continue;
                }

                login = inputLogin;
                isValidLogin = true;

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nВведите ваш Пароль: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                string inputPassword = Console.ReadLine();

                if (inputPassword.Length > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x3 пароль должен содержать не более 100 символов!");
                    continue;
                }

                password = inputPassword;
                isValidPassword = true;
            }

            OpenConnection();

            string query = "SELECT COUNT(*) FROM Data_Users WHERE Login = @login AND Password = @password";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@login", login);
                command.Parameters.AddWithValue("@password", password);

                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count > 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nАутентификация прошла успешно. Доступ разрешен...");
                    Console.Write("\nДобро пожаловать, ");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write($"{login}!");

                    string localIP = GetLocalIPAddress();

                    InsertInformation(localIP, DateTime.Now, login);

                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x13 аутентификации неверный логин или пароль. Доступ запрещен...");

                    Environment.Exit(0);
                    return false;
                }
            }

            CloseConnection();
        }

        public DataTable GetDataFromDatabase()
        {
            OpenConnection();

            DataTable dataTable = new DataTable();

            string query = "SELECT Internet_Protocol, Data_Time, Login_User FROM Insert_In_System";

            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                try
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dataTable.Load(reader);
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("\nПолученные данные из базы данных:\n");

                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n");

                            foreach (DataRow row in dataTable.Rows)
                            {
                                string ip = row["Internet_Protocol"].ToString();
                                DateTime dateTime = (DateTime)row["Data_Time"];
                                string login = row["Login_User"].ToString();

                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("IP: ");

                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write($"{ip}, ");

                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("DateTime: ");

                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write($"{dateTime}, ");

                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.Write("Login: ");

                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write($"{login}\n");
                            }

                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("База данных не содержит данные.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка 0x11 при получении данных из базы данных: {ex.Message}!");
                }
            }

            CloseConnection();
            return dataTable;
        }

        public void SaveLogsToDatabase()
        {
            OpenConnection();

            FTPServer ftpServer = new FTPServer();
            LogAggregator logAggregator = new LogAggregator(ftpServer.localFilePathOne);

            var logEntries = logAggregator.ReadLogs();

            foreach (var entry in logEntries)
            {
                if (entry.Url.Length <= 100 && entry.Method.Length <= 100)
                {
                    var query = "INSERT INTO Apache_Logs (Internet_Protocol, Date_Time, Method, Url) VALUES (@ip, @dateTime, @method, @url)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ip", entry.IP);
                        command.Parameters.AddWithValue("@dateTime", entry.DateTime);
                        command.Parameters.AddWithValue("@method", entry.Method);
                        command.Parameters.AddWithValue("@url", entry.Url);

                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x3: длина столбцов Url и Method должна быть не больше 100 символов!");
                }
            }

            CloseConnection();
        }

        public void RetrieveLogsFromDatabase(string ipAddress)
        {
            OpenConnection();

            var query = "SELECT Internet_Protocol, Date_Time, Method, Url FROM Apache_Logs WHERE Internet_Protocol = @IpAddress";

            using (var command = new MySqlCommand(query))
            {
                command.Parameters.AddWithValue("@IpAddress", ipAddress);

                using (var reader = command.ExecuteReader())
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n");

                    while (reader.Read())
                    {
                        string ip = reader.GetString("Internet_Protocol");
                        DateTime date_time = reader.GetDateTime("Date_Time");
                        string method = reader.GetString("Method");
                        string url = reader.GetString("Url");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("IP: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{ip}, ");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("DateTime: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{date_time}, ");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("Method: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{method}, ");

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("URL: ");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write($"{url}\n");
                    }

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n");
                }
            }

            CloseConnection();
        }

        public void RetrieveLogsFromDatabase()
        {
            OpenConnection();

            var query = "SELECT Internet_Protocol, Date_Time, Method, Url FROM Apache_Logs";

            using (var command = new MySqlCommand(query))
            {
                command.Connection = connection;

                using (var reader = command.ExecuteReader())
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n");

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string ip = reader.GetString("Internet_Protocol");
                            DateTime date_time = reader.GetDateTime("Date_Time");
                            string method = reader.GetString("Method");
                            string url = reader.GetString("Url");

                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("IP: ");

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write($"{ip}, ");

                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("DateTime: ");

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write($"{date_time}, ");

                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("Method: ");

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write($"{method}, ");

                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write("URL: ");

                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.Write($"{url}\n");
                        }
                    }

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("\n////////////////////////////////////////////////////////////////////////////////////////////////////////////////////\n");
                }
            }

            CloseConnection();
        }

        public string GetLocalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка 0x2 при получении локального IP-адреса: {ex.Message}!");
            }

            return localIP;
        }
    }
}