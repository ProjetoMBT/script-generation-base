using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProductInstantiator
{
    /// <summary>
    /// Contains several utilitary methods.
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Copy source directory to target destination.
        /// </summary>
        /// <param name="source">Directory to be copied.</param>
        /// <param name="target">Location of generated copy.</param>
        public static void CopyDirectory(string source, string target)
        {
            //Remove directories with same name (if suitable).
            if (Directory.Exists(target) && !target.Equals("./Repository/"))
            {
                Directory.Delete(target, true);
            }

            //Creates a new directory to copy files to.
            if (!target.Equals("./Repository/"))
            {
                Directory.CreateDirectory(target);
            }

            //Create files (non-directories) to destination (target).
            string[] filesInsideSource = Directory.GetFiles(source);
            foreach (string s in filesInsideSource)
            {
                FileInfo f = new FileInfo(s);
                File.Copy(s, target + "/" + f.Name,true);
            }

            //Recursivelly copies directories to destination.
            string[] directoriesInsideSource = Directory.GetDirectories(source);
            foreach (string s in directoriesInsideSource)
            {
                DirectoryInfo d = new DirectoryInfo(s);
                Utils.CopyDirectory(s, target + "/" + d.Name);
            }
        }
    }
}
