namespace project.Managers;

/// <summary>
/// Extension methods for VisualElement to get screen and relative coordinates.
/// </summary>
public static class VisualElementExtensions
{
    public static Point GetScreenCoordinates(this VisualElement element)
    {
#if ANDROID
        var nativeView = element.Handler?.PlatformView as Android.Views.View;
        if (nativeView == null) return new Point(0, 0);

        int[] location = new int[2];
        nativeView.GetLocationOnScreen(location);
        var context = Android.App.Application.Context;
        if (context?.Resources?.DisplayMetrics == null)
            return new Point(0, 0);

        var density = context.Resources.DisplayMetrics.Density;

        return new Point(location[0] / density, location[1] / density);

#elif WINDOWS
        var nativeView = element.Handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;
        if (nativeView == null) return new Point(0, 0);

        var transform = nativeView.TransformToVisual(null);
        var point = transform.TransformPoint(new Windows.Foundation.Point(0, 0));

        return new Point(point.X, point.Y);

#else
        return new Point(element.X, element.Y);
#endif
    }

    /// <summary>
    /// Gets the coordinates of the element relative to another element.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="relativeTo"></param>
    /// <returns></returns>
    public static Point GetRelativeCoordinates(this VisualElement element, VisualElement relativeTo)
    {
#if WINDOWS
    var view = element.Handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;
    var to = relativeTo.Handler?.PlatformView as Microsoft.UI.Xaml.FrameworkElement;

    if (view == null || to == null) return new Point(0, 0);

    var transform = view.TransformToVisual(to);
    var pos = transform.TransformPoint(new Windows.Foundation.Point(0, 0));

    return new Point(pos.X, pos.Y);
#else
        var screen = element.GetScreenCoordinates();
        var parent = relativeTo.GetScreenCoordinates();
        return new Point(screen.X - parent.X, screen.Y - parent.Y);
#endif
    }



    /// <summary>
    /// Gets the bounds of the element relative to another element.
    /// </summary>
    /// <param name="view"></param>
    /// <param name="relativeTo"></param>
    /// <returns></returns>
    public static Rect GetBoundsRelativeTo(this VisualElement view, VisualElement relativeTo)
    {
        var absPos = view.GetScreenCoordinates();
        var relativePos = relativeTo.GetScreenCoordinates();

        return new Rect(
            absPos.X - relativePos.X,
            absPos.Y - relativePos.Y,
            view.Width,
            view.Height
        );
    }
}
