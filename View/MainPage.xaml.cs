// Using statements – základní importy
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.ComponentModel;
using project.Services;
using project.Managers;
using project.Models;
using project.Converters;
using System.Globalization;


namespace project.View;

/// <summary>
/// Hlavní stránka aplikace, která obsahuje UI a logiku pro zpracování souborů a grafů.
/// </summary>
public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    // Notifikace změny property – nutná pro binding
    public new event PropertyChangedEventHandler? PropertyChanged;
    private new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Instance správce grafů a správce souborů
    private GraphManager _graphManager = new();
    private FileHandler _fileHandler = new();

    // Kolekce záložek (načtených souborů)
    private ObservableCollection<GraphTab> _tabs = new();
    public ObservableCollection<GraphTab> Tabs
    {
        get => _tabs;
        set
        {
            if (_tabs != value)
            {
                _tabs = value;
                OnPropertyChanged(nameof(Tabs));
                OnPropertyChanged(nameof(Tabs.Count));
            }
        }
    }

    // Aktuálně vybraná záložka
    private GraphTab? _selectedTab;
    public GraphTab? SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (_selectedTab != value)
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
                OnPropertyChanged(nameof(SelectedTab.Graphs));
            }
        }
    }

    // Kolekce všech grafů (propojeno se správcem grafů)
    public ObservableCollection<GraphModel> Graphs { get; set; }

    // Návrhy do dropdownů pro hledání teploty/frekvence
    public ObservableCollection<string> TemperatureSuggestions { get; set; } = new();
    public ObservableCollection<string> FrequencySuggestions { get; set; } = new();

    // Zda je některý graf zvětšený – pro UI logiku
    private bool _isAnyGraphExpanded;
    public bool IsAnyGraphExpanded
    {
        get => _isAnyGraphExpanded;
        set
        {
            if (_isAnyGraphExpanded != value)
            {
                _isAnyGraphExpanded = value;
                OnPropertyChanged(nameof(IsAnyGraphExpanded));
            }
        }
    }

    // Lockování vstupních polí (teplota vs. frekvence)
    private bool _isFrequencyEnabled = true;
    private bool _isTemperatureEnabled = true;

    public bool IsFrequencyEnabled
    {
        get => _isFrequencyEnabled;
        set
        {
            _isFrequencyEnabled = value;
            OnPropertyChanged(nameof(IsFrequencyEnabled));
        }
    }

    public bool IsTemperatureEnabled
    {
        get => _isTemperatureEnabled;
        set
        {
            _isTemperatureEnabled = value;
            OnPropertyChanged(nameof(IsTemperatureEnabled));
        }
    }

    // Statická instance MainPage – dá se na ni odkázat z jiných tříd
    public static MainPage? Instance { get; private set; }

    /// <summary>
    /// Konstruktor – inicializace bindingu a eventů
    /// </summary>
    public MainPage()
    {
        Instance = this;
        InitializeComponent();
        Graphs = _graphManager.Graphs;
        BindingContext = this;

        Tabs.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(Tabs));
            OnPropertyChanged(nameof(Tabs.Count));
        };
    }

    /// <summary>
    /// Tlačítko načíst soubor – vyvolá dialog, načte data, vytvoří záložku
    /// </summary>
    private async void OnLoadFileClicked(object sender, EventArgs e)
    {
        string? filePath = await _fileHandler.PickFileAsync();
        if (!string.IsNullOrEmpty(filePath))
        {
            var newTab = new GraphTab() { FileName = System.IO.Path.GetFileName(filePath) };
            bool loaded = await newTab.MeasureData.LoadData(filePath);
            if (!loaded)
            {
                await DisplayAlert("Chyba", "Nepodařilo se načíst data ze souboru.", "OK");
                return;
            }

            // Získání náhledu dat
            var lines = await newTab.MeasureData.MakeToStringAsync(100);
            newTab.DisplayData.Clear();
            foreach (var line in lines)
                newTab.DisplayData.Add(line);

            // Jeden výchozí graf
            newTab.Graphs.Add(new GraphModel() { Name = "Graf 1" });

            Tabs.Add(newTab);
            SelectedTab = newTab;
        }
        RecalculateGraphSizes();
    }

    /// <summary>
    /// Přidání nového grafu do aktivní záložky
    /// </summary>
    private void OnAddGraphClicked(object sender, EventArgs e)
    {
        if (SelectedTab != null)
        {
            int nextNumber = SelectedTab.Graphs.Count + 1;
            SelectedTab.Graphs.Add(new GraphModel { Name = $"Graf {nextNumber}" });
            UpdateGraphVisibility();
            RecalculateGraphSizes();
        }
    }

    /// <summary>
    /// Změna stavu zvětšení u grafu (expand/collapse)
    /// </summary>
    private void OnResizeGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph)
        {
            if (SelectedTab == null) return;

            if (graph.IsExpanded)
                graph.IsExpanded = false;
            else
            {
                foreach (var g in SelectedTab.Graphs)
                    g.IsExpanded = false;

                graph.IsExpanded = true;
            }

            UpdateGraphVisibility();
            OnPropertyChanged(nameof(SelectedTab));
            RecalculateGraphSizes();
        }
    }

    /// <summary>
    /// Nastaví které grafy budou vidět – pokud je jeden expandnutý, ostatní se skryjí
    /// </summary>
    private void UpdateGraphVisibility()
    {
        if (SelectedTab == null) return;

        IsAnyGraphExpanded = SelectedTab.Graphs.Any(g => g.IsExpanded);

        foreach (var g in SelectedTab.Graphs)
            g.IsVisible = !IsAnyGraphExpanded || g.IsExpanded;

        OnPropertyChanged(nameof(IsAnyGraphExpanded));
        OnPropertyChanged(nameof(SelectedTab));
    }

    /// <summary>
    /// Simulovaný export grafu – aktuálně jen Alert
    /// </summary>
    private async void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph)
            await DisplayAlert("Export", $"Exportuji graf: {graph.Name}", "OK");
        else
            await DisplayAlert("Chyba", "Nelze exportovat graf.", "OK");
    }

    /// <summary>
    /// Změna textu v teplotním políčku – zapíná/vypíná druhé pole + suggestions
    /// </summary>
    private void OnTemperatureTextChanged(object sender, TextChangedEventArgs e)
    {
        IsFrequencyEnabled = string.IsNullOrWhiteSpace(e.NewTextValue);
        UpdateTemperatureSuggestions(e.NewTextValue);
    }

    /// <summary>
    /// Změna textu ve frekvenčním políčku – zapíná/vypíná druhé pole + suggestions
    /// </summary>
    private void OnFrequencyTextChanged(object sender, TextChangedEventArgs e)
    {
        IsTemperatureEnabled = string.IsNullOrWhiteSpace(e.NewTextValue);
        UpdateFrequencySuggestions(e.NewTextValue);
    }

    /// <summary>
    /// Zpracování hledání podle teploty – vytvoří `FilteredData` a vygeneruje výpis
    /// </summary>
    private async void OnTemperatureSearch(object sender, EventArgs e)
    {
        if (SelectedTab == null || TemperatureSearchBar == null) return;

        if (double.TryParse(TemperatureSearchBar.Text, out double temp))
        {
            SelectedTab.FilteredData = new FilteredData("temperature", temp, SelectedTab.MeasureData);
            var lines = await SelectedTab.FilteredData.MakeToStringAsync(0);

            SelectedTab.FilteredDisplayData.Clear();
            foreach (var line in lines)
                SelectedTab.FilteredDisplayData.Add(line);
        }
    }

    /// <summary>
    /// Zpracování hledání podle frekvence – pozor na parsing
    /// </summary>
    private async void OnFrequencySearch(object sender, EventArgs e)
    {
        if (SelectedTab == null || FrequencySearchBar == null) return;

        // Důležité! Nahradí čárku tečkou a použije invariantní kulturu
        string input = FrequencySearchBar.Text?.Replace(",", ".") ?? "";

        if (double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out double freq))
        {
            SelectedTab.FilteredData = new FilteredData("frequency", freq, SelectedTab.MeasureData);
            var lines = await SelectedTab.FilteredData.MakeToStringAsync(0);

            SelectedTab.FilteredDisplayData.Clear();
            foreach (var line in lines)
                SelectedTab.FilteredDisplayData.Add(line);
        }
    }

    /// <summary>
    /// Nápověda – zobrazí Alert
    /// </summary>
    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Nápověda",
            "Tato aplikace umožňuje analyzovat data na základě teploty a frekvence.\n\n" +
            "1. Zadejte teplotu nebo frekvenci.\n" +
            "2. Druhá hodnota se automaticky zamkne.\n" +
            "3. Klikněte na tlačítko hledání pro analýzu.\n" +
            "4. Můžete přidávat a upravovat grafy.\n\n" +
            "Pro další informace kontaktujte podporu.",
            "OK");
    }

    /// <summary>
    /// Zavření záložky – odstranění dat a výběr jiné
    /// </summary>
    private void OnCloseTabClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphTab tab)
        {
            Tabs.Remove(tab);
            SelectedTab = Tabs.FirstOrDefault();

            OnPropertyChanged(nameof(Tabs));
            OnPropertyChanged(nameof(Tabs.Count));
            OnPropertyChanged(nameof(SelectedTab));

            RecalculateGraphSizes();
        }
    }

    /// <summary>
    /// Dynamické přepočítání velikosti grafů podle velikosti okna
    /// </summary>
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        RecalculateGraphSizes();

        double leftPanelWidth = 300;
        double padding = 40;

        double rightAvailableWidth = width - leftPanelWidth - padding;
        double usableHeight = height - 120;

        if (SelectedTab?.Graphs is null)
            return;

        foreach (var graph in SelectedTab.Graphs)
        {
            graph.Width = graph.IsExpanded ? rightAvailableWidth - 20 : rightAvailableWidth / 2 - 20;
            graph.Height = graph.IsExpanded ? usableHeight * 0.95 : usableHeight * 0.5;
        }
    }

    /// <summary>
    /// Přepočet velikosti grafů ručně – např. po změně záložky
    /// </summary>
    private void RecalculateGraphSizes()
    {
        double leftPanelWidth = 300;
        double padding = 40;

        double rightAvailableWidth = Width - leftPanelWidth - padding;
        double usableHeight = Height - 100;

        if (SelectedTab?.Graphs is null)
            return;

        foreach (var graph in SelectedTab.Graphs)
        {
            graph.Width = graph.IsExpanded ? rightAvailableWidth - 20 : rightAvailableWidth / 2 - 20;
            graph.Height = graph.IsExpanded ? usableHeight : usableHeight * 0.5;
        }
    }

    // --- metody pro suggestions a jejich výběr ---

    /// <summary>
    /// Naplní kolekci FilteredDisplayData daty, včetně záhlaví a všech extra hodnot
    /// </summary>
    private void UpdateFilteredDisplayData(IEnumerable<Data> data)
    {
        if (SelectedTab == null) return;

        var target = SelectedTab.FilteredDisplayData;
        target.Clear();

        var extraKeys = data.SelectMany(d => d.extraValues.Keys).Distinct().ToArray();
        var header = new List<string> { "Frequency", "Temperature" };
        header.AddRange(extraKeys);
        target.Add(string.Join("\t", header));

        foreach (var d in data)
        {
            List<string> row = new()
            {
                d.Frequency.ToString("E2"),
                d.Temperature.ToString("E2")
            };

            foreach (var key in extraKeys)
            {
                row.Add(d.extraValues.TryGetValue(key, out double val) ? val.ToString("E2") : "-");
            }

            target.Add(string.Join("\t", row));
        }
    }

    /// <summary>
    /// Na základě vstupu v teplotním SearchBaru připraví návrhy hodnot
    /// </summary>
    private void UpdateTemperatureSuggestions(string input)
    {
        TemperatureSuggestions.Clear();

        if (SelectedTab?.MeasureData?.FileData == null || string.IsNullOrWhiteSpace(input)) return;

        var temps = SelectedTab.MeasureData.FileData
            .Select(d => d.Temperature.ToString("E2"))
            .Distinct()
            .Where(t => t.StartsWith(input));

        foreach (var t in temps)
            TemperatureSuggestions.Add(t);
    }

    /// <summary>
    /// Na základě vstupu ve frekvenčním SearchBaru připraví návrhy hodnot
    /// </summary>
    private void UpdateFrequencySuggestions(string input)
    {
        FrequencySuggestions.Clear();

        if (SelectedTab?.MeasureData?.FileData == null || string.IsNullOrWhiteSpace(input)) return;

        var freqs = SelectedTab.MeasureData.FileData
            .Select(d => d.Frequency.ToString("E2"))
            .Distinct()
            .Where(f => f.StartsWith(input));

        foreach (var f in freqs)
            FrequencySuggestions.Add(f);
    }

    /// <summary>
    /// Po kliknutí na návrh v teplotním dropdownu nastaví hodnotu a spustí hledání
    /// </summary>
    private void OnTemperatureSuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selected)
        {
            TemperatureSearchBar.Text = selected;
            TemperatureSuggestions.Clear();
            OnTemperatureSearch(sender, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Po kliknutí na návrh ve frekvenčním dropdownu nastaví hodnotu a spustí hledání
    /// </summary>
    private void OnFrequencySuggestionSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selected)
        {
            FrequencySearchBar.Text = selected;
            FrequencySuggestions.Clear();
            OnFrequencySearch(sender, EventArgs.Empty);
        }
    }
}
