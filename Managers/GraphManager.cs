using project.Models;
using System.Collections.ObjectModel;

namespace project.Managers;

/// <summary>
/// Manages a collection of graphs – provides methods to add, remove, and toggle graph expansion.
/// </summary>
public class GraphManager
{
    /// <summary>
    /// The collection of graphs that supports dynamic updates and UI notifications.
    /// </summary>
    public ObservableCollection<GraphModel> Graphs { get; private set; } = new();

    /// <summary>
    /// Initializes the GraphManager and populates it with four default graphs.
    /// </summary>
    public GraphManager()
    {
        Graphs.Add(new GraphModel { Name = "Graf 1" });
        Graphs.Add(new GraphModel { Name = "Graf 2" });
        Graphs.Add(new GraphModel { Name = "Graf 3" });
        Graphs.Add(new GraphModel { Name = "Graf 4" });
    }

    /// <summary>
    /// Adds a new graph to the collection with an automatically generated name.
    /// </summary>
    public void AddGraph()
    {
        int nextNumber = Graphs.Count + 1;
        Graphs.Add(new GraphModel { Name = $"Graf {nextNumber}" });
    }

    /// <summary>
    /// Removes the specified graph from the collection if it exists.
    /// </summary>
    /// <param name="graph">The graph to remove.</param>
    public void RemoveGraph(GraphModel graph)
    {
        if (Graphs.Contains(graph))
        {
            Graphs.Remove(graph);
        }
    }

    /// <summary>
    /// Toggles the expansion state of the specified graph.
    /// </summary>
    /// <param name="graph">The graph to toggle expansion for.</param>
    public void ToggleGraphSize(GraphModel graph)
    {
        if (graph != null)
        {
            graph.IsExpanded = !graph.IsExpanded;
        }
    }
}
