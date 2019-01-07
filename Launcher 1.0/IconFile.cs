using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher_1._0
{
    class IconFile
    {
        public IconFile()
        {
        }

        public void StartProcess(string path)
        {
            string programPath = System.AppDomain.CurrentDomain.BaseDirectory;

            Process proc = new Process();
            proc.StartInfo.FileName = programPath;
            proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(programPath);
            proc.Start();
        }

        void createAllDir(string sourcePath, string destinationPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
        }

        List<FileInfo> getFileInfos(Uri uri)
        {
            return new DirectoryInfo(uri.ToString()).GetFiles("*.exe*", SearchOption.AllDirectories).ToList();
        }
    }
}
