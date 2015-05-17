using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XAMLPropertyAdder
{
    /// <summary>
    /// Finds all the files associated with a filetype in the given directory and its subdirectories.
    /// Results is a list of string representations of the absolute paths of each of these files.
    /// </summary>
    public class DirCrawler
    {
        public List<string> Results { get; private set; }

        public DirCrawler()
        {

            Results = new List<string>();
        }

        public void GetFiles(string path, string filter)
        {
            var dirs = Directory.EnumerateDirectories(path);
            var files = Directory.EnumerateFiles(path, filter).ToList();
            if (files.Any())
            {
                Results.AddRange(files);
            }
            foreach (var dir in dirs)
            {
                GetFiles(dir, filter); // Recursion here. Careful about using this with Escher filesystems.
            }
        }
    }
}