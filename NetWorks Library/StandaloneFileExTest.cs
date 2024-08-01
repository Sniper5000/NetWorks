using System.Net;
using NetWorks.FileEx;
using NetWorks.Utils;
using NetWorks_Library.FileGenerator;

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
        var fileExClient = NetWorks.FileEx.FileExClient.DirectConnect(endPoint);
        //using MemoryStream file = new();
        if (!Directory.Exists("Temp"))
        {
            Directory.CreateDirectory("Temp");
        }
     

        fileExClient.ReceiveFile("Temp", out int tag, (read, total) =>
        {
            Console.WriteLine("Received {0}/{1}", NumberFormatting.FormatDataMagnitude(read), NumberFormatting.FormatDataMagnitude(total));
        });
        Console.WriteLine($"Received file! tag: {tag}");
        Thread.Sleep(500);
        FileExClient.Shutdown(); //Only shutdown FileEx connection after you've finished sending/receiving files.. otherwise files may be "Incomplete"
        
    }

    private void RunServer()
    {
        FileExClient.DirectListen(endPoint, fileExClient =>
        {
            Console.WriteLine("Client connected!");
            fileExClient.StreamFile("Files/UselessTestFile.Useless", 999, true);
        });
    }
}