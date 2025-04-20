<<<<<<< HEAD
﻿using Microsoft.Maui;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Syncfusion.Maui.Core.Hosting;
using project.Converters;

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
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); // Přidání vlastního fontu
            })
            .ConfigureSyncfusionCore(); // Nastavení Syncfusion komponent


        return builder.Build(); // Sestavení aplikace
    }
}
=======
using Syncfusion.Maui.Core.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;
using CommunityToolkit.Maui;

namespace project.App;

/// <summary>
/// Configures and builds the main MAUI application.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates and configures the MAUI application instance.
    /// </summary>
    /// <returns>The configured <see cref="MauiApp"/> instance.</returns>
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
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

        return builder.Build();
    }
}
>>>>>>> export-w-graphs
