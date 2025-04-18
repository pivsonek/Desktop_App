using Microsoft.Maui.Controls;
using project.Models;
using System;

namespace project.View;

public partial class GraphCard : ContentView
{
    public GraphCard()
    {
        InitializeComponent();
    }

    private void OnResizeGraphClicked(object sender, EventArgs e)
    {
        if (BindingContext is GraphModel graph)
            MainPage.Instance?.OnResizeGraphClicked(sender, e);
    }

    private void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (BindingContext is GraphModel graph)
            MainPage.Instance?.OnExportGraphClicked(sender, e);
    }

    private async void OnShowYAxisMenuClicked(object sender, EventArgs e)
    {
        if (BindingContext is not GraphModel graph || graph.AvailableYKeys is null)
            return;

        var options = graph.AvailableYKeys
            .Where(k => k != graph.SelectedKeyY)
            .ToArray();

        if (options.Length == 0) return;

        var selected = await Application.Current.MainPage.DisplayActionSheet(
            "Osa Y", "Zrušit", null, options);

        if (!string.IsNullOrEmpty(selected) && selected != "Zrušit")
        {
            graph.SelectedKeyY = selected;
            MainPage.Instance?.RenderGraph(graph);
        }
    }


    
}