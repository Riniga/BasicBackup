using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading;

namespace BasicBackup
{
    class Backuper
    {
        public BackupInfo BackupInfo;
        private List<string> _parameters;
        private Source _source;
        private string _destination { get; set; }
        private int Progress;

        public Backuper(List<string> parameters, Source source, string destination)
        {
            _parameters = parameters;
            _source = source;
            _destination = destination;
            _source.CalculateInfo();
            BackupInfo = new BackupInfo
            {
                Parameters = _parameters,
                Source = _source.SourceDirectory.FullName,
                Destination=_destination,
                Created = DateTime.Now,
                Directories= _source.Directories,
                Files= _source.Files,
                Size=_source.Size
            };
        }

        internal void doBackup()
        {
            Console.WriteLine("Backup from source: " + _source.SourceDirectory.FullName );
            Console.WriteLine("To destination: " + BackupInfo.Destination);
            if (BackupInfo.Parameters.Contains("/V")) Console.WriteLine("Backup will be verified");
            if (BackupInfo.Parameters.Contains("/Y")) Console.WriteLine("confirm will be suppressed");
            if (BackupInfo.Parameters.Contains("/A")) AppendToBackup();
            else CreateNewBackup();
        }

        private void CreateNewBackup()
        {
            Thread thread = new Thread(UpdateProgressReport);
            thread.Start();

            using (FileStream zipToOpen = new FileStream(BackupInfo.Destination, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    AddBackupInfoFile(archive);
                    if (Directory.Exists(_source.SourceDirectory.FullName)) 
                        AddDirectoryTree(_source.SourceDirectory, _source.SourceDirectory, archive);
                }
            }
        }

        private void UpdateProgressReport()
        {
            int percent = 100 * Progress / BackupInfo.Files;
            while (percent<100)
            { 
                Console.CursorLeft = 0;
                percent = 100 * Progress / BackupInfo.Files;
                Console.Write(Progress + " files, " + percent.ToString().PadLeft(2, '0') + " %");
                Thread.Sleep(200);
            }
        }

        private void AddDirectoryTree(DirectoryInfo currentRoot, DirectoryInfo currentDir, ZipArchive archive)
        {
            string directory = currentDir.FullName.Replace(currentRoot.FullName ,"").Trim('\\');
            if (!string.IsNullOrEmpty(directory)) directory += '\\';
            FileInfo[] files = currentDir.GetFiles("*.*");
            Progress += files.Length;
            foreach (FileInfo file in files)
            {
                archive.CreateEntryFromFile(file.FullName, directory + file.Name);
            }

            DirectoryInfo[] subDirs = currentDir.GetDirectories();
            foreach (DirectoryInfo dirInfo in subDirs)
            {
                AddDirectoryTree(currentRoot,dirInfo, archive);
            }
        }

        private void AddBackupInfoFile(ZipArchive archive)
        {
            var readmeEntry = archive.CreateEntry("basicbackup.json");

            using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
            {
                var data = JsonSerializer.Serialize<BackupInfo>(BackupInfo);
                writer.WriteLine(data);
            }
        }

        private void AppendToBackup()
        {
            Console.WriteLine("data will be appended to available backup");
            Console.WriteLine("FAILURE: Not yet implmented");
        }
    }
}
