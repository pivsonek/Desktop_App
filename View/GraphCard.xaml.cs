using ClosedXML.Excel;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using project.Models;
using project.Services;

namespace project.View;

/// <summary>
/// Represents a single chart UI component.
/// </summary>
public partial class GraphCard : ContentView
{
    public GraphCard()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the event to toggle the graph expansion.
    /// </summary>
    public void OnResizeGraphClicked(object sender, EventArgs e)
    {
        if (BindingContext is GraphModel graph)
            MainPage.Instance?.OnResizeGraphClicked(sender, e);
    }

    /// <summary>
    /// Handles the export of the current graph to an Excel file.
    /// </summary>
    public async void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph && MainPage.Instance?.SelectedTab?.FilteredData != null)
        {
            try
            {
                var chartData = GetChartDataPoints(graph);
                if (chartData == null || chartData.Count == 0)
                {
                    await MainPage.Instance.DisplayAlert("Chyba", "Žádná data k exportu.", "OK");
                    return;
                }

                var filePath = await GetSaveFilePath(graph.Name);
                if (string.IsNullOrEmpty(filePath)) return;

                await ExportToExcel(filePath, graph, chartData);

                await MainPage.Instance.DisplayAlert("Úspěch", $"Graf '{graph.Name}' byl exportován do souboru: {filePath}", "OK");
            }
            catch (Exception ex)
            {
                await MainPage.Instance.DisplayAlert("Chyba", $"Nelze exportovat graf: {ex.Message}", "OK");
            }
        }
        else
        {
            await (MainPage.Instance?.DisplayAlert("Chyba", "Nelze exportovat graf.", "OK") ?? Task.CompletedTask);

        }
    }

    /// <summary>
    /// Extracts data points from the graph's series.
    /// </summary>
    private List<(double X, double Y)> GetChartDataPoints(GraphModel graph)
    {
        var result = new List<(double X, double Y)>();

        if (graph.Series == null || graph.Series.Length == 0 ||
            graph.Series[0] is not LineSeries<ObservablePoint> lineSeries ||
            lineSeries.Values == null)
            return result;

        var points = lineSeries.Values.Cast<ObservablePoint>();
        foreach (var point in points)
        {
            if (point.X.HasValue && point.Y.HasValue)
            {
                double x = point.X.Value;
                if (MainPage.Instance?.SelectedTab?.FilteredData?.FilterType == "temperature")
                    x = Math.Pow(10, x);

                result.Add((x, point.Y.Value));
            }
        }

        return result;
    }

    /// <summary>
    /// Opens the save file dialog and returns the selected path.
    /// </summary>
    private async Task<string?> GetSaveFilePath(string defaultFileName)
    {
        try
        {
            var fileSaver = new ExcelFileSaver
            {
                InitialFileName = $"{defaultFileName.Trim()}.xlsx",
                FileExtension = "xlsx"
            };

            var result = await fileSaver.SaveAsync();
            return result?.FilePath;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Writes the graph's data into an Excel file at the given path.
    /// </summary>
    private async Task ExportToExcel(string filePath, GraphModel graph, List<(double X, double Y)> chartData)
    {
        await Task.Run(() =>
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(graph.Name.Trim());

                string xHeader = MainPage.Instance?.SelectedTab?.FilteredData?.FilterType == "temperature"
                    ? "Frequency [Hz]"
                    : "Temperature [°C]";
                string yHeader = graph.SelectedKeyY ?? "Value";

                worksheet.Cell(1, 1).Value = xHeader;
                worksheet.Cell(1, 2).Value = yHeader;

                worksheet.Row(1).Style.Font.Bold = true;

                for (int i = 0; i < chartData.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = chartData[i].X;
                    worksheet.Cell(i + 2, 2).Value = chartData[i].Y;
                }

                worksheet.Column(1).AdjustToContents();
                worksheet.Column(2).AdjustToContents();

                workbook.SaveAs(filePath);
            }
        });
    }

    /// <summary>
    /// Displays a menu for selecting a new Y-axis key.
    /// </summary>
    public async void OnShowYAxisMenuClicked(object sender, EventArgs e)
    {
        if (BindingContext is not GraphModel graph || graph.AvailableYKeys is null)
            return;

        var options = graph.SelectedKeyY == null
            ? graph.AvailableYKeys.ToArray()
            : graph.AvailableYKeys.Where(k => k != graph.SelectedKeyY).ToArray();

        if (options.Length == 0) return;

        var selected = await Shell.Current.DisplayActionSheet("Osa Y", "Zrušit", null, options);

        if (!string.IsNullOrEmpty(selected) && selected != "Zrušit")
            graph.SelectedKeyY = selected;
    }
}
