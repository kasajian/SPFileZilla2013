using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SPFileZillaCli
{
    class Program
    {
        public static void ConfigureJson() => JsonConvert.DefaultSettings =
            () =>
                new JsonSerializerSettings
                { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore, Converters = { new StringEnumConverter() } };

        static int Main(string[] args)
        {
            try
            {
                DoIt(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 1;
            }
            Console.WriteLine("Done");
            return 0;
        }

        private static void DoIt(string[] args)
        {
            ConfigureJson();

            if (args.Length == 0)
            {
                Help();
                return;
            }

            var file = args[0];
            if (!File.Exists(file))
            {
                Console.WriteLine(@"Could not fine file '{0}'", file);
            }

            var fileContent = File.ReadAllText(file);
            var commands = JsonConvert.DeserializeObject<Commands>(fileContent);

            bool skipped;
            string msg;

            BandR.SpComHelper.UploadFileToSharePoint(
                commands.SiteUrl,
                commands.SiteUsername,
                commands.SitePwd,
                commands.SiteDomain,
                false,
                commands.FilePath,
                commands.FolderUrl,
                true,
                null, null,
                out skipped,
                out msg
                );

            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
        }

        class Commands
        {
            public string SiteUrl;
            public string SiteUsername;
            public string SitePwd;
            public string SiteDomain;
            public string FilePath;
            public string FolderUrl;
            public bool Overwrite;
        }

        private static void Help()
        {
            var commands = new Commands()
            {
                SiteUrl = "http://sharepoint.acme.com/sandbox/kenny/",
                SiteUsername = "admin",
                SitePwd = "Bob Loblaw",
                SiteDomain = "ACME",
                FilePath = @"c:\myfiles\myreport.docx",
                FolderUrl = "Docs",
                Overwrite = true,
            };
            var formatedJson = JsonConvert.SerializeObject(commands);

            Console.WriteLine(@"Command line should be path to a json file that specifies what to do.");
            Console.WriteLine(@"Example:");
            Console.WriteLine(@"SPFileZillaCli commands.json");
            Console.WriteLine(@"");
            Console.WriteLine(@"Contents of commands.json can look something like this:");
            Console.WriteLine(@"");
            Console.WriteLine(formatedJson);
        }
    }
}

