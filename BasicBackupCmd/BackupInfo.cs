using System;
using System.Collections.Generic;

namespace BasicBackup
{
    public class BackupInfo
    {
        public List<string> Parameters { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public DateTime Created { get; set; }
        public BackupInfo() { }
        public int Directories{ get; set; }
        public int Files{ get; set; }
        public long Size { get; set; }

    }
}
