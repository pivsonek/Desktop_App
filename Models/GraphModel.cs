using project.View;
using System.ComponentModel; // Pro podporu notifikace změn v UI
using System.Diagnostics; // Pro ladící výpisy do konzole

namespace project.Models;

/// <summary>
/// Model grafu, který podporuje změny vlastností (notifikace).
/// Každý graf má svůj název, stav rozbalení a viditelnost.
/// </summary>
public class GraphModel : INotifyPropertyChanged
{
    // Soukromá proměnná pro název grafu
    private string _name = string.Empty;

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

    // Soukromá proměnná pro viditelnost grafu
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
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); // Oznámíme změnu UI
    }
}
