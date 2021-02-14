using System.IO;

namespace BasicBackup
{
    public class Source
    {
        public DirectoryInfo SourceDirectory { get; set; }
        public int Directories { get; set; }
        public int Files { get; set; }
        public long Size { get; set; }

        public Source() { }
        public Source(string sourceDirectory)
        {
            SourceDirectory = new DirectoryInfo(sourceDirectory.Trim('\\'));
        }

        internal void CalculateInfo()
        {
            CalculateInfoForDirectory(SourceDirectory);
        }
        
        private void CalculateInfoForDirectory(DirectoryInfo currentDir)
        {
            CalculateInfoForfilesInDirectory(currentDir.GetFiles("*.*"));
            CalculateInfoForDirectoriesInDirectory(currentDir.GetDirectories());
        }

        private void CalculateInfoForDirectoriesInDirectory(DirectoryInfo[] directories)
        {
            Directories += directories.Length;
            foreach (DirectoryInfo currentDirectory in directories)
            {
                CalculateInfoForDirectory(currentDirectory);
            }
        }

        private void CalculateInfoForfilesInDirectory(FileInfo[] files)
        {
            Files += files.Length;
            foreach (var file in files)
            {
                Size += file.Length;
            }
        }
    }
}
