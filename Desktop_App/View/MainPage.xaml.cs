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
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.Kernel;


namespace project.View;

/// <summary>
/// Hlavní stránka aplikace, která obsahuje UI a logiku pro zpracování souborů a grafů.
/// </summary>
public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    // Správa změny property
    public new event PropertyChangedEventHandler? PropertyChanged;
    private new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    // Instance správce grafů, který se stará o jejich správu.
    private GraphManager _graphManager = new();

    // Instance správce souborů, který umožňuje načítání obsahu souborů.
    private FileHandler _fileHandler = new();

    // Záložka nahraného souboru
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

    // Právě vybraná záložka
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


    // Kolekce grafů, která je propojena s UI a obsahuje seznam všech grafů.
    public ObservableCollection<GraphModel> Graphs { get; set; }

    
    // Interní proměnná pro sledování, zda je nějaký graf zvětšený.
    private bool _isAnyGraphExpanded;

    // Vlastnost pro indikaci, zda je některý graf zvětšený.
    public bool IsAnyGraphExpanded
    {
        get => _isAnyGraphExpanded;
        set
        {
            if (_isAnyGraphExpanded != value)
            {
                _isAnyGraphExpanded = value;
                OnPropertyChanged(nameof(IsAnyGraphExpanded)); // Notifikace změny
            }
        }
    }

    // Proměnné pro řízení vstupu uživatele, kdy jeden vstup zamyká druhý.
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

    // Statická instance MainPage pro snadný přístup k této třídě z jiných tříd.
    public static MainPage? Instance { get; private set; }

    /// <summary>
    /// Konstruktor hlavní stránky aplikace.
    /// </summary>
    public MainPage()
    {
        Instance = this; // Nastavení statické instance
        InitializeComponent(); // Inicializace komponent UI
        Graphs = _graphManager.Graphs; // Propojení kolekce grafů se správcem
        BindingContext = this; // Nastavení BindingContext pro data binding

        Tabs.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(Tabs));
            OnPropertyChanged(nameof(Tabs.Count));
        };

        // Přidání testovacích záložek
        // Tabs.Add(new GraphTab() { FileName = "Testovací soubor 1.txt" });
        

        

        // Debug výpis pro kontrolu počtu grafů při spuštění aplikace.
        Debug.WriteLine($"Počet grafů při spuštění: {Graphs.Count}");
    }

    /// <summary>
    /// Metoda pro načtení souboru po kliknutí na tlačítko.
    /// </summary>
    private async void OnLoadFileClicked(object sender, EventArgs e)
    {
        string? filePath = await _fileHandler.PickFileAsync(); // Výběr souboru
        if (!string.IsNullOrEmpty(filePath))
        {
            var newTab = new GraphTab() { FileName = System.IO.Path.GetFileName(filePath) };
            bool loaded = await newTab.MeasureData.LoadData(filePath);
            if (!loaded)
            {
                await DisplayAlert("Chyba", "Nepodařilo se načíst data ze souboru.", "OK");
                return;
            }

            var lines = await newTab.MeasureData.MakeToStringAsync(100);
            newTab.DisplayData.Clear();
            foreach (var line in lines)
            {
                newTab.DisplayData.Add(line);
            }
            newTab.Graphs.Add(new GraphModel { Name = "Graf 1" });

            Tabs.Add(newTab);
            SelectedTab = newTab;
        }
        RecalculateGraphSizes();
    }


    /// <summary>
    /// Přidá nový graf do seznamu grafů.
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
    /// Změní velikost vybraného grafu.
    /// </summary>
    public void OnResizeGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph)
        {
            if (SelectedTab == null) return;

            if (graph.IsExpanded)
            {
                graph.IsExpanded = false;
            }
            else
            {
                foreach (var g in SelectedTab.Graphs)
                {
                    g.IsExpanded = false;
                }
                graph.IsExpanded = true;
            }

            UpdateGraphVisibility();
            OnPropertyChanged(nameof(SelectedTab));
            RecalculateGraphSizes();
        }
    }



    /// <summary>
    /// Aktualizuje viditelnost grafů podle jejich stavu zvětšení.
    /// </summary>
    private void UpdateGraphVisibility()
    {
        if (SelectedTab == null)
            return;

        IsAnyGraphExpanded = SelectedTab.Graphs.Any(g => g.IsExpanded);

        foreach (var g in SelectedTab.Graphs)
        {
            g.IsVisible = !IsAnyGraphExpanded || g.IsExpanded;
        }

        OnPropertyChanged(nameof(IsAnyGraphExpanded));
        OnPropertyChanged(nameof(SelectedTab));
    }


    /// <summary>
    /// Exportuje vybraný graf.
    /// </summary>
    public async void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph)
        {
            await DisplayAlert("Export", $"Exportuji graf: {graph.Name}", "OK");
        }
        else
        {
            await DisplayAlert("Chyba", "Nelze exportovat graf.", "OK");
        }
    }

    /// <summary>
    /// Zamyká zadávání frekvence, pokud uživatel zadal hodnotu teploty.
    /// </summary>
    private void OnTemperatureTextChanged(object sender, TextChangedEventArgs e)
    {
        IsFrequencyEnabled = string.IsNullOrWhiteSpace(e.NewTextValue); // Zamknutí frekvence
    }

    /// <summary>
    /// Zamyká zadávání teploty, pokud uživatel zadal hodnotu frekvence.
    /// </summary>
    private void OnFrequencyTextChanged(object sender, TextChangedEventArgs e)
    {
        IsTemperatureEnabled = string.IsNullOrWhiteSpace(e.NewTextValue); // Zamknutí teploty
    }

    /// <summary>
    /// Simuluje hledání dat na základě zadané teploty.
    /// </summary>
    private async void OnTemperatureSearch(object sender, EventArgs e)
    {
        if (SelectedTab == null || TemperatureSearchBar == null) return;

        if (double.TryParse(TemperatureSearchBar.Text, out double temp))
        {
            SelectedTab.FilteredData = new FilteredData("temperature", temp, SelectedTab.MeasureData);

            // aktualizuj zobrazení
            var lines = await SelectedTab.FilteredData.MakeToStringAsync(0);

            SelectedTab.FilteredDisplayData.Clear();
            foreach (var line in lines)
            {
                SelectedTab.FilteredDisplayData.Add(line);
            }
        }

        if (SelectedTab.Graphs.Count > 0)
        {
            var graph = SelectedTab.Graphs[0];

            var keys = SelectedTab.FilteredData.Filtered
                .SelectMany(d => d.extraValues.Keys)
                .Distinct()
                .ToArray();

            graph.AvailableYKeys = new ObservableCollection<string>(keys);
            graph.SelectedKeyY = "Eps'";

            RenderGraph(graph);

            Debug.WriteLine($"[GRAPH] Dostupné Y klíče: {string.Join(", ", graph.AvailableYKeys)}");
            Debug.WriteLine($"[GRAPH] Zvolený Y klíč: {graph.SelectedKeyY}");
        }




    }

    /// <summary>
    /// Simuluje hledání dat na základě zadané frekvence.
    /// </summary>
    private async void OnFrequencySearch(object sender, EventArgs e)
    {
        if (SelectedTab == null || FrequencySearchBar == null) return;

        if (double.TryParse(FrequencySearchBar.Text, out double freq))
        {
            SelectedTab.FilteredData = new FilteredData("frequency", freq, SelectedTab.MeasureData);

            // aktualizuj zobrazení
            var lines = await SelectedTab.FilteredData.MakeToStringAsync(0);

            SelectedTab.FilteredDisplayData.Clear();
            foreach (var line in lines)
            {
                SelectedTab.FilteredDisplayData.Add(line);
            }
        }

        if (SelectedTab.Graphs.Count > 0)
        {
            var graph = SelectedTab.Graphs[0];

            var keys = SelectedTab.FilteredData.Filtered
                .SelectMany(d => d.extraValues.Keys)
                .Distinct()
                .ToList();

            graph.AvailableYKeys = new ObservableCollection<string>(keys);
            graph.SelectedKeyY = "Eps'";

            RenderGraph(graph);

            Debug.WriteLine($"[GRAPH] Dostupné Y klíče: {string.Join(", ", graph.AvailableYKeys)}");
            Debug.WriteLine($"[GRAPH] Zvolený Y klíč: {graph.SelectedKeyY}");
        }


    }

    /// <summary>
    /// Zobrazuje nápovědu k aplikaci.
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
    /// Handler pro zavření záložky s grafy.
    /// Odstraní záložku z kolekce `Tabs`, nastaví nově aktivní záložku
    /// a přepočítá rozměry zobrazených grafů.
    /// </summary>
    private void OnCloseTabClicked(object sender, EventArgs e)
    {
        // Ověření, že tlačítko má vázaný datový kontext typu GraphTab
        if (sender is Button button && button.BindingContext is GraphTab tab)
        {
            // Odebere záložku ze seznamu
            Tabs.Remove(tab);

            // Pokud nějaké záložky zůstaly, nastaví první z nich jako aktivní
            SelectedTab = Tabs.FirstOrDefault();

            // Notifikace změn pro UI – aktualizace bindingů
            OnPropertyChanged(nameof(Tabs));
            OnPropertyChanged(nameof(Tabs.Count));
            OnPropertyChanged(nameof(SelectedTab));

            // Přepočítání rozměrů grafů podle nového výběru záložky
            RecalculateGraphSizes();
        }
    }


    /// <summary>
    /// Přepis metody, která se volá při změně velikosti okna.
    /// Slouží k přizpůsobení velikosti grafů podle aktuální šířky a výšky.
    /// </summary>
    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Přepočet velikostí grafů
        RecalculateGraphSizes();

        // Definice velikosti levého panelu (sidebaru) a mezery
        double leftPanelWidth = 300;
        double padding = 40;

        // Vypočítání dostupného prostoru pro oblast s grafy
        double rightAvailableWidth = width - leftPanelWidth - padding;
        double usableHeight = height - 120; // odečteme horní část s horní lištou

        // Pokud nejsou žádné grafy nebo záložka není vybraná, končíme
        if (SelectedTab?.Graphs is null)
            return;

        // Pro každý graf v záložce nastavíme šířku a výšku podle stavu zvětšení
        foreach (var graph in SelectedTab.Graphs)
        {
            // Pokud je graf rozbalený, zabírá celou šířku; jinak půlku
            graph.Width = graph.IsExpanded
                ? rightAvailableWidth - 20
                : rightAvailableWidth / 2 - 20;

            // Výška se přizpůsobí podobně – větší pro rozbalený graf
            graph.Height = graph.IsExpanded
                ? usableHeight * 0.95
                : usableHeight * 0.5;
        }
    }


    /// <summary>
    /// Pomocná metoda, která přepočítává rozměry všech grafů
    /// na základě aktuální velikosti okna a stavu zvětšení jednotlivých grafů.
    /// Volá se např. při změně záložky nebo přidání grafu.
    /// </summary>
    private void RecalculateGraphSizes()
    {
        // Konstanty určující levý panel a mezery
        double leftPanelWidth = 300;
        double padding = 40;

        // Výpočet dostupné šířky a výšky pro oblast grafů
        double rightAvailableWidth = Width - leftPanelWidth - padding;
        double usableHeight = Height - 100;

        // Pokud nejsou žádné grafy nebo žádná záložka není vybraná, neděláme nic
        if (SelectedTab?.Graphs is null)
            return;

        // Pro každý graf nastavíme jeho velikost
        foreach (var graph in SelectedTab.Graphs)
        {
            // Zvětšený graf má větší šířku, ostatní se zmenší
            graph.Width = graph.IsExpanded
                ? rightAvailableWidth - 20
                : rightAvailableWidth / 2 - 20;

            // Výška grafu podle stavu rozbalení
            graph.Height = graph.IsExpanded
                ? usableHeight
                : usableHeight * 0.5;
            Debug.WriteLine($"Graph size: W={graph.Width}, H={graph.Height}");
        }

    }

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

    


    public void RenderGraph(GraphModel graph)
    {
        if (SelectedTab?.FilteredData == null || !SelectedTab.FilteredData.Any())
            return;
        var data = SelectedTab.FilteredData.Filtered;
        string yKey = graph.SelectedKeyY ?? graph.AvailableYKeys?.FirstOrDefault() ?? "Eps'";
        graph.SelectedKeyY = yKey;
        var points = data
            .Where(d => d.extraValues.ContainsKey(yKey))
            .Select(d => new ObservablePoint(
                SelectedTab.FilteredData.FilterType == "temperature" ? d.Frequency : d.Temperature,
                d.extraValues[yKey]))
            .ToList();

        Debug.WriteLine($"[RenderGraph] Klíč Y: {yKey}, Počet bodů: {points.Count}");
        if (points.Count == 0)
        {
            Debug.WriteLine("[RenderGraph] Žádné body k vykreslení.");
            return;
        }

        // Calculate min/max values for axes
        double minX = (double)points.Min(p => p.X);
        double maxX = (double)points.Max(p => p.X);
        double minY = (double)points.Min(p => p.Y);
        double maxY = (double)points.Max(p => p.Y);

        // Apply some padding to the min/max values
        double xPadding = (maxX - minX) * 0.1;
        double yPadding = (maxY - minY) * 0.1;

        // Improve the line series configuration
        graph.Series = new ISeries[]
        {
        new LineSeries<ObservablePoint>
        {
            Values = points,
            GeometrySize = 4, // Point size
            GeometryFill = new SolidColorPaint(SKColors.Blue),
            GeometryStroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 2 },
            Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 },
            Name = yKey,
            Fill = null,
            XToolTipLabelFormatter = (chartPoint) => $"X: {chartPoint.Model.X:E2}",
            YToolTipLabelFormatter = (chartPoint) => $"Y: {chartPoint.Model.Y:E2}"
        }
        };

        // Configure X Axis with more visible properties
        graph.XAxes = new Axis[]
        {
        new Axis
        {
            Name = SelectedTab.FilteredData.FilterType == "temperature" ? "Frequency [Hz]" : "Temperature [°C]",
            NamePaint = new SolidColorPaint(SKColors.Black),
            
            // Ensure labels are visible with proper rotation
            LabelsRotation = 15,
            Labeler = val => val.ToString("E2"),
            TextSize = 13,
            
            // Make separators visible
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
            
            // Set the min and max limits to ensure the axis is visible
            MinLimit = minX - xPadding,
            MaxLimit = maxX + xPadding,
            
            // Show axis
            IsVisible = true,
            ShowSeparatorLines = true
        }
        };

        // Configure Y Axis with more visible properties
        graph.YAxes = new Axis[]
        {
        new Axis
        {
            Name = yKey,
            NamePaint = new SolidColorPaint(SKColors.Black),
            
            // Ensure labels are properly formatted
            Labeler = val => val.ToString("E2"),
            TextSize = 13,
            
            // Make separators visible
            SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
            
            // Set the min and max limits to ensure the axis is visible
            MinLimit = minY - yPadding,
            MaxLimit = maxY + yPadding,
            
            // Show axis
            IsVisible = true,
            ShowSeparatorLines = true
        }
        };

        Debug.WriteLine($"Graph Size -> Width: {graph.Width}, Height: {graph.Height}");
    }
}
