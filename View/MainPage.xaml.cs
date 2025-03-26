using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.ComponentModel;
using project.Services;
using project.Managers;
using project.Models;

namespace project.View;

/// <summary>
/// Hlavní stránka aplikace, která obsahuje UI a logiku pro zpracování souborů a grafů.
/// </summary>
public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    // Správa změny property
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName)
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
    private GraphTab _selectedTab;
    public GraphTab SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (_selectedTab != value)
            {
                _selectedTab = value;
                OnPropertyChanged(nameof(SelectedTab));
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

    // Kolekce pro ukládání načtených dat z textového souboru.
    public ObservableCollection<string> DataItems { get; set; } = new();

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
    public static MainPage Instance { get; private set; }

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

            // Získání řádků dat a naplnění kolekce
            var lines = await newTab.MeasureData.MakeToStringAsync(100);
            newTab.DisplayData.Clear();
            foreach (var line in lines)
            {
                newTab.DisplayData.Add(line);
            }

            Tabs.Add(newTab);
            SelectedTab = newTab;

            // OnPropertyChanged(nameof(Tabs));
            // OnPropertyChanged(nameof(SelectedTab));
            // FilePreviewLabel.Text = await newTab.MeasureData.MakeToStringAsync(); // Zobrazení obsahu
        }
    }

    /// <summary>
    /// Přidá nový graf do seznamu grafů.
    /// </summary>
    private void OnAddGraphClicked(object sender, EventArgs e)
    {
        _graphManager.AddGraph(); // Přidání grafu
        UpdateGraphVisibility(); // Aktualizace zobrazení grafů
    }

    /// <summary>
    /// Změní velikost vybraného grafu.
    /// </summary>
    private void OnResizeGraphClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphModel graph)
        {
            // Pokud už je graf zvětšený, zmenšíme ho
            if (graph.IsExpanded)
            {
                graph.IsExpanded = false;
            }
            else
            {
                // Nejprve všechny grafy zmenšíme
                foreach (var g in Graphs)
                {
                    g.IsExpanded = false;
                }
                // A zvětšíme pouze vybraný graf
                graph.IsExpanded = true;
            }



            UpdateGraphVisibility(); // Aktualizace UI
        }
    }

    /// <summary>
    /// Aktualizuje viditelnost grafů podle jejich stavu zvětšení.
    /// </summary>
    private void UpdateGraphVisibility()
    {
        IsAnyGraphExpanded = Graphs.Any(g => g.IsExpanded); // Kontrola, zda je nějaký graf zvětšený
        Debug.WriteLine($"IsAnyGraphExpanded: {IsAnyGraphExpanded}");

        foreach (var g in Graphs)
        {
            g.IsVisible = !IsAnyGraphExpanded || g.IsExpanded; // Nastavení viditelnosti podle stavu
            Debug.WriteLine($"Graf: {g.Name}, IsVisible: {g.IsVisible}, IsExpanded: {g.IsExpanded}");
        }

        // Přinutíme UI překreslit CollectionView
        OnPropertyChanged(nameof(IsAnyGraphExpanded));
        OnPropertyChanged(nameof(Graphs));
    }

    /// <summary>
    /// Exportuje vybraný graf.
    /// </summary>
    private async void OnExportGraphClicked(object sender, EventArgs e)
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
        Console.WriteLine($"Teplota změněna: {e.NewTextValue}");
        IsFrequencyEnabled = string.IsNullOrWhiteSpace(e.NewTextValue); // Zamknutí frekvence
    }

    /// <summary>
    /// Zamyká zadávání teploty, pokud uživatel zadal hodnotu frekvence.
    /// </summary>
    private void OnFrequencyTextChanged(object sender, TextChangedEventArgs e)
    {
        Console.WriteLine($"Frekvence změněna: {e.NewTextValue}");
        IsTemperatureEnabled = string.IsNullOrWhiteSpace(e.NewTextValue); // Zamknutí teploty
    }

    /// <summary>
    /// Simuluje hledání dat na základě zadané teploty.
    /// </summary>
    private void OnTemperatureSearch(object sender, EventArgs e)
    {
        Console.WriteLine("Hledání teploty bylo spuštěno.");
    }

    /// <summary>
    /// Simuluje hledání dat na základě zadané frekvence.
    /// </summary>
    private void OnFrequencySearch(object sender, EventArgs e)
    {
        Console.WriteLine("Hledání frekvence bylo spuštěno.");
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

    private void OnCloseTabClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is GraphTab tab)
        {
            Tabs.Remove(tab);

            SelectedTab = Tabs.FirstOrDefault();
                

            OnPropertyChanged(nameof(Tabs));
            OnPropertyChanged(nameof(Tabs.Count));
            OnPropertyChanged(nameof(SelectedTab));
        }
    }
}