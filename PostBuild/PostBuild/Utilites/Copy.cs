/*
<License>
Copyright 2015 Virtium Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http ://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
</License>
 */

using System;
using System.IO;

namespace PostBuild.Utilites
{
    static class Copy
    {
        #region Public Functions
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true, bool overwrite = false)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dirSource = new DirectoryInfo(sourceDirName);

            if (false == dirSource.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            // If the destination directory doesn't exist, create it.
            if (false == Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            PrintCopyTo(sourceDirName, destDirName);

            // Get the files in the directory and copy them to the new location.
            foreach (FileInfo file in dirSource.GetFiles())
            {
                FileCopy(file.Name, destDirName, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (true == copySubDirs)
            {
                foreach (DirectoryInfo subDir in dirSource.GetDirectories())
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    DirectoryCopy(subDir.FullName, tempPath, copySubDirs, overwrite);
                }
            }
        }

        public static void FileCopy(string sourceFile, string targetPath, bool overwrite = false)
        {
            // Create a new target folder, if necessary.
            if (false == System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            FileInfo fileInfo = new FileInfo(sourceFile);
            string destFilePath = System.IO.Path.Combine(targetPath, fileInfo.Name);

            PrintCopyTo(sourceFile, destFilePath);

            if (true == File.Exists(destFilePath))
            {
                if (true == overwrite)
                {
                    System.IO.File.Copy(sourceFile, destFilePath, true);
                    Console.WriteLine(Indent(3) + String.Format("WARINING: {0} has already existed in {1}. File is overwritten.", fileInfo.Name, destFilePath));
                }
                else
                {
                    Console.WriteLine(Indent(3) + String.Format("WARINING: {0} has already existed in {1}. File does not overwrite.", fileInfo.Name, destFilePath));
                }
            }
            else
            {
                System.IO.File.Copy(sourceFile, destFilePath);
            }
        }
        #endregion

        #region Private Function
        private static string GetRelativePath(string fullPath)
        {
            string currentDir = Directory.GetCurrentDirectory();
            return fullPath.Replace(currentDir, "").TrimStart(Path.DirectorySeparatorChar);
        }

        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }

        private static void PrintCopyTo(string source, string destination)
        {
            Console.WriteLine(Indent(3) + String.Format("Copy {0} to {1}", GetRelativePath(source), destination));
        }
        #endregion
    }
}
