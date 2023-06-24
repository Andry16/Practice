using DataBaseApactheAggregator;
using FtpServerApactheAggregator;
using Newtonsoft.Json;
using System.Globalization;
using System.ServiceProcess;
using System.Text.RegularExpressions;

namespace ModulesApactheAggregator
{
    public class MD
    {
        public void metodForFTPServer()
        {
            FTPServer ftpServer = new FTPServer();
            string logFilePath = ftpServer.localFilePathOne;
        }

        public string questionTwo = "";

        public void ChooseAction()
        {
            DB db = new DB();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\nВы хотите авторизоваться или войти в свой аккаунт?\n\nВыберите одну из цифр и нажмите клавишу ввода...\n");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("1. ");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("Регистрация\n");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("2. ");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("Авторизация\n\n");

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write("Ваш выбор: ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                string choice = Console.ReadLine();
                string login;
                int numericChoice;

                if (!int.TryParse(choice, out numericChoice))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nНекорректный выбор! Введите число.\n");
                    continue;
                }

                switch (numericChoice)
                {
                    case 1:
                        db.RegisterUser();

                        ChooseAction();

                        return;

                    case 2:

                        bool isAuthenticated = db.AuthenticateUser(out login);
                        if (isAuthenticated)
                        {
                            bool isAdmin = db.IsAdmin(login);
                            if (isAdmin)
                            {
                                CommandsForAdmins();
                            }
                            else
                            {
                                CommandsForUsers();
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nОшибка 0x20 не удалось авторизоваться!\n");
                        }
                        return;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nНекорректный выбор!\n");
                        break;
                }
            }
        }

