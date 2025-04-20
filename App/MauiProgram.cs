using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Syncfusion.Maui.Core.Hosting;
using project.Converters;
using LiveChartsCore.SkiaSharpView.Maui; // Přidáno pro grafy
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
            .UseMauiApp<App>()
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SkiaSharp.Views.Maui.Controls.SKCanvasView, SkiaSharp.Views.Maui.Handlers.SKCanvasViewHandler>();
            })
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            })
            .ConfigureSyncfusionCore();


        return builder.Build(); // Sestavení aplikace
    }
}
