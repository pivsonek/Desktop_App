using project.Models;
using System.Collections.ObjectModel; // Pro kolekci ObservableCollection, která umožňuje dynamické aktualizace UI

namespace project.Managers;

/// <summary>
/// Spravuje kolekci grafů – umožňuje jejich přidávání, mazání a změnu velikosti.
/// </summary>
public class GraphManager
{
    /// <summary>
    /// Kolekce grafů, která podporuje dynamické změny a notifikace pro UI.
    /// </summary>
    public ObservableCollection<GraphModel> Graphs { get; private set; } = new();

    /// <summary>
    /// Konstruktor třídy, který inicializuje seznam grafů.
    /// </summary>
    public GraphManager()
    {
        // Při vytvoření instance GraphManager se automaticky přidají 4 výchozí grafy
        Graphs.Add(new GraphModel { Name = "Graf 1" });
        Graphs.Add(new GraphModel { Name = "Graf 2" });
        Graphs.Add(new GraphModel { Name = "Graf 3" });
        Graphs.Add(new GraphModel { Name = "Graf 4" });
    }

    /// <summary>
    /// Přidá nový graf do seznamu grafů.
    /// Každý nový graf dostane unikátní název podle pořadí.
    /// </summary>
    public void AddGraph()
    {
        int nextNumber = Graphs.Count + 1; // Určení čísla nového grafu
        Graphs.Add(new GraphModel { Name = $"Graf {nextNumber}" }); // Přidání nového grafu do kolekce
    }

    /// <summary>
    /// Odebere zadaný graf ze seznamu, pokud existuje.
    /// </summary>
    /// <param name="graph">Graf, který chceme odstranit.</param>
    public void RemoveGraph(GraphModel graph)
    {
        if (Graphs.Contains(graph)) // Kontrola, zda graf existuje v seznamu
        {
            Graphs.Remove(graph); // Odebrání grafu
        }
    }

    /// <summary>
    /// Přepne stav zvětšení/zmenšení daného grafu.
    /// </summary>
    /// <param name="graph">Graf, jehož velikost chceme změnit.</param>
    public void ToggleGraphSize(GraphModel graph)
    {
        if (graph != null) // Kontrola, zda je graf platný
        {
            graph.IsExpanded = !graph.IsExpanded; // Přepnutí stavu (zvětšený <-> zmenšený)
        }
    }
}
