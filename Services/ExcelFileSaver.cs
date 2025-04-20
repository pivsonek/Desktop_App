using CommunityToolkit.Maui.Storage;

namespace project.Services;

/// <summary>
/// Provides functionality to let the user choose a path for saving Excel files.
/// </summary>
public class ExcelFileSaver
{
    /// <summary>
    /// The default name used in the file picker.
    /// </summary>
    public string InitialFileName { get; set; } = "export.xlsx";

    /// <summary>
    /// The file extension used for the Excel file.
    /// </summary>
    public string FileExtension { get; set; } = "xlsx";

    /// <summary>
    /// Opens the platform file picker for saving a file and returns the selected path.
    /// </summary>
    /// <returns>FileSaverResult with status and path info.</returns>
    public async Task<FileSaverResult?> SaveAsync()
    {
        var result = await FileSaver.Default.SaveAsync(InitialFileName,
            new MemoryStream(),
            CancellationToken.None);

        return new FileSaverResult
        {
            IsSuccessful = result.IsSuccessful,
            FilePath = result.FilePath
        };
    }
}

/// <summary>
/// Represents the result of a file saving operation.
/// </summary>
public class FileSaverResult
{
    /// <summary>
    /// Indicates whether the file saving was successful.
    /// </summary>
    public bool IsSuccessful { get; set; }

    /// <summary>
    /// The full file path chosen by the user, or null if canceled.
    /// </summary>
    public string? FilePath { get; set; }
}
