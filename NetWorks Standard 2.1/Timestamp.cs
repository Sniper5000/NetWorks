namespace NetWorks
{
    public class Timestamp
    {
        /// <summary>
        /// Calculates the difference between 2 timestamps
        /// </summary>
        /// <param name="startTimestamp"></param>
        /// <param name="endTimestamp"></param>
        /// <returns>  </returns>
        public static long CalculateZeroedTimestamp(long startTimestamp, long endTimestamp)
        {
            return endTimestamp - startTimestamp;
        }
    }
}