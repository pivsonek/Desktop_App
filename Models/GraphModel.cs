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

    // Soukromá proměnná pro sledování, zda je graf rozbalený (zvětšený)
    private bool _isExpanded;

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

    /// <summary>
    /// Určuje, zda je graf rozbalený (zvětšený).
    /// Pokud se hodnota změní, UI se aktualizuje.
    /// </summary>
    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded != value) // Pokud došlo ke změně hodnoty
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded)); // Notifikace změny
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
        get => !MainPage.Instance.IsAnyGraphExpanded || IsExpanded;
        set
        {
            if (_isVisible != value) // Pokud došlo ke změně hodnoty
            {
                _isVisible = value;
                Debug.WriteLine($"Graf viditelnost změněna: {Name} -> {value}"); // Ladící výpis do konzole
                OnPropertyChanged(nameof(IsVisible)); // Notifikace změny
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
