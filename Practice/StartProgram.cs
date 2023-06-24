using FtpServerApactheAggregator;
using ModulesApactheAggregator;

namespace StartProgramApactheAggregator
{
    public class StartProgram
    {
        public void visualStartProgram()
        {
            MD md = new MD();
            FTPServer ftpServer = new FTPServer();

            string? visual = "";
            string nameOfCompany = "Jmix Programs";
            string version = "0.1.4.56";

            Console.ForegroundColor = ConsoleColor.DarkBlue;
            visual = ftpServer.localFilePathTwo != null ? md.ReadTextFile(ftpServer.localFilePathTwo) : null;

            Console.WriteLine($"\n{visual}");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("О программе: ");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Издатель программы: ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{nameOfCompany}");

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"Версия программы: ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"{version}\n\n");
        }
    }
}