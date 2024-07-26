namespace NetWorks_Library
{
    public class TestLibrary
    {
        private List<TestEntry> entries = new();

        public void AddTest(string tooltip, Action performTest)
        {
            entries.Add(new(tooltip, performTest));
        }

        public void InteractiveTestSelection()
        {
            while(true)
            {
                Console.WriteLine("--- WELCOME to the NETWORKS LIBRARY interactive test suite! ---");
                
                for(int i = 0; i < entries.Count; i++)
                {
                    Console.WriteLine($"[ {i} ] {entries[i].Tooltip}");
                }

                Console.WriteLine("Input the test number you wish to perform, prefix with R to repeat it infinitely");
                string query = (Console.ReadLine() ?? throw new NullReferenceException()).ToLower();
                bool repeat = false;

                if(query[0] == 'r')
                {
                    repeat = true;
                    query = query[1..];
                }

                int testIndex = int.Parse(query);

                if(testIndex == 9 && repeat)
                {
                    Console.WriteLine("[WARNING]: This test performs Read & Writes on the drive. Are you sure you want to continue?" + Environment.NewLine + "[Y or N]");
                    string Response = (Console.ReadLine() ?? throw new NullReferenceException()).ToLower();

                    if (Response[0] != 'y')
                        continue;
                }
                do PerformTest(testIndex);
                while(repeat);
            }
        }

        public void PerformTest(int index)
        {
            entries[index].PerformTest();
        }

        private record TestEntry(string Tooltip, Action PerformTest);
    }
}