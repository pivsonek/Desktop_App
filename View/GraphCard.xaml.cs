using ClosedXML.Excel;
using CommunityToolkit.Maui.Storage;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Maui.Controls;
using project.Models;
using project.Services;
using System;
using System.Linq;

namespace project.View;
/// <summary>
/// Třída reprezentující graf v uživatelském rozhraní.
/// </summary>
public partial class GraphCard : ContentView
{
    public GraphCard()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Událost pro zmenšení grafu.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnResizeGraphClicked(object sender, EventArgs e)
    {
        if (BindingContext is GraphModel graph)
            MainPage.Instance?.OnResizeGraphClicked(sender, e);
    }

    /// <summary>
    /// Událost pro odstranění grafu.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph && MainPage.Instance.SelectedTab?.FilteredData != null)
        {
            try
            {
                // Get the data from the chart
                var chartData = GetChartDataPoints(graph);
                if (chartData == null || chartData.Count == 0)
                {
                    await MainPage.Instance?.DisplayAlert("Chyba", "Žádná data k exportu.", "OK");
                    return;
                }

                // Ask the user where to save the file
                var filePath = await GetSaveFilePath(graph.Name);
                if (string.IsNullOrEmpty(filePath))
                    return; // User cancelled

                // Export the data to Excel
                await ExportToExcel(filePath, graph, chartData);

                await MainPage.Instance?.DisplayAlert("Úspěch", $"Graf '{graph.Name}' byl exportován do souboru: {filePath}", "OK");
            }
            catch (Exception ex)
            {
                await MainPage.Instance?.DisplayAlert("Chyba", $"Nepodařilo se exportovat graf: {ex.Message}", "OK");
            }
        }
        else
        {
            await MainPage.Instance?.DisplayAlert("Chyba", "Nelze exportovat graf.", "OK");
        }
    }

    private List<(double X, double Y)> GetChartDataPoints(GraphModel graph)
    {
        var result = new List<(double X, double Y)>();

        // Check if the chart has a valid series
        if (graph.Series == null || graph.Series.Length == 0 ||
            graph.Series[0] is not LineSeries<ObservablePoint> lineSeries ||
            lineSeries.Values == null)
            return result;

        // Extract data points from the series
        var points = lineSeries.Values.Cast<ObservablePoint>();
        foreach (var point in points)
        {
            if (point.X.HasValue && point.Y.HasValue)
            {
                double x = point.X.Value;
                // If X is logarithmic (frequency), convert it back to the original value
                if (MainPage.Instance?.SelectedTab?.FilteredData?.FilterType == "temperature")
                    x = Math.Pow(10, x); // Convert back from log10

                result.Add((x, point.Y.Value));
            }
        }

        return result;
    }

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
            return result.FilePath;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task ExportToExcel(string filePath, GraphModel graph, List<(double X, double Y)> chartData)
    {
        await Task.Run(() =>
        {
            using (var workbook = new XLWorkbook())
            {
                // Create a worksheet
                var worksheet = workbook.Worksheets.Add(graph.Name);

                // Add headers
                string xHeader = MainPage.Instance?.SelectedTab.FilteredData.FilterType == "temperature" ? "Frequency [Hz]" : "Temperature [°C]";
                string yHeader = graph.SelectedKeyY ?? "Value";

                worksheet.Cell(1, 1).Value = xHeader;
                worksheet.Cell(1, 2).Value = yHeader;

                // Bold headers
                worksheet.Row(1).Style.Font.Bold = true;

                // Add data points
                for (int i = 0; i < chartData.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = chartData[i].X;
                    worksheet.Cell(i + 2, 2).Value = chartData[i].Y;
                }

                // Autofit columns
                worksheet.Column(1).AdjustToContents();
                worksheet.Column(2).AdjustToContents();

                // Save the workbook
                workbook.SaveAs(filePath);
            }
        });
    }

    /// <summary>
    /// Událost pro zobrazení nabídky pro výběr klíče osy Y.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