        public void CommandsForAdmins()
        {
            FTPServer ftpServer = new FTPServer();
            DB db = new DB();

            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nЧтобы ознакомиться со списком команд пропишите /Help");
                Console.Write("\nУкажите нужную вам команду: ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;

                string TypeCommands = Console.ReadLine() ?? string.Empty;
                TypeCommands = Convert.ToString(TypeCommands);

                if (TypeCommands == "/Help" || TypeCommands == "/help")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nСписок доступных вам команд:");

                    string txtFilePath = ftpServer.localFilePathThree;
                    string line1, line2, line3, line4, line5, line6, line7, line8, line9, line10, line11, line12, line13, line14, line15, line16;

                    ReadLinesFromFile(txtFilePath, out line1, out line2, out line3, out line4, out line5, out line6, out line7, out line8, out line9, out line10, out line11, out line12, out line13, out line14, out line15, out line16);

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"\n{line1}\n{line2}\n{line3}{line4}\n{line5}\n{line6}\n{line7}\n{line8}\n{line9}\n{line10}\n{line11}\n{line12}\n{line13}\n{line14}\n{line15}\n{line16}");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                }
                else if (TypeCommands == "/GetData" || TypeCommands == "/getdata")
                {
                    QuestionUser();
                    db.GetDataFromDatabase();
                }
                else if (TypeCommands == "/GetAPI" || TypeCommands == "/getapi")
                {
                    QuestionUser();
                    GetApi(ftpServer.localFilePathOne, ftpServer.localFilePathFour, ftpServer.localFilePathFive);
                }
                else if (TypeCommands == "/Exit" || TypeCommands == "/exit")
                {
                    exitProgram = true;
                }
                else if (TypeCommands == "/ReadLogs" || TypeCommands == "/readlogs")
                {
                    ApacheLogsReaderFromDataBase();
                }
                else if (TypeCommands == "/SaveLogs" || TypeCommands == "/savelogs")
                {
                    ApacheSaveLogsToDatabase();
                }
                else if (TypeCommands == "/UpdateOrAddAdmin" || TypeCommands == "/updateoraddadmin")
                {
                    QuestionUser();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("\nВведите ваш логин: ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;

                    var adminLogin = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("\nВведите логин пользователя: ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;

                    var newAdminLogin = Console.ReadLine();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("\nВведите новый статус для Админа (Valid или Not Valid): ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;

                    var adminStatus = Console.ReadLine();

                    db.AddAdmin(adminLogin, newAdminLogin, adminStatus);
                }
                else if (TypeCommands == "/CheckAdmins" || TypeCommands == "/checkadmins")
                {
                    QuestionUser();
                    db.GetAllAdminUsers();
                }
                else if (TypeCommands == "/CheckUsers" || TypeCommands == "/checkusers")
                {
                    QuestionUser();
                    db.GetNonAdminUsers();
                }
                else if (TypeCommands == "/DeleteUsers" || TypeCommands == "/deleteusers")
                {
                    var loginDelete = "";

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("\nВведите логин пользователя которого нужно удалить из БД: ");
                    Console.ForegroundColor = ConsoleColor.DarkCyan;

                    loginDelete = Convert.ToString(Console.ReadLine());

                    QuestionUser();
                    db.DeleteUser(loginDelete);
                }
                else if (TypeCommands == "/CreateNewRegKey" || TypeCommands == "/createnewregkey")
                {
                    QuestionUser();
                    db.AddRegistrationKey();
                }
                else if (TypeCommands == "/CheckRegKeys" || TypeCommands == "/checkregkeys")
                {
                    QuestionUser();
                    db.GetAllRegistrationKeys();
                }
                else if (TypeCommands == "/DeletRegKeys" || TypeCommands == "/deleteregkeys")
                {
                    QuestionUser();
                    db.DeleteRegistrationKey();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x5 неизвестная команда или указано число!\nПопробуйте ещё раз!");
                }
            }
        }

        public void CommandsForUsers()
        {
            FTPServer ftpServer = new FTPServer();
            DB db = new DB();

            bool exitProgram = false;

            while (!exitProgram)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nЧтобы ознакомиться со списком команд пропишите /Help");
                Console.Write("\nУкажите нужную вам команду: ");
                Console.ForegroundColor = ConsoleColor.DarkGreen;

                string TypeCommands = Console.ReadLine() ?? string.Empty;
                TypeCommands = Convert.ToString(TypeCommands);

                if (TypeCommands == "/Help" || TypeCommands == "/help")
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\nОбратите внимание!!! Ваш аккаунт не имеет Административных прав, поэтому многий функционал недоступен!");

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write("\nЕсли вам нужен больший доступ к фунционалу программы, запросите его у Админгистратора!\n\nГлавный Администратор: ");

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write("andryafpooo@mail.ru");

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nСписок доступных вам команд:");

                    string txtFilePath = ftpServer.localFilePathThree;
                    string line1, line2, line3, line4, line5, line6, line7, line8, line9, line10, line11, line12, line13, line14, line15, line16;

                    ReadLinesFromFile(txtFilePath, out line1, out line2, out line3, out line4, out line5, out line6, out line7, out line8, out line9, out line10, out line11, out line12, out line13, out line14, out line15, out line16);

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"\n{line1}\n{line2}\n{line3}\n{line6}");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                }
                else if (TypeCommands == "/GetData" || TypeCommands == "/getdata")
                {
                    QuestionUser();
                    db.GetDataFromDatabase();
                }
                else if (TypeCommands == "/Exit" || TypeCommands == "/exit")
                {
                    exitProgram = true;
                }
                else if (TypeCommands == "/ReadLogs" || TypeCommands == "/readlogs")
                {
                    ApacheLogsReaderFromDataBase();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x5 неизвестная команда или указано число!\nПопробуйте ещё раз!");
                }
            }
        }

        public void StartService()
        {
            string serviceName = "ApacheLogs";

            ServiceController service = new ServiceController(serviceName);

            if (service.Status == ServiceControllerStatus.Stopped)
            {
                try
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nСлужба успешно запущена!");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x3 при запуске службы: " + ex.Message);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nСлужба уже запущена!!!");
            }
        }

        public void StopService()
        {
            string serviceName = "ApacheLogs";

            ServiceController service = new ServiceController(serviceName);

            if (service.Status == ServiceControllerStatus.Running)
            {
                try
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nСлужба успешно остановлена!");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x4 при остановки службы: " + ex.Message);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nСлужба уже остановлена!!!");
            }
        }

        public void QuestionUser()
        {
            string question = "";

            question = Convert.ToString(question);
            questionTwo = Convert.ToString(questionTwo);

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("\nВы точно хотите совершить данное действие? (Y - Да или N - Нет)  -  ");
            Console.ForegroundColor = ConsoleColor.Blue;

            question = Console.ReadLine() ?? string.Empty;

            if (question == "Y" || question == "y")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"\nПодтвердите свой выбор {question} ? (Y - Да или N - Нет)  -  ");
                Console.ForegroundColor = ConsoleColor.Blue;

                questionTwo = Console.ReadLine() ?? string.Empty;

                if (questionTwo == "Y" || questionTwo == "y")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"\nВыбран {questionTwo}, выполнение операции!");
                }
                else if (questionTwo == "N" || questionTwo == "n")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"\nВыбран {questionTwo}, отмена операции!");

                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x1! Укажите пожалуйста заново свой ответ: (Y - Да или N - Нет)");

                    QuestionUser();
                }
            }
            else if (question == "N" || question == "n")
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write($"\nПодтвердите свой выбор {question} ? (Y - Да или N - Нет)  -  ");
                Console.ForegroundColor = ConsoleColor.Blue;

                questionTwo = Console.ReadLine() ?? string.Empty;

                if (questionTwo == "N" || questionTwo == "n")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"\nВыбран {questionTwo}, отмена операции!");

                    return;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x1! Укажите пожалуйста заново свой ответ: (Y - Да или N - Нет)");

                    QuestionUser();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nОшибка 0x1! Укажите пожалуйста заново свой ответ: (Y - Да или N - Нет)");

                QuestionUser();
            }

            return;
        }

        public void GetApi(string logFilePath, string allJsonFilePath, string byIpJsonFilePath)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("\nВведите IP-адресс по которому нужно сделать JSON файл: ");
            Console.ForegroundColor = ConsoleColor.Blue;

            string ipAddress = Console.ReadLine();

            var api = new AccessLogAPI(logFilePath);

            if (!File.Exists(allJsonFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка 0x22 файл {allJsonFilePath} не найден.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                api.SaveAllLogEntries(allJsonFilePath);
                Console.WriteLine($"\nФайл {allJsonFilePath} успешно создан.");
            }

            if (!File.Exists(byIpJsonFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка 0x22 файл {byIpJsonFilePath} не найден.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                api.SaveLogEntriesByIPAddress(ipAddress, byIpJsonFilePath);
                Console.WriteLine($"\nФайл {byIpJsonFilePath} успешно создан.");
            }
        }

        public class LogEntryApi
        {
            public string IPAddress { get; set; }
            public DateTime Timestamp { get; set; }
            public string Method { get; set; }
            public string URL { get; set; }
            public int StatusCode { get; set; }
            public int Size { get; set; }
        }

        public class AccessLogAPI
        {
            private List<LogEntryApi> logEntries;

            public AccessLogAPI(string logFilePath)
            {
                logEntries = ParseAccessLog(logFilePath);
            }

            public void SaveAllLogEntries(string filePath)
            {
                string json = JsonConvert.SerializeObject(logEntries, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }

            public void SaveLogEntriesByIPAddress(string ipAddress, string filePath)
            {
                var filteredEntries = logEntries.Where(entry => entry.IPAddress == ipAddress).ToList();
                string json = JsonConvert.SerializeObject(filteredEntries, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }

            public void ExportAllLogEntriesToJson(string outputPath)
            {
                string json = JsonConvert.SerializeObject(logEntries);
                File.WriteAllText(outputPath, json);
            }

            public void ExportLogEntriesByIPAddressToJson(string ipAddress, string outputPath)
            {
                var filteredEntries = logEntries.Where(entry => entry.IPAddress == ipAddress).ToList();
                string json = JsonConvert.SerializeObject(filteredEntries);
                File.WriteAllText(outputPath, json);
            }

            private List<LogEntryApi> ParseAccessLog(string logFilePath)
            {
                var logEntries = new List<LogEntryApi>();

                try
                {
                    using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fileStream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var entry = ParseLogEntry(line);
                            if (entry != null)
                                logEntries.Add(entry);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nОшибка 0x21 при чтении access.log: " + ex.Message);
                }

                return logEntries;
            }

            private LogEntryApi ParseLogEntry(string logLine)
            {
                var fields = logLine.Split(' ');

                if (fields.Length >= 10)
                {
                    var entry = new LogEntryApi
                    {
                        IPAddress = fields[0],
                        Timestamp = ParseTimestamp(fields[3] + " " + fields[4]),
                        Method = fields[5].Trim('"'),
                        URL = fields[6],
                        StatusCode = int.Parse(fields[8]),
                        Size = int.TryParse(fields[9], out var size) ? size : 0
                    };

                    return entry;
                }

                return null;
            }

            private DateTime ParseTimestamp(string timestamp)
            {
                if (DateTime.TryParseExact(timestamp, "[dd/MMM/yyyy:HH:mm:ss zzz]", null, System.Globalization.DateTimeStyles.None, out var parsedTimestamp))
                    return parsedTimestamp;

                return DateTime.MinValue;
            }
        }

        public class LogEntry
        {
            public string IP { get; set; }
            public DateTime DateTime { get; set; }
            public string Method { get; set; }
            public string Url { get; set; }
        }

        public class LogAggregator
        {
            private string logFilePath;

            public LogAggregator(string filePath)
            {
                logFilePath = filePath;
            }

            public List<LogEntry> ReadLogs()
            {
                var logEntries = new List<LogEntry>();

                using (var reader = new StreamReader(logFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var entry = ParseLogLine(line);
                        if (entry != null)
                        {
                            logEntries.Add(entry);
                        }
                    }
                }

                return logEntries;
            }

            private LogEntry ParseLogLine(string line)
            {
                var pattern = @"^([\d.]+) - - \[(.*?)\] ""(\w+) (.*?) HTTP/\d\.\d"" \d+ \d+$";
                var match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    var entry = new LogEntry
                    {
                        IP = match.Groups[1].Value,
                        DateTime = DateTime.ParseExact(match.Groups[2].Value, "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture),
                        Method = match.Groups[3].Value,
                        Url = match.Groups[4].Value
                    };

                    return entry;
                }

                return null;
            }
        }

        public void ApacheSaveLogsToDatabase()
        {
            DB db = new DB();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nХотите отправить Access.log в БД?");
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            QuestionUser();

            if (questionTwo == "Y" || questionTwo == "y")
            {
                try
                {
                    db.SaveLogsToDatabase();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\nЛоги успешно сохранены в базе данных.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\nОшибка при сохранении логов в базе данных: {ex.Message}");
                }
            }
            else if (questionTwo == "N" || questionTwo == "n")
            {
                return;
            }

            return;
        }

        public void ApacheLogsReaderFromDataBase()
        {
            DB db = new DB();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\nХотите прочитать Access.log из БД?");
            Console.ForegroundColor = ConsoleColor.Blue;

            QuestionUser();

            if (questionTwo == "Y" || questionTwo == "y")
            {
                string question = "";
                var ipAddress = "";

                question = Convert.ToString(question);
                questionTwo = Convert.ToString(questionTwo);

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\nХотите ли вы отфильровать ваш запрос по IP адрессу? (Y - Да или N - Нет)  -  ");
                Console.ForegroundColor = ConsoleColor.Blue;

                question = Console.ReadLine() ?? string.Empty;

                if (question == "Y" || question == "y")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write($"\nПодтвердите свой выбор {question} ? (Y - Да или N - Нет)  -  ");
                    Console.ForegroundColor = ConsoleColor.Blue;

                    questionTwo = Console.ReadLine() ?? string.Empty;

                    if (questionTwo == "Y" || questionTwo == "y")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine($"\nВыбран {questionTwo}, выполнение операции!");

                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("\nВведите нужный вам IP адресс: ");
                        Console.ForegroundColor = ConsoleColor.Blue;

                        ipAddress = Console.ReadLine();

                        if (ipAddress != null)
                        {
                            if (!IsValidIpAddress(ipAddress))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Ошибка! Введен неверный формат IP-адреса.");
                                return;
                            }
                            db.RetrieveLogsFromDatabase(ipAddress);
                        }
                    }
                    else if (questionTwo == "N" || questionTwo == "n")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine($"\nВыбран {questionTwo}, отмена операции!");

                        return;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nОшибка 0x1! Укажите пожалуйста заново свой ответ: (Y - Да или N - Нет)");

                        QuestionUser();
                    }
                }
                else if (question == "N" || question == "n")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write($"\nПодтвердите свой выбор {question} ? (Y - Да или N - Нет)  -  ");
                    Console.ForegroundColor = ConsoleColor.Blue;

                    questionTwo = Console.ReadLine() ?? string.Empty;

                    if (questionTwo == "N" || questionTwo == "n")
                    {
                        db.RetrieveLogsFromDatabase();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nОшибка 0x1! Укажите пожалуйста заново свой ответ: (Y - Да или N - Нет)");

                        QuestionUser();
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nОшибка 0x1! Укажите пожалуйста заново свой ответ: (Y - Да или N - Нет)");

                    QuestionUser();
                }
            }
            else
            {
                return;
            }
        }

        public static bool IsValidIpAddress(string ipAddress)
        {
            string pattern = @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";

            return Regex.IsMatch(ipAddress, pattern);
        }

        public string? ReadTextFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);

                return content;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nОшибка 0x7 при чтении файла: {ex.Message}");

                return null;
            }
        }

        public void ReadLinesFromFile(string? txtFilePath, out string? line1, out string? line2, out string? line3, out string? line4, out string? line5, out string? line6, out string? line7, out string? line8, out string? line9, out string? line10, out string? line11, out string? line12, out string? line13, out string? line14, out string? line15, out string? line16)
        {
            line1 = null;
            line2 = null;
            line3 = null;
            line4 = null;
            line5 = null;
            line6 = null;
            line7 = null;
            line8 = null;
            line9 = null;
            line10 = null;
            line11 = null;
            line12 = null;
            line13 = null;
            line14 = null;
            line15 = null;
            line16 = null;

            if (txtFilePath != null)
            {
                using (StreamReader sr = new StreamReader(txtFilePath))
                {
                    line1 = sr.ReadLine();

                    line2 = sr.ReadLine();

                    line3 = sr.ReadLine();

                    line5 = sr.ReadLine();

                    line6 = sr.ReadLine();

                    line7 = sr.ReadLine();

                    line8 = sr.ReadLine();

                    line9 = sr.ReadLine();

                    line10 = sr.ReadLine();

                    line11 = sr.ReadLine();

                    line12 = sr.ReadLine();

                    line13 = sr.ReadLine();

                    line14 = sr.ReadLine();

                    line15 = sr.ReadLine();

                    line16 = sr.ReadLine();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nОшибка 0x6 путь к файлу равен null!");
            }
        }
    }
}