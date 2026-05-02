// <copyright file="MainViewModel.cs" company="LocalAIWorkspace">
// Copyright (c) LocalAIWorkspace. All rights reserved.
// </copyright>

namespace LocalAIWorkspace.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using DiffPlex;
    using DiffPlex.DiffBuilder;
    using DiffPlex.DiffBuilder.Model;
    using LocalAIWorkspace.Models;
    using LocalAIWorkspace.Services;

    /// <summary>
    /// ViewModel for the main window.
    /// Handles file loading, explaining, and comparing.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly FileService fileService = new();
        private readonly AiService aiService = new();

        private string folderAPath = string.Empty;
        private string folderBPath = string.Empty;
        private string fileContent = string.Empty;
        private CodeFile? selectedFile;
        private string aiResponse = string.Empty;

        /// <summary>
        /// Event fired when a property changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets Version A files.
        /// </summary>
        public ObservableCollection<CodeFile> Files { get; set; } = new();

        /// <summary>
        /// Gets or sets Version B files.
        /// </summary>
        public List<CodeFile> FilesB { get; set; } = new();

        /// <summary>
        /// Gets or sets Folder A path.
        /// </summary>
        public string FolderAPath
        {
            get => this.folderAPath;
            set
            {
                this.folderAPath = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets Folder B path.
        /// </summary>
        public string FolderBPath
        {
            get => this.folderBPath;
            set
            {
                this.folderBPath = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets selected file.
        /// </summary>
        public CodeFile? SelectedFile
        {
            get => this.selectedFile;
            set
            {
                this.selectedFile = value;
                this.OnPropertyChanged();
                this.LoadFile();
            }
        }

        /// <summary>
        /// Gets or sets file content.
        /// </summary>
        public string FileContent
        {
            get => this.fileContent;
            set
            {
                this.fileContent = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets AI response.
        /// </summary>
        public string AiResponse
        {
            get => this.aiResponse;
            set
            {
                this.aiResponse = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Loads Version A files.
        /// </summary>
        /// <param name="path">The path to the folder containing Version A files.</param>
        public void LoadFolder(string path)
        {
            this.FolderAPath = path;

            this.Files.Clear();
            foreach (var file in this.fileService.GetCodeFiles(path))
            {
                this.Files.Add(file);
            }
        }

        /// <summary>
        /// Loads Version B files.
        /// </summary>
        /// <param name="folder">The path to the folder containing Version B files.</param>
        public void LoadFolderB(string folder)
        {
            this.FolderBPath = folder;

            this.FilesB = this.fileService.GetCodeFiles(folder);
            this.OnPropertyChanged(nameof(this.FilesB));
        }

        /// <summary>
        /// Swaps Version A and Version B.
        /// </summary>
        public void SwapVersions()
        {
            var tempFiles = this.Files.ToList();

            this.Files.Clear();
            foreach (var file in this.FilesB)
            {
                this.Files.Add(file);
            }

            this.FilesB = tempFiles;

            var tempPath = this.FolderAPath;
            this.FolderAPath = this.FolderBPath;
            this.FolderBPath = tempPath;

            this.OnPropertyChanged(nameof(this.Files));
            this.OnPropertyChanged(nameof(this.FilesB));
        }

        /// <summary>
        /// Explains project structure.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Explain()
        {
            if (this.Files.Count == 0)
            {
                return;
            }

            var context = new StringBuilder();

            foreach (var file in this.Files.Take(5))
            {
                var content = this.fileService.ReadFile(file.FullPath);

                context.AppendLine($"FILE: {file.Name}");
                context.AppendLine(content);
                context.AppendLine("\n----------------------\n");
            }

            this.AiResponse = await this.aiService.AskAsync(
                "Explain the structure and purpose of this C# project.\n\n" +
                "Focus on architecture, flow, and relationships.\n\n" +
                context.ToString());
        }

        /// <summary>
        /// Compares Version A and Version B using DiffPlex and AI analysis.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Compare()
        {
            if (this.Files.Count == 0 || this.FilesB.Count == 0)
            {
                this.AiResponse = "Load both Version A and Version B folders.";
                return;
            }

            var context = new StringBuilder();
            var collectedChanges = new List<CodeChange>();

            var diffBuilder = new SideBySideDiffBuilder(new Differ());

            foreach (var fileA in this.Files.Take(8))
            {
                var match = this.FilesB.FirstOrDefault(f => f.Name == fileA.Name);

                if (match == null)
                {
                    continue;
                }

                var contentA = this.fileService.ReadFile(fileA.FullPath);
                var contentB = this.fileService.ReadFile(match.FullPath);

                var diff = diffBuilder.BuildDiffModel(contentA, contentB);

                var changes = diff.NewText.Lines
                    .Where(line => line.Type != ChangeType.Unchanged)
                    .Take(30)
                    .ToList();

                if (!changes.Any())
                {
                    continue;
                }

                context.AppendLine($"FILE: {fileA.Name}");

                foreach (var line in changes)
                {
                    var prefix = line.Type switch
                    {
                        ChangeType.Inserted => "+",
                        ChangeType.Deleted => "-",
                        ChangeType.Modified => "*",
                        _ => " ",
                    };

                    var text = line.Text?.Trim() ?? string.Empty;

                    context.AppendLine($"{prefix} {text}");

                    collectedChanges.Add(new CodeChange
                    {
                        File = fileA.Name,
                        Type = line.Type,
                        Line = text,
                    });
                }

                context.AppendLine("\n----------------------\n");
            }

            if (context.Length == 0)
            {
                this.AiResponse = "No differences found.";
                return;
            }

            foreach (var change in collectedChanges)
            {
                change.MethodKey = this.GetMethodKey(change.Line);
            }

            // ---------- BUILD STRUCTURED DIFF (NO AI) ----------
            var rawBuilder = new StringBuilder();

            foreach (var change in collectedChanges)
            {
                rawBuilder.AppendLine("- Change:");
                rawBuilder.AppendLine($"  Evidence: {change.Line}");
                rawBuilder.AppendLine();
            }

            var raw = rawBuilder.ToString();

            var insights = new StringBuilder();
            var explanation = await this.aiService.AskAsync(
                "You are describing individual C# code lines.\n\n" +

                "STRICT RULES:\n" +
                "- Describe ONLY what the line literally does\n" +
                "- DO NOT infer meaning\n" +
                "- DO NOT summarize multiple lines\n" +
                "- DO NOT compare to other lines\n" +
                "- DO NOT assume return values represent balances or states\n" +
                "- DO NOT use words like 'allows', 'ensures', 'repeats', or 'similar'\n\n" +

                "REQUIREMENTS:\n" +
                "- Treat every line independently\n" +
                "- Use exact method names\n" +
                "- Include parentheses if the line represents a method call\n\n" +

                "FORMAT:\n" +
                "- Change:\n" +
                "  Evidence: <line>\n" +
                "  Explanation: <exact behavior of the line>\n\n" +

                raw);

            var duplicateCalls = collectedChanges
                .Where(c => c.Line.Contains("SellFood"))
                .GroupBy(c => c.Line)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicateCalls)
            {
                insights.AppendLine($"⚠ Duplicate call: {group.Key}");
            }

            var unusedReturns = collectedChanges
                .Where(c => c.Line.Contains("SellFood(") && !c.Line.Contains("="));

            foreach (var item in unusedReturns)
            {
                insights.AppendLine($"⚠ Unused return: {item.Line}");
            }

            var duplicateVariables = collectedChanges
                .Where(c => c.Line.StartsWith("decimal") || c.Line.StartsWith("double"))
                .GroupBy(c => c.Line)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicateVariables)
            {
                insights.AppendLine($"⚠ Duplicate variable: {group.Key}");
            }

            var grouped = collectedChanges
                .GroupBy(c => c.MethodKey)
                .Where(g => g.Key != "GLOBAL");

            if (grouped.Any())
            {
                insights.AppendLine("\n--- Grouped by Method ---");

                foreach (var group in grouped)
                {
                    insights.AppendLine($"Method: {group.Key}");

                    foreach (var item in group)
                    {
                        insights.AppendLine($"  - {item.Line}");
                    }
                }
            }

            this.AiResponse = explanation;

            if (insights.Length > 0)
            {
                this.AiResponse += "\n\n--- Analysis ---\n" + insights.ToString();
            }
        }

        /// <summary>
        /// Raises PropertyChanged.
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Attempts to determine a grouping key for a line of code.
        /// </summary>
        /// <param name="line">The line of code.</param>
        /// <returns>A method key or GLOBAL if none found.</returns>
        private string GetMethodKey(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return "GLOBAL";
            }

            if (line.Contains("(") && line.Contains(")") && !line.Contains(";"))
            {
                return line;
            }

            if (line.Contains("this."))
            {
                var index = line.IndexOf("this.");
                return line.Substring(index).Split('(')[0];
            }

            return "GLOBAL";
        }

        /// <summary>
        /// Loads selected file content.
        /// </summary>
        private void LoadFile()
        {
            if (this.SelectedFile == null)
            {
                return;
            }

            this.FileContent = this.fileService.ReadFile(this.SelectedFile.FullPath);
        }
    }
}