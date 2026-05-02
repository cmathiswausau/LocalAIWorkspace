// <copyright file="CodeChange.cs" company="LocalAIWorkspace">
// Copyright (c) LocalAIWorkspace. All rights reserved.
// </copyright>

namespace LocalAIWorkspace.Models
{
    using DiffPlex.DiffBuilder.Model;

    /// <summary>
    /// Represents a single code change from a diff.
    /// </summary>
    public class CodeChange
    {
        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string File { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the change type.
        /// </summary>
        public ChangeType Type { get; set; }

        /// <summary>
        /// Gets or sets the line content.
        /// </summary>
        public string Line { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the associated method key.
        /// </summary>
        public string MethodKey { get; set; } = string.Empty;
    }
}