<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Models
{
    /// <summary>
    /// Reprezentuje jednu záložku (tab) v uživatelském rozhraní aplikace.
    /// Každá záložka odpovídá jednomu načtenému souboru a obsahuje s ním související data a grafy.
    /// </summary>
    public class GraphTab
    {
  
        /// <summary>
        /// Název souboru, který byl načten (např. "data1.txt").
        /// Zobrazuje se v UI jako popisek záložky.
        /// </summary>
        public string? FileName { get; set; }   // Název souboru
      
        /// <summary>
        /// Kolekce grafů v rámci této záložky.
        /// Každý graf reprezentuje vizualizaci části dat.
        /// Tato kolekce je přímo vázaná na UI (např. BindableLayout).
        /// </summary>
        public ObservableCollection<GraphModel> Graphs { get; set; } = new(); // Kolekce grafů
      
        /// <summary>
        /// Textová reprezentace dat ze souboru, určená pro zobrazení v náhledu (vlevo).
        /// Zde se ukládají řádky, které se vykreslují v ListView nebo CollectionView.
        /// </summary>
        public ObservableCollection<string> DisplayData { get; set; } = new(); // kolekce pro zobrazení dat

        /// <summary>
        /// Kolekce textových řádků, které reprezentují filtrovaná data.
        /// Zobrazuje se v levém panelu aplikace po provedení filtrace podle teploty nebo frekvence.
        /// Každá položka je jeden řádek tabulky převedený na text.
        /// </summary>
        public ObservableCollection<string> FilteredDisplayData { get; set; } = new();

        /// <summary>
        /// Objekt, který obsahuje všechna data načtená ze souboru.
        /// Používá se k zobrazení původních (nefiltrovaných) dat a k provádění filtrací.
        /// </summary>
        public MeasureData MeasureData { get; set; } = new();

        /// <summary>
        /// Výsledek poslední aplikované filtrace (např. podle teploty nebo frekvence).
        /// Pokud nebyl aplikován žádný filtr, zůstává null.
        /// </summary>
        public FilteredData? FilteredData { get; set; }

        /// <summary>
        /// Textová hodnota zadaná uživatelem do teplotního SearchBaru.
        /// Uchovává se na úrovni záložky, takže každá záložka má svůj vlastní vstup.
        /// </summary>
        public string TemperatureInput { get; set; } = string.Empty;

        /// <summary>
        /// Textová hodnota zadaná uživatelem do frekvenčního SearchBaru.
        /// Uchovává se na úrovni záložky, takže každá záložka má svůj vlastní vstup.
        /// </summary>
        public string FrequencyInput { get; set; } = string.Empty;
    }
}
=======
using System.Collections.ObjectModel;

namespace project.Models
{
    /// <summary>
    /// Represents a single tab in the application's user interface.
    /// Each tab corresponds to one loaded file and contains associated data and graphs.
    /// </summary>
    public class GraphTab
    {
        /// <summary>
        /// The name of the loaded file (e.g., "data1.txt").
        /// Shown in the UI as the tab's title.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// Collection of graphs within this tab.
        /// Each graph visualizes a portion of the loaded data.
        /// </summary>
        public ObservableCollection<GraphModel> Graphs { get; set; } = new();

        /// <summary>
        /// Text-based representation of the raw file data for preview.
        /// Shown in a ListView or CollectionView in the left panel.
        /// </summary>
        public ObservableCollection<string> DisplayData { get; set; } = new();

        /// <summary>
        /// Filtered lines of data (after temperature or frequency filtering).
        /// Displayed in the left panel after filtering is applied.
        /// </summary>
        public ObservableCollection<string> FilteredDisplayData { get; set; } = new();

        /// <summary>
        /// Contains the complete loaded data from the file.
        /// Used for filtering and rendering graphs.
        /// </summary>
        public MeasureData MeasureData { get; set; } = new();

        /// <summary>
        /// Stores the result of the most recent filter applied.
        /// Remains null if no filter is active.
        /// </summary>
        public FilteredData? FilteredData { get; set; }

        /// <summary>
        /// Temperature input entered by the user in the SearchBar.
        /// Each tab maintains its own value.
        /// </summary>
        public string TemperatureInput { get; set; } = string.Empty;

        /// <summary>
        /// Frequency input entered by the user in the SearchBar.
        /// Each tab maintains its own value.
        /// </summary>
        public string FrequencyInput { get; set; } = string.Empty;
    }
}
>>>>>>> export-w-graphs
