using Microsoft.Maui.Storage; // Pro práci se soubory v MAUI aplikaci
using System;
using System.Threading.Tasks; // Pro asynchronní operace

namespace project.Services;

/// <summary>
/// Třída pro zpracování načítání souborů v aplikaci.
/// Umožňuje uživateli vybrat soubor a načíst jeho obsah.
/// </summary>
public class FileHandler
{
    /// <summary>
    /// Otevře dialog pro výběr souboru a vrátí cestu k vybranému souboru.
    /// </summary>
    /// <returns>Úplná cesta k vybranému souboru nebo null, pokud nebyl vybrán žádný soubor.</returns>
    public async Task<string?> PickFileAsync()
    {
        var result = await FilePicker.PickAsync(); // Otevře systémový dialog pro výběr souboru
        return result?.FullPath; // Vrátí cestu k souboru, pokud byl vybrán
    }

    /// <summary>
    /// Načte obsah souboru ze zadané cesty.
    /// </summary>
    /// <param name="filePath">Cesta k souboru, který chceme načíst.</param>
    /// <returns>Obsah souboru jako string nebo null, pokud soubor neexistuje.</returns>
    public async Task<string?> ReadFileContentAsync(string filePath)
    {
        if (!string.IsNullOrEmpty(filePath)) // Kontrola, zda byla zadána platná cesta
        {
            return await File.ReadAllTextAsync(filePath); // Načte obsah souboru jako string
        }
        return null; // Pokud cesta není platná, vrátí null
    }
}
