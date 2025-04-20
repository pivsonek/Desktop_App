using Microsoft.Maui.Controls;
using project.Models;
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
    public void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (BindingContext is GraphModel graph)
            MainPage.Instance?.OnExportGraphClicked(sender, e);
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
