using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Models
{
    public class GraphTab
    {
        public string? FileName { get; set; }   // Název souboru
        public ObservableCollection<GraphModel> Graphs { get; set; } = new(); // Kolekce grafů
        public ObservableCollection<string> DisplayData { get; set; } = new(); // kolekce pro zobrazení dat
        public MeasureData MeasureData { get; set; } = new(); // Data měření
    }
}
