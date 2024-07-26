using NetWorks.Utils;
using NetWorks.Security;

internal class EncryptionMemoryUsageTest
{
    public void Run()
    {
        IterationCycle(8, 5);
        IterationCycle(8, 40);
        IterationCycle(8, 80);
        IterationCycle(8, 120);
        IterationCycle(8, 200);
        IterationCycle(8, 5);
        IterationCycle(8, 40);
    }

    private void IterationCycle(int iterations, int bufferSizeMb)
    {
        SecurityKeypair keys = new();
        for(int i = 0; i < iterations; i++)
        {
            GC.Collect();
            long memoryUsage = GC.GetTotalMemory(true);
            int size = bufferSizeMb * 1024 * 1024;
            float ratio = (float)memoryUsage / size;

            Console.WriteLine("Iteration {0}: {1} (buffer is {2}) {3}", i, 
                NumberFormatting.FormatDataMagnitude(memoryUsage, 3), 
                NumberFormatting.FormatDataMagnitude(size, 3), ratio);

            {
                byte[] buffer = new byte[size];
                byte[] encrypted = AedmEncryption.Encrypt(keys.PublicKey, buffer);
                AedmEncryption.Decrypt(keys.PrivateKey, encrypted);
            }
        }
    }
}