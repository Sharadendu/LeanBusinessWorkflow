using System;
using System.Collections.Generic;

namespace LeanBusinessWorkflow.Core.Models;

/// <summary>
/// Represents an Obsidian-style note (markdown file with optional YAML frontmatter)
/// </summary>
public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Dictionary<string, object> Frontmatter { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<string> Tags { get; set; } = new();
}