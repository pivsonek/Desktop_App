using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui.Storage;

namespace project.Services;

public class ExcelFileSaver
{
    public string InitialFileName { get; set; } = "export.xlsx";
    public string FileExtension { get; set; } = "xlsx";

    public async Task<FileSaverResult?> SaveAsync()
    {
        // On desktop platforms, we can use the FileSaver API
        var result = await FileSaver.Default.SaveAsync(InitialFileName,
            new MemoryStream(), // We're using an empty stream here, as we'll save the file directly with ClosedXML
            CancellationToken.None);

        return new project.Services.FileSaverResult
        {
            IsSuccessful = result.IsSuccessful,
            FilePath = result.FilePath
        };
    }
}

// Add this to handle the result of the file saver
public class FileSaverResult
{
    public bool IsSuccessful { get; set; }
    public string? FilePath { get; set; }
}