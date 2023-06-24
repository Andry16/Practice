using System.Net;

namespace FtpServerApactheAggregator
{
    public class FTPServer
    {
        string currentDirectory = Environment.CurrentDirectory;

        public string directoryPathOne;
        public string directoryPathTwo;
        public string directoryPathThree;
        public string directoryPathFour;
        public string directoryPathFive;
        public string nameFileOne;
        public string nameFileTwo;
        public string nameFileThree;
        public string nameFileFour;
        public string nameFileFive;
        public string serverftp;
        public string username;
        public string password;
        public string remoteFilePathOne;
        public string remoteFilePathTwo;
        public string remoteFilePathThree;
        public string localFilePathOne;
        public string localFilePathTwo;
        public string localFilePathThree;
        public string localFilePathFour;
        public string localFilePathFive;

        public FTPServer()
        {
            directoryPathOne = "/Apache24";
            directoryPathTwo = "/logs";
            directoryPathThree = "/Resourses";
            directoryPathFour = "/txt";
            directoryPathFive = "/json";
            nameFileOne = "/access.log";
            nameFileTwo = "/startImage.txt";
            nameFileThree = "/discription.txt";
            nameFileFour = "/AllAccessLog.json";
            nameFileFive = "/ByIpAccessLog.json";
            serverftp = "ftp://server4.hosting.reg.ru";
            username = "u2096277";
            password = "0ot29R45cOoAdPFi";
            remoteFilePathOne = "/Apatch24/logs/access.log";
            remoteFilePathTwo = "/Resourses/txt/startImage.txt";
            remoteFilePathThree = "/Resourses/txt/discription.txt";
            localFilePathOne = $"{currentDirectory}" + $"{directoryPathOne}" + $"{directoryPathTwo}" + $"{nameFileOne}";
            localFilePathTwo = $"{currentDirectory}" + $"{directoryPathThree}" + $"{directoryPathFour}" + $"{nameFileTwo}";
            localFilePathThree = $"{currentDirectory}" + $"{directoryPathThree}" + $"{directoryPathFour}" + $"{nameFileThree}";
            localFilePathFour = $"{currentDirectory}" + $"{directoryPathThree}" + $"{directoryPathFive}" + $"{nameFileFour}";
            localFilePathFive = $"{currentDirectory}" + $"{directoryPathThree}" + $"{directoryPathFive}" + $"{nameFileFive}";
        }

        public void CheckDirectoryPath(string nameDirectoryOne, string nameDirectoryTwo)
        {
            if (Directory.Exists(currentDirectory + nameDirectoryOne + nameDirectoryTwo))
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("Проверка папки... Успешно!");
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(currentDirectory + nameDirectoryOne + nameDirectoryTwo);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Папка успешно создана.");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка 0x9 при создании папки: " + ex.Message);
                }
            }
        }

        public void DownloadFileFromFTPServer(string serverftp, string username, string password, string remoteFilePath, string localFilePath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverftp + remoteFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(username, password);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(responseStream))
                    using (StreamWriter writer = new StreamWriter(localFilePath))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.WriteLine("Скачивание или обновление файла... Успешно!");

                        writer.Write(reader.ReadToEnd());
                    }
                }
            }
            catch (WebException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка 0x8 при скачивании или обновлении файла: " + ex.Message);
            }
        }
    }
}