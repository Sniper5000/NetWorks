using System.Net;
using NetWorks.FileEx;
using NetWorks.Utils;
using NetWorks_Library.FileGenerator;
using NetWorks.Network;

/// <summary>
/// Tests FileEx sending a file and receiving it via TCP.
/// </summary>
public class StandaloneFileExTest
{
    private static readonly IPEndPoint endPoint = new(IPAddress.Parse("127.0.0.1"), 9999);

    public void Run()
    {
        //Check if the file exists! else, create it
        //Asks the User how large they want the test file to be.. it's created only once.
        UselessFileGenerator.TryCreateUselessFile("Files/UselessTestFile.Useless", 10737418240);

        Task.Run(RunServer);
        var fileExClient = NetWorks.FileEx.FileExClient.DirectConnect(endPoint, 1024 * 1024, 1024 * 1024);
        //using MemoryStream file = new();
        if (!Directory.Exists("Temp"))
        {
            Directory.CreateDirectory("Temp");
        }
     
        /*
        fileExClient.ReceiveFile("Temp", out int tag, (read, total) =>
        {
            Console.WriteLine("Received {0}/{1}", NumberFormatting.FormatDataMagnitude(read), NumberFormatting.FormatDataMagnitude(total));
        });*/
        MemoryStream memoryStream = new MemoryStream();
        //var throttledstream = new ThrottledStream(memoryStream, 1024 * 4000);
        string FileName = "";
        int tag = 0;
        try
        {
            fileExClient.ReceiveFile(memoryStream, out FileName, out tag, (read, total) =>
            {
                Console.WriteLine("Received {0}/{1}", NumberFormatting.FormatDataMagnitude(read), NumberFormatting.FormatDataMagnitude(total));
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        Console.WriteLine($"Received file {FileName}! tag: {tag}");
        Thread.Sleep(500);
        memoryStream.Dispose();
        memoryStream.Close();
        FileExClient.Shutdown(); //Only shutdown FileEx connection after you've finished sending/receiving files.. otherwise files may be "Incomplete"
        
    }

    private void RunServer()
    {
        FileExClient.DirectListen(endPoint, fileExClient =>
        {
            Console.WriteLine("Client connected!");
            
            var TS = File.Open("Files/UselessTestFile.Useless", FileMode.Open, FileAccess.Read, FileShare.Read);
            //var TS = new ThrottledStream(File.Open("Files/UselessTestFile.Useless", FileMode.Open, FileAccess.Read, FileShare.Read), 1024 * 2048);
            fileExClient.StreamFile(TS, "UselessTestFile.Useless", 999, true, (read, total) =>
            {
                Console.WriteLine("Sending {0}/{1}", NumberFormatting.FormatDataMagnitude(read), NumberFormatting.FormatDataMagnitude(total));
            });
            TS.Close();
            Console.WriteLine($"File Sent!");
        });
    }
}