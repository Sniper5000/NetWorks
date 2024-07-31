using NetWorks.Utils;
using System;

public static class TestUtils
{
    public static void TraceMemoryUsage(string? tag = null)
    {
        GC.Collect();
        long usage = GC.GetTotalMemory(true);
        
        Console.Write("Memory usage: {0}", NumberFormatting.FormatDataMagnitude(usage));
        if(tag != null) Console.Write(" ({0})", tag);
        Console.WriteLine();
    }
}