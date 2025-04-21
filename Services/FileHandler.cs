using Microsoft.Maui.Storage;
using System;
using System.Threading.Tasks;

namespace project.Services;

/// <summary>
/// Provides functionality to handle file selection and reading in a MAUI application.
/// </summary>
public class FileHandler
{
    /// <summary>
    /// Opens a file picker dialog and returns the full path to the selected file.
    /// </summary>
    /// <returns>The full path to the selected file or null if no file was selected.</returns>
    public async Task<string?> PickFileAsync()
    {
        var result = await FilePicker.PickAsync();
        return result?.FullPath;
    }

    /// <summary>
    /// Reads the content of a file at the specified path.
    /// </summary>
    /// <param name="filePath">The path to the file to read.</param>
    /// <returns>The content of the file as a string, or null if the path is invalid.</returns>
    public async Task<string?> ReadFileContentAsync(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath))
        {
            return await File.ReadAllTextAsync(filePath);
        }
        return null;
    }
}
