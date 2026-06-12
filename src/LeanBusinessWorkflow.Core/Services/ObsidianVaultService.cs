using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LeanBusinessWorkflow.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LeanBusinessWorkflow.Core.Services;

/// <summary>
/// Obsidian-style vault service for managing markdown notes with YAML frontmatter.
/// </summary>
public class ObsidianVaultService
{
    private readonly string _vaultPath;
    private readonly ISerializer _yamlSerializer;
    private readonly IDeserializer _yamlDeserializer;

    public ObsidianVaultService(string vaultPath)
    {
        _vaultPath = Path.GetFullPath(vaultPath);
        Directory.CreateDirectory(_vaultPath);

        _yamlSerializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    public string VaultPath => _vaultPath;

    /// <summary>
    /// Saves or updates a note as a markdown file with YAML frontmatter.
    /// </summary>
    public void SaveNote(Note note)
    {
        if (string.IsNullOrWhiteSpace(note.Title))
            throw new ArgumentException("Note title cannot be empty.");

        note.UpdatedAt = DateTime.UtcNow;

        string fileName = $"{SanitizeFileName(note.Title)}.md";
        string filePath = Path.Combine(_vaultPath, fileName);
        note.FilePath = filePath;

        var frontmatter = new Dictionary<string, object>
        {
            ["id"] = note.Id,
            ["title"] = note.Title,
            ["created"] = note.CreatedAt.ToString("o"),
            ["updated"] = note.UpdatedAt.ToString("o"),
            ["tags"] = note.Tags
        };

        foreach (var kvp in note.Frontmatter)
        {
            frontmatter[kvp.Key] = kvp.Value;
        }

        string yaml = _yamlSerializer.Serialize(frontmatter);
        string content = $"---\n{yaml}---\n\n{note.Content}";

        File.WriteAllText(filePath, content, Encoding.UTF8);
    }

    /// <summary>
    /// Loads all notes from the vault.
    /// </summary>
    public List<Note> GetAllNotes()
    {
        var notes = new List<Note>();
        var files = Directory.GetFiles(_vaultPath, "*.md", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            try
            {
                var note = LoadNoteFromFile(file);
                if (note != null) notes.Add(note);
            }
            catch
            {
                // Skip corrupted files
            }
        }

        return notes.OrderByDescending(n => n.UpdatedAt).ToList();
    }

    public Note? GetNoteById(string id)
    {
        return GetAllNotes().FirstOrDefault(n => n.Id == id);
    }

    public List<Note> SearchNotes(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAllNotes();

        query = query.ToLowerInvariant();

        return GetAllNotes()
            .Where(n =>
                n.Title.ToLowerInvariant().Contains(query) ||
                n.Content.ToLowerInvariant().Contains(query) ||
                n.Tags.Any(t => t.ToLowerInvariant().Contains(query)))
            .ToList();
    }

    private Note? LoadNoteFromFile(string filePath)
    {
        string content = File.ReadAllText(filePath, Encoding.UTF8);

        var match = Regex.Match(content, @"^---\s*\r?\n(.*?)\r?\n---\s*\r?\n(.*)$", RegexOptions.Singleline);
        if (!match.Success) return null;

        string yaml = match.Groups[1].Value;
        string body = match.Groups[2].Value.TrimStart();

        var frontmatter = _yamlDeserializer.Deserialize<Dictionary<string, object>>(yaml);

        var note = new Note
        {
            FilePath = filePath,
            Content = body
        };

        if (frontmatter.TryGetValue("id", out var id)) note.Id = id.ToString()!;
        if (frontmatter.TryGetValue("title", out var title)) note.Title = title.ToString()!;
        if (frontmatter.TryGetValue("created", out var created)) note.CreatedAt = DateTime.Parse(created.ToString()!);
        if (frontmatter.TryGetValue("updated", out var updated)) note.UpdatedAt = DateTime.Parse(updated.ToString()!);
        if (frontmatter.TryGetValue("tags", out var tags) && tags is List<object> tagList)
            note.Tags = tagList.Select(t => t.ToString()!).ToList();

        // Store remaining frontmatter
        foreach (var kvp in frontmatter)
        {
            if (!new[] { "id", "title", "created", "updated", "tags" }.Contains(kvp.Key))
                note.Frontmatter[kvp.Key] = kvp.Value;
        }

        return note;
    }

    private static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}