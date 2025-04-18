using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Syncfusion.Maui.Core.Hosting;
using project.Converters;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.SkiaSharpView;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace project.App;

public static class MauiProgram
{
    /// <summary>
    /// Vytvoří a nakonfiguruje hlavní aplikaci MAUI.
    /// </summary>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>() // Použití hlavní třídy aplikace
            .UseLiveCharts()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); // Přidání vlastního fontu
            })
            .ConfigureSyncfusionCore(); // Nastavení Syncfusion komponent



        return builder.Build(); // Sestavení aplikace
    }
}
