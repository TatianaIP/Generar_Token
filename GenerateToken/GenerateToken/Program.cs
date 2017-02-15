using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GenerateToken
{
    class Program
    {
        private static string key = null;
        private static string appID = null;
        private static string userName = null;
        private static long   expiresInSecs = 0;
        private static string expiresAt = null;

        private const long EPOCH_SECONDS = 62167219200;

        public static void Main(string[] args)
        {
            // Parse the command line arguments
            string delimStr = "=";
            char[] delimiter = delimStr.ToCharArray();
            string[] split = null;

            try
            {
                for (int i = 0; i < args.Length; ++i)
                {
                    //Console.WriteLine("i = {0}, val = {1}", i, args[i]);
                    split = args[i].Split(delimiter, 2);

                    if (split[0].Contains("-help"))
                    {
                        Console.WriteLine("\nThis program will generate a provision login token from a developer key");
                        Console.WriteLine("\nOptions:");
                        Console.WriteLine("     --key           Developer key supplied with the developer account");
                        Console.WriteLine("     --appID         ApplicationID supplied with the developer account");
                        Console.WriteLine("     --userName      Username to generate a token for");
                        Console.WriteLine("     --vCardFile     Path to the XML file containing a vCard for the user");
                        Console.WriteLine("     --expiresInSecs Number of seconds the token will be valid can be used instead of expiresAt");
                        Console.WriteLine("     --expiresAt     Time at which the token will expire ex: (2055-10-27T10:54:22Z) can be used instead of expiresInSecs");
                        return;
                    }
                    else if (split[0].Contains("key"))
                    {
                        key = split[1];
                    }
                    else if (split[0].Contains("appID"))
                    {
                        appID = split[1];
                    }
                    else if (split[0].Contains("userName"))
                    {
                        userName = split[1];
                    }
                    else if (split[0].Contains("vCardFile"))
                    {
                        // NOT SUPPORTED
                    }
                    else if (split[0].Contains("expiresInSecs"))
                    {
                        expiresInSecs = long.Parse(split[1]);
                    }
                    else if (split[0].Contains("expiresAt"))
                    {
                        expiresAt = split[1];
                    }
                    else
                    {
                        Console.WriteLine("Argument {0} is not valid.", split[0]);
                    }
                    //Console.WriteLine("   {0} {1}", split[0], split[1]);
                }
            }
            catch (Exception e)
            {
                Console.Write("Command line exception: ");
                Console.WriteLine(e.Message);
                return;
            }

            // As long as proper arguments were entered, generate the token
            if ((appID != null) && (key != null) && (userName != null))
            {
                string expires = "";

                // Check if using expiresInSecs or expiresAt
                if (expiresInSecs > 0)
                {
                    TimeSpan timeSinceEpoch = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                    expires = (Math.Floor(timeSinceEpoch.TotalSeconds) + EPOCH_SECONDS + expiresInSecs).ToString();
                }
                else if (expiresAt != null)
                {
                    try
                    {
                        TimeSpan epochToExpires = DateTime.Parse(expiresAt).ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
                        expires = (Math.Floor(epochToExpires.TotalSeconds) + EPOCH_SECONDS).ToString();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nException caught in expiresAt time calculation. Time format probably invalid. Should look like so: 2055-10-27T10:54:22Z");
                        Console.WriteLine(e);
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("\nExiting! Neither expiresInSecs or expiresAt was set.");
                    return;
                }

                Console.WriteLine("Setting key           : " + key);
                Console.WriteLine("Setting appId         : " + appID);
                Console.WriteLine("Setting userName      : " + userName);
                Console.WriteLine("Setting expiresInSecs : {0}", expiresInSecs);
                if (expiresAt != null)
                    Console.WriteLine("Setting expiresAt     : " + expiresAt);
                Console.WriteLine("Expirey time          : " + expires);
                string jid = userName + "@" + appID;
                string body = "provision" + "\0" + jid + "\0" + expires + "\0" + "";

                var encoder = new UTF8Encoding();
                var hmacsha = new HMACSHA384(encoder.GetBytes(key));
                byte[] mac = hmacsha.ComputeHash(encoder.GetBytes(body));

                // macBase64 can be used for debugging
                //string macBase64 = Convert.ToBase64String(hashmessage);

                // Get the hex version of the mac
                string macHex = BytesToHex(mac);

                string serialized = body + '\0' + macHex;

                Console.WriteLine("\nGenerated token:\n" + Convert.ToBase64String(encoder.GetBytes(serialized)));
            }
        }

        private static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}
