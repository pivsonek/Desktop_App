using project.View;
using System.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;

namespace project.Models;

/// <summary>
/// Model grafu, který podporuje změny vlastností (notifikace).
/// Každý graf má svůj název, stav rozbalení a viditelnost.
/// </summary>
public class GraphModel : INotifyPropertyChanged
{
    private string _name = string.Empty;

    public ObservableCollection<string> AvailableYKeys { get; set; } = new();

    private string? _selectedKeyY;
    public string? SelectedKeyY
    {
        get => _selectedKeyY;
        set
        {
            if (_selectedKeyY != value)
            {
                _selectedKeyY = value;
                OnPropertyChanged(nameof(_selectedKeyY));
                MainPage.Instance?.RenderGraph(this);
            }
        }
    }

    private double _width;
    public double Width
    {
        get => _width;
        set { _width = value; OnPropertyChanged(nameof(Width)); }
    }

    private double _height;
    public double Height
    {
        get => _height;
        set { _height = value; OnPropertyChanged(nameof(Height)); }
    }

    private ISeries[] _series = null!;
    public ISeries[] Series
    {
        get => _series;
        set { _series = value; OnPropertyChanged(nameof(Series)); }

    }

    private Axis[] _xAxes = null!;
    public Axis[] XAxes
    {
        get => _xAxes;
        set { _xAxes = value; OnPropertyChanged(nameof(XAxes)); }
    }

    private Axis[] _yAxes = null!;
    public Axis[] YAxes { get => _yAxes; set { _yAxes = value; OnPropertyChanged(nameof(_yAxes)); } }


    /// <summary>
    /// Název grafu.
    /// Pokud se změní, notifikujeme UI, aby se aktualizovalo.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value) // Kontrola, zda je nová hodnota jiná než stará
            {
                _name = value;
                OnPropertyChanged(nameof(Name)); // Oznámíme změnu vlastnosti
            }
        }
    }
    // Soukromá proměnná pro sledování, zda je graf rozbalený (zvětšený)
    private bool _isExpanded;


    /// <summary>
    /// Určuje, zda je graf rozbalený (zvětšený).
    /// Pokud se hodnota změní, UI se aktualizuje.
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value)
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
    }
    private bool _isVisible = true;

    /// <summary>
    /// Určuje, zda je graf viditelný.
    /// Graf je viditelný, pokud není žádný jiný graf rozbalený, nebo je sám rozbalený.
    /// </summary>
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible != value)
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
    }

    /// <summary>
    /// Událost pro oznamování změn vlastností (binding s UI).
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Volá se při změně vlastnosti, aby UI vědělo, že se má překreslit.
    /// </summary>
    /// <param name="propertyName">Název změněné vlastnosti.</param>
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
