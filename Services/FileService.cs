// <copyright file="FileService.cs" company="LocalAIWorkspace">
// Copyright (c) LocalAIWorkspace. All rights reserved.
// </copyright>
namespace LocalAIWorkspace.Services
{
    using System.Collections.Generic;
    using System.IO;
    using LocalAIWorkspace.Models;

    /// <summary>
    /// Service for handling file operations, such as retrieving code files from a specified folder and reading file contents.
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// Retrieves all C# source files from the specified folder and its subdirectories, excluding common build and
        /// generated files.
        /// </summary>
        /// <remarks>Files located in 'bin' and 'obj' directories, as well as common generated files such
        /// as 'AssemblyInfo.cs', 'GlobalUsings.g.cs', files ending with '.Designer.cs' or '.g.cs', and 'App.xaml.cs',
        /// are excluded from the results.</remarks>
        /// <param name="folder">The path to the root folder to search for C# source files. Must not be null or empty.</param>
        /// <returns>A list of CodeFile objects representing the discovered C# source files. The list is empty if the folder does
        /// not exist or contains no matching files.</returns>
        public List<CodeFile> GetCodeFiles(string folder)
        {
            var result = new List<CodeFile>();

            if (!Directory.Exists(folder))
            {
                return result;
            }

            foreach (var file in Directory.GetFiles(folder, "*.cs", SearchOption.AllDirectories))
            {
                // Skip build folders
                if (file.Contains(@"\bin\") || file.Contains(@"\obj\"))
                {
                    continue;
                }

                var fileName = Path.GetFileName(file);

                // Skip generated files
                if (fileName == "AssemblyInfo.cs" ||
                    fileName == "GlobalUsings.g.cs" ||
                    fileName.EndsWith(".Designer.cs") ||
                    fileName.EndsWith(".g.cs") ||
                    fileName == "App.xaml.cs")
                {
                    continue;
                }

                result.Add(new CodeFile
                {
                    Name = fileName,
                    FullPath = file,
                });
            }

            return result;
        }

        /// <summary>
        /// Reads the contents of the specified file and returns it as a string.
        /// </summary>
        /// <param name="path">The path to the file to read. The path can be relative or absolute.</param>
        /// <returns>A string containing the contents of the file if it exists; otherwise, an empty string.</returns>
        public string ReadFile(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }
    }
}
