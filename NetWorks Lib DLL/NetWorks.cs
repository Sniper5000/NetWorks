
using System;
using System.Collections.Generic;

namespace NetWorks
{
    public class GeneralSettings
    {
        /*
            /!/ Do not modify unless you know what you're doing /!/
            -- Incorrect Settings will cause NetWorks to fail
        */

        /// <summary>
        /// NetWorks Current Version. 
        /// </summary>
        public static readonly string Version = "1.3.0";
        /// <summary>
        /// Minimum Version this NetWorks version can communicate with.
        /// </summary>
        public static readonly string MinVersion = "1.3.0";

        //Checkers
        public static bool VersionChecker(string CheckVersion)
        {
            if(string.CompareOrdinal(Version, CheckVersion) == 0)
                return true; //Same version detected.

            //Maybe, this version is within the min version range
            try
            {
                var M = int.Parse(MinVersion.Replace(".",""));
                var CVersion = int.Parse(CheckVersion.Replace(".", ""));
                var V = int.Parse(Version.Replace(".", ""));
                //So long the version is within minimum version accept client.
                if (M <= CVersion && CVersion <= V) return true;
            } 
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            Console.WriteLine($"CHALLENGE FAILURE: Connection Version is outside supported version range. Minimum {MinVersion} | Tried Version {CheckVersion} | Current Version {Version}");
            return false;
        }
    }

    public enum MessageSendMode
    {
        Unreliable,
        Reliable,
    }
}


