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

        public ObservableCollection<string> FilteredDisplayData { get; set; } = new(); // kolekce pro zobrazení filtrovaných dat
      
        /// <summary>
        /// Instance třídy, která obsahuje parsovaná měřená data ze souboru.
        /// Používá se např. pro filtrování podle teploty/frekvence nebo formátování textového výstupu.
        /// </summary>
        public MeasureData MeasureData { get; set; } = new(); // Data měření

        public FilteredData? FilteredData { get; set; } // Filtrovaná data
    }
}
