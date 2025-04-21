using project.View;
using System.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;

namespace project.Models;

/// <summary>
/// Represents a graph model containing configuration and display properties for rendering.
/// </summary>
public class GraphModel : INotifyPropertyChanged
{
    private string _name = string.Empty;

    /// <summary>
    /// List of available Y-axis keys that can be plotted.
    /// </summary>
    public ObservableCollection<string> AvailableYKeys { get; set; } = new();

    private string? _selectedKeyY;

    /// <summary>
    /// Currently selected Y-axis key.
    /// When changed, the graph is re-rendered.
    /// </summary>
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

    /// <summary>
    /// Width of the graph UI element.
    /// </summary>
    public double Width
    {
        get => _width;
        set { _width = value; OnPropertyChanged(nameof(Width)); }
    }

    private double _height;

    /// <summary>
    /// Height of the graph UI element.
    /// </summary>
    public double Height
    {
        get => _height;
        set { _height = value; OnPropertyChanged(nameof(Height)); }
    }

    private ISeries[] _series = null!;

    /// <summary>
    /// The data series to be plotted.
    /// </summary>
    public ISeries[] Series
    {
        get => _series;
        set { _series = value; OnPropertyChanged(nameof(Series)); }
    }

    private Axis[] _xAxes = null!;

    /// <summary>
    /// Configuration for the X-axis.
    /// </summary>
    public Axis[] XAxes
    {
        get => _xAxes;
        set { _xAxes = value; OnPropertyChanged(nameof(XAxes)); }
    }

    private Axis[] _yAxes = null!;

    /// <summary>
    /// Configuration for the Y-axis.
    /// </summary>
    public Axis[] YAxes
    {
        get => _yAxes;
        set { _yAxes = value; OnPropertyChanged(nameof(_yAxes)); }
    }

    /// <summary>
    /// Title of the graph.
    /// </summary>
    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    private bool _isExpanded;

    /// <summary>
    /// Indicates whether the graph is currently expanded to full width.
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
    /// Determines whether the graph is visible in the UI.
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
    /// Event triggered when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies the UI about property value changes.
    /// </summary>
    public void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
