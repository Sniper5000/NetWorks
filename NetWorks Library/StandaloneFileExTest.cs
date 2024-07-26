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
        if (!File.Exists("Files/UselessTestFile.Useless"))
        {
            //Generate!
            Console.WriteLine("Please input the file size in MB");
            long Response = long.Parse(Console.ReadLine() ?? throw new NullReferenceException());
            if (Response * 1048576 > 10737418240)
            {
                Console.WriteLine($"Your desired file size {Response} exceeds the 10GB limit and thus has been set to 10GB");
                Response = 10240;
            }

            Console.WriteLine("Generating file..");
            UselessFileGenerator.CreateUselessFile("Files/UselessTestFile.Useless", Response * 1048576);
            Console.WriteLine("Complete!");
        }

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