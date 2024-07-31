namespace NetWorks_Library;
using NetWorks.Utils;
using NetWorks_Library.Examples;

public class Program
{
    static void Main(string[] args)
    {
        new Program().Entry();
    }

    private void Entry()
    {
        Action? forceTest = null;
        
        if(forceTest is Action performTest)
        {
            while(true)
            {
                performTest();
                Console.ReadLine();
            }
        }

        TestLibrary library = BuildTestLibrary();
        library.InteractiveTestSelection();
    }

    private TestLibrary BuildTestLibrary()
    {
        TestLibrary library = new();
        library.AddTest("Security test suite", () => new SecurityTests().Run());
        library.AddTest("Number formatting functions", () => TestNumberFormat(PromptLong()));
        library.AddTest("Number formatting function suite", TestNumberFormatSuite);
        library.AddTest("Test security challenge failure", NetworkTests.TestSecurityChallengeFail);
        library.AddTest("Test file exchange protocol (no server/client)", NetworkTests.TestFileExchange);
        library.AddTest("Test file exchange protocol (with server/client)", NetworkTests.TestFileExchangeConnection);
        library.AddTest("Test FileEx server/client", () => new FileExTest().Run());
        library.AddTest("Test stream encryption", () => new StreamEncryptionTest().Run());
        library.AddTest("Test encryption/decryption memory usage", () => new EncryptionMemoryUsageTest().Run());
        library.AddTest("Test standalone FileExClient", () => new StandaloneFileExTest().Run());
        library.AddTest("Example: Send file via FileEx", () => new SendFileViaFileEx().Run());
        return library;
    }

    private long PromptLong()
    {
        return long.Parse(Console.ReadLine() ?? throw new NullReferenceException());
    }

    private void TestNumberFormat(long number)
    {
        const int tableSize = 10;
        Console.WriteLine($"{number,20} AN: {NumberFormatting.FormatNumberMagnitudeInteger(number),tableSize} FK: {NumberFormatting.FormatNumberMagnitude(number, 3),tableSize} SK: {NumberFormatting.FormatDataMagnitude(number),tableSize}");
    }

    private void TestNumberFormatSuite()
    {
        long[] values = new long[] 
        { 0, 1, 100, 1024, 2048, 3000, 5000, 9999, 999999, 11111111, 1111111111, -5000, -0, -1, long.MaxValue, long.MinValue };
        foreach (long value in values) TestNumberFormat(value);
    }
}