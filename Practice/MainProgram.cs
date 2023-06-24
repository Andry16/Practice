using DataBaseApactheAggregator;
using FtpServerApactheAggregator;
using ModulesApactheAggregator;
using StartProgramApactheAggregator;

namespace MainApacheLogAggregator
{
    class MainProgram
    {
        static void Main(string[] args)
        {
            StartProgram startProgram = new StartProgram();
            MD md = new MD();
            DB db = new DB();
            FTPServer ftpserver = new FTPServer();

            ftpserver.CheckDirectoryPath(ftpserver.directoryPathThree, ftpserver.directoryPathFour);
            ftpserver.DownloadFileFromFTPServer(ftpserver.serverftp, ftpserver.username, ftpserver.password, ftpserver.remoteFilePathTwo, ftpserver.localFilePathTwo);

            startProgram.visualStartProgram();

            db.CheckConnection();

            ftpserver.CheckDirectoryPath(ftpserver.directoryPathOne, ftpserver.directoryPathTwo);
            ftpserver.CheckDirectoryPath(ftpserver.directoryPathThree, ftpserver.directoryPathFour);
            ftpserver.CheckDirectoryPath(ftpserver.directoryPathThree, ftpserver.directoryPathFive);
            ftpserver.DownloadFileFromFTPServer(ftpserver.serverftp, ftpserver.username, ftpserver.password, ftpserver.remoteFilePathOne, ftpserver.localFilePathOne);
            ftpserver.DownloadFileFromFTPServer(ftpserver.serverftp, ftpserver.username, ftpserver.password, ftpserver.remoteFilePathThree, ftpserver.localFilePathThree);

            md.ChooseAction();
        }
    }
}