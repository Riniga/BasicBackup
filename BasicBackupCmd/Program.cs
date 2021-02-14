using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BasicBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                DisplayHelp();
            }
            else
            {
                List<string> parameters = new List<string>();
                int index;
                for (index=1;index<args.Length;index++)
                {
                    if (args[index].StartsWith('/')) parameters.Add(args[index].ToUpper());
                    else break;
                }
                if (index > args.Length - 2)
                {
                    DisplayHelp();
                    return;
                }

                var sourceDirectory = args[index++];
                if (!Directory.Exists(sourceDirectory)) 
                {
                    Console.WriteLine("Failure: Directory " + sourceDirectory + " does not exist. Program terminated");
                    return;
                }

                var source = new Source(Path.GetFullPath(sourceDirectory));

                var destination = args[index];
                if (!Path.IsPathFullyQualified(destination)) destination = Path.GetFullPath(destination);
                if (!destination.EndsWith(".zip")) destination = destination + ".zip";
                if (File.Exists(destination)) parameters.Add("/A");


                var backuper = new Backuper(parameters, source, destination);

                Console.WriteLine("Number of directories:\t" + backuper.BackupInfo.Directories);
                Console.WriteLine("Number of files:\t" + backuper.BackupInfo.Files);
                Console.WriteLine("total size:\t\t" + HumanReadableSize(backuper.BackupInfo.Size));

                backuper.doBackup();
            }
        }

        private static string HumanReadableSize(long size)
        {
            string[] units = new string[] { "bytes", "kB", "MB", "GB", "TB" };
            int unitId = 0;
            while (size >= 1024)
            {
                size /= 1024;
                unitId++;
            }
            if (unitId >= units.Length) unitId = units.Length-1;
            return size + " " + units[unitId];
        }

        private static void DisplayHelp()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            var version = currentAssembly.GetName().Version;
            Console.WriteLine($"Basic backup and restore application version: {version.Major}.{version.Minor} build: { currentAssembly.GetLinkerTime() }  ");
            Console.WriteLine("\r\nBASICBACKUP [/V] [/Y] source [+ source [+...]] destination");
            Console.WriteLine("");
            Console.WriteLine("  source\tSpecifies the file or files to be copied.");
            Console.WriteLine("  destination\tSpecifies the filename for the new backup.");
            Console.WriteLine("  /V\t\tVerifies that new files are written correctly.");
            Console.WriteLine("  /Y\t\tSuppresses prompting to confirm you want to overwrite an existing destination file.");
        }
    }
}
