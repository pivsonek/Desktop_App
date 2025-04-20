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
