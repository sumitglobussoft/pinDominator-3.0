using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BaseLib
{
    public class FileBrowseHelper
    {
        public static string UploadTextFile(string DirectoryPath)
        {
            string FilePath = string.Empty;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = DirectoryPath;
                ofd.Filter = "Text Files (*.txt)|*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    FilePath = ofd.FileName;
                }
            }

            return FilePath;
        }
    }
}
