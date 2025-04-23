using System.Collections.ObjectModel;
using System.ComponentModel;
using project.Services;
using project.Managers;
using project.Models;
using System.Globalization;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using SkiaSharp;


namespace project.View;

/// <summary>
/// The main page of the application, containing UI and logic for file processing and graph rendering.
/// </summary>
public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the temperature input value for the currently selected tab.
    /// </summary>
    public string TemperatureInput
    {
        get => SelectedTab?.TemperatureInput ?? string.Empty;
        set
        {
            if (SelectedTab != null)
            {
                SelectedTab.TemperatureInput = value;
                OnPropertyChanged(nameof(TemperatureInput));
            }
        }
    }

    /// <summary>
    /// Gets or sets the frequency input value for the currently selected tab.
    /// </summary>
    public string FrequencyInput
    {
        get => SelectedTab?.FrequencyInput ?? string.Empty;
        set
        {
            if (SelectedTab != null)
            {
                SelectedTab.FrequencyInput = value;
                OnPropertyChanged(nameof(FrequencyInput));
            }
        }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;
    private new void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private GraphManager _graphManager = new();
    private FileHandler _fileHandler = new();

    private ObservableCollection<GraphTab> _tabs = new();

    /// <summary>
    /// Collection of all tabs containing loaded files.
    /// </summary>
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

    private GraphTab? _selectedTab;

    /// <summary>
    /// Currently selected tab.
    /// </summary>
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
                OnPropertyChanged(nameof(TemperatureInput));
                OnPropertyChanged(nameof(FrequencyInput));

                if (_selectedTab?.Graphs != null)
                {
                    foreach (var graph in _selectedTab.Graphs)
                    {
                        RenderGraph(graph);
                    }
                }
            }
        }
    }

    /// <summary>
    /// List of graphs managed globally.
    /// </summary>
    public ObservableCollection<GraphModel> Graphs { get; set; }

    public ObservableCollection<string> TemperatureSuggestions { get; set; } = new();
    public ObservableCollection<string> FrequencySuggestions { get; set; } = new();

    private bool _isAnyGraphExpanded;

    /// <summary>
    /// Indicates if any graph is currently expanded (fullscreen).
    /// </summary>
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

    private bool _isFrequencyEnabled = true;
    private bool _isTemperatureEnabled = true;

    /// <summary>
    /// Determines if the frequency input is enabled.
    /// </summary>
    public bool IsFrequencyEnabled
    {
        get => _isFrequencyEnabled;
        set
        {
            _isFrequencyEnabled = value;
            OnPropertyChanged(nameof(IsFrequencyEnabled));
        }
    }

    /// <summary>
    /// Determines if the temperature input is enabled.
    /// </summary>
    public bool IsTemperatureEnabled
    {
        get => _isTemperatureEnabled;
        set
        {
            _isTemperatureEnabled = value;
            OnPropertyChanged(nameof(IsTemperatureEnabled));
        }
    }

    /// <summary>
    /// Static reference to the MainPage instance.
    /// </summary>
    public static MainPage? Instance { get; private set; }

    /// <summary>
    /// Constructor – sets up bindings and initializes UI components.
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
    /// Opens file picker, loads selected file, and creates a new tab with the data.
    /// </summary>
    private async void OnLoadFileClicked(object sender, EventArgs e)
    {
        string? filePath = await _fileHandler.PickFileAsync();
        if (string.IsNullOrEmpty(filePath)) return;

        var newTab = new GraphTab { FileName = Path.GetFileName(filePath) };

        if (!await newTab.MeasureData.LoadData(filePath))
        {
            await DisplayAlert("Chyba", "Nepodařilo se načíst data ze souboru.", "OK");
            return;
        }

        newTab.DisplayData.Clear();
        foreach (var line in await newTab.MeasureData.MakeToStringAsync(100))
            newTab.DisplayData.Add(line);

        var allKeys = newTab.MeasureData.FileData
                      .SelectMany(d => d.extraValues.Keys)
                      .Distinct()
                      .ToList();

        if (allKeys.Count == 0)
        {
            await DisplayAlert("Varování", "Soubor neobsahuje žádné metriky.", "OK");
            return;
        }

        var g1 = new GraphModel
        {
            Name = "Graf 1",
            AvailableYKeys = new ObservableCollection<string>(allKeys),
            SelectedKeyY = null
        };
        newTab.Graphs.Add(g1);

        Tabs.Add(newTab);
        SelectedTab = newTab;

        RecalculateGraphSizes();
    }

    /// <summary>
    /// Adds a new graph to the currently selected tab.
    /// </summary>
    private void OnAddGraphClicked(object sender, EventArgs e)
    {
        if (SelectedTab == null) return;

        var allKeys = SelectedTab.MeasureData.FileData
                     .SelectMany(d => d.extraValues.Keys)
                     .Distinct()
                     .ToList();

        int n = SelectedTab.Graphs.Count + 1;

        var newGraph = new GraphModel
        {
            Name = $"Graf {n}",
            AvailableYKeys = new ObservableCollection<string>(allKeys),
            SelectedKeyY = null
        };

        SelectedTab.Graphs.Add(newGraph);

        UpdateGraphVisibility();
        RecalculateGraphSizes();
    }

    /// <summary>
    /// Toggles the expansion state of the graph.
    /// </summary>
    public void OnResizeGraphClicked(object sender, EventArgs e)
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
    /// Updates the visibility of each graph based on the expansion state.
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
    /// Simulates exporting a graph (currently just a confirmation dialog).
    /// </summary>
    public async void OnExportGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph)
            await DisplayAlert("Export", $"Exportuji graf: {graph.Name}", "OK");
        else
            await DisplayAlert("Chyba", "Nelze exportovat graf.", "OK");
    }

    /// <summary>
    /// Handles changes in temperature input – disables the frequency input and shows suggestions.
    /// </summary>
    private void OnTemperatureTextChanged(object sender, TextChangedEventArgs e)
    {
        IsFrequencyEnabled = string.IsNullOrWhiteSpace(e.NewTextValue);
        UpdateTemperatureSuggestions(e.NewTextValue);
    }

    /// <summary>
    /// Handles changes in frequency input – disables the temperature input and shows suggestions.
    /// </summary>
    private void OnFrequencyTextChanged(object sender, TextChangedEventArgs e)
    {
        IsTemperatureEnabled = string.IsNullOrWhiteSpace(e.NewTextValue);
        UpdateFrequencySuggestions(e.NewTextValue);
    }

    /// <summary>
    /// Executes temperature search and generates the filtered data output.
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
    /// Executes frequency search and generates the filtered data output.
    /// </summary>
    private async void OnFrequencySearch(object sender, EventArgs e)
    {
        if (SelectedTab == null || FrequencySearchBar == null) return;

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
    /// Help button click handler – displays an informational dialog.
    /// </summary>
    private async void OnHelpClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Nápověda",
            "Tato aplikace slouží k analýze dat podle teploty a frekvence.\n\n" +
            "1. Klikněte na tlačítko 'Načíst soubor' a vyberte textový soubor s měřením.\n" +
            "2. Po načtení se zobrazí náhled dat a automaticky se vytvoří grafy.\n\n" +
            "3. Pro filtrování dat zadejte hodnotu teploty (např. 25) nebo frekvence (např. 1000).\n" +
            "   - Jakmile zadáte jednu hodnotu, druhé pole se automaticky uzamkne.\n" +
            "   - Vyberte hodnotu ze seznamu nebo klikněte na lupu pro vyhledání.\n\n" +
            "4. V hlavní části se zobrazují grafy:\n" +
            "   - Každý graf lze rozbalit kliknutím na 'Zvětšit' a ostatní grafy se dočasně skryjí.\n" +
            "   - Pomocí šipky můžete přepnout zobrazenou veličinu na ose Y.\n" +
            "   - Graf lze exportovat do Excelu pomocí tlačítka 'Export'.\n\n" +
            "5. Kliknutím na tlačítko '+' přidáte další prázdný graf.\n" +
            "   - Grafy se automaticky aktualizují podle zvoleného filtru a veličiny.\n\n" +
            "6. Každý otevřený soubor (záložku) můžete zavřít kliknutím na tlačítko 'X'.\n\n",
            "OK");
    }


    /// <summary>
    /// Closes the current tab and selects another if available.
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
    /// Automatically recalculates graph dimensions when the window is resized.
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
    /// Manually recalculates graph sizes, e.g., when switching tabs.
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

    /// <summary>
    /// Generates value suggestions for temperature input based on current file data.
    /// </summary>
    private void UpdateTemperatureSuggestions(string input)
    {
        TemperatureSuggestions.Clear();

        if (SelectedTab?.MeasureData?.FileData == null || string.IsNullOrWhiteSpace(input)) return;

        var temps = SelectedTab.MeasureData.FileData
            .Select(d => d.Temperature.ToString())
            .Distinct()
            .Where(t => t.StartsWith(input))
            .ToList();

        if (temps.Count == 1 && temps[0] == input)
            return;

        foreach (var t in temps)
            TemperatureSuggestions.Add(t);

        TemperatureDropdown.HeightRequest = Math.Min(TemperatureSuggestions.Count * 40, 200);
    }

    /// <summary>
    /// Generates value suggestions for frequency input based on current file data.
    /// </summary>
    private void UpdateFrequencySuggestions(string input)
    {
        FrequencySuggestions.Clear();

        if (SelectedTab?.MeasureData?.FileData == null || string.IsNullOrWhiteSpace(input)) return;

        var freqs = SelectedTab.MeasureData.FileData
            .Select(d => d.Frequency.ToString())
            .Distinct()
            .Where(f => f.StartsWith(input))
            .ToList();

        if (freqs.Count == 1 && freqs[0] == input)
            return;

        foreach (var f in freqs)
            FrequencySuggestions.Add(f);

        FrequencyDropdown.HeightRequest = Math.Min(FrequencySuggestions.Count * 40, 200);
    }

    /// <summary>
    /// Handles selection from the temperature suggestion dropdown.
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
    /// Handles selection from the frequency suggestion dropdown.
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

    /// <summary>
    /// Configures Y-axis appearance for the given graph.
    /// </summary>
    private Axis[] setYAxes(GraphModel graph, string yKey, double minY, double maxY, double yPadding)
    {
        return new Axis[]
        {
            new Axis
            {
                Name = "",
                NamePaint = new SolidColorPaint(SKColors.Black),
                Labeler = val => val.ToString("F2"),
                TextSize = 13,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                MinLimit = minY - yPadding,
                MaxLimit = maxY + yPadding,
                IsVisible = true,
                ShowSeparatorLines = true,
                Position = LiveChartsCore.Measure.AxisPosition.Start
            }
        };
    }

    /// <summary>
    /// Configures X-axis appearance for the given graph.
    /// </summary>
    private Axis[] setXAxes(GraphModel graph, bool isFrequencyXAxis, double minX, double maxX, double xPadding)
    {
        return new Axis[]
        {
            new Axis
            {
                Name = "",
                NamePaint = new SolidColorPaint(SKColors.Black),
                LabelsRotation = 15,
                Labeler = isFrequencyXAxis
                    ? (Func<double, string>)(val => $"{Math.Pow(10, val):0}")
                    : (val => val.ToString("F1")),
                TextSize = 13,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightGray) { StrokeThickness = 1 },
                MinLimit = minX - xPadding,
                MaxLimit = maxX + xPadding,
                IsVisible = true,
                ShowSeparatorLines = false
            }
        };
    }

    /// <summary>
    /// Creates and returns a configured data series.
    /// </summary>
    private ISeries[] CreateSeries(IEnumerable<ObservablePoint> points, string yKey, bool isFrequencyXAxis)
    {
        return new ISeries[]
        {
            new LineSeries<ObservablePoint>
            {
                Values = points.ToList(),
                GeometrySize = 4,
                GeometryFill = new SolidColorPaint(SKColors.Blue),
                GeometryStroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 2 },
                Stroke = new SolidColorPaint(SKColors.Green) { StrokeThickness = 3 },
                Name = yKey,
                Fill = null,
                XToolTipLabelFormatter = (chartPoint) =>
                {
                    if (chartPoint.Model is ObservablePoint model && model.X.HasValue)
                    {
                        double xValue = model.X.GetValueOrDefault();
                        return isFrequencyXAxis
                            ? $"f: {Math.Pow(10, xValue):E3} Hz"
                            : $"T: {model.X:F1} °C";
                    }
                    return string.Empty;
                },
                YToolTipLabelFormatter = (chartPoint) =>
                {
                    if (chartPoint.Model is ObservablePoint model)
                        return $"Y: {model.Y:F3}";
                    return string.Empty;
                }
            }
        };
    }

    /// <summary>
    /// Renders a graph using the current data and axis configurations.
    /// </summary>
    public void RenderGraph(GraphModel graph)
    {
        if (SelectedTab?.FilteredData == null || !SelectedTab.FilteredData.Any())
            return;

        var data = SelectedTab.FilteredData.Filtered;
        string yKey = graph.SelectedKeyY ?? graph.AvailableYKeys?.FirstOrDefault() ?? "Eps'";
        graph.SelectedKeyY = yKey;

        bool isFrequencyXAxis = SelectedTab.FilteredData.FilterType == "temperature";

        var points = data
            .Where(d => d.extraValues.ContainsKey(yKey) && (isFrequencyXAxis ? d.Frequency > 0 : true))
            .Select(d => new ObservablePoint(
                isFrequencyXAxis ? Math.Log10(d.Frequency) : d.Temperature,
                d.extraValues[yKey]))
            .ToList();

        if (points.Count == 0)
        {
            return;
        }

        double minX = points.Min(p => p.X) ?? 0.0;
        double maxX = points.Max(p => p.X) ?? 0.0;
        double minY = points.Min(p => p.Y) ?? 0.0;
        double maxY = points.Max(p => p.Y) ?? 0.0;

        double xPadding = (maxX - minX) * 0.1;
        double yPadding = (maxY - minY) * 0.1;

        graph.Series = CreateSeries(points, yKey, isFrequencyXAxis);
        graph.XAxes = setXAxes(graph, isFrequencyXAxis, minX, maxX, xPadding);
        graph.YAxes = setYAxes(graph, yKey, minY, maxY, yPadding);
    }
}
