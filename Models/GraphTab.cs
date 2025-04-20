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
