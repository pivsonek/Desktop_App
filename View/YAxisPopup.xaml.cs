using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Layouts;
using project.Managers;
using System.Diagnostics;

namespace project.View;

/// <summary>
/// Represents a popup for selecting Y-axis options.
/// </summary>
public partial class YAxisPopup : Popup
{
    private readonly IEnumerable<string> _options;
    private readonly Action<string> _onItemSelected;

    /// <summary>
    /// Constructor for the YAxisPopup.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="onItemSelected"></param>
    public YAxisPopup(IEnumerable<string> options, Action<string> onItemSelected)
    {
        InitializeComponent();

        _options = options;
        _onItemSelected = onItemSelected;

        this.Opened += OnPopupOpened;
        this.Color = new Color(0,0,0,0);
        this.CanBeDismissedByTappingOutsideOfPopup = true;
        

        foreach (var option in _options)
        {
            var label = new Label
            {
                Text = option,
                Margin = new Thickness(0, 5),
                FontSize = 16,
                TextColor = Colors.Black,
                Padding = new Thickness(8),
                BackgroundColor = Colors.Transparent,
            };

            label.GestureRecognizers.Add(new PointerGestureRecognizer
            {
                PointerEnteredCommand = new Command(() =>
                {
                    label.BackgroundColor = new Color(1f, 0.84f, 0.6f, 0.2f);
                }),
                PointerExitedCommand = new Command(() =>
                {
                    label.BackgroundColor = Colors.Transparent;
                })
            });

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (_, _) =>
            {
                _onItemSelected(option);
                CloseWithAnimation();
            };

            label.GestureRecognizers.Add(tapGesture);
            ItemsLayout.Children.Add(label);
        }
    }

    /// <summary>
    /// Handles the popup opened event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void OnPopupOpened(object? sender, EventArgs e)
    {
        if (PopupContent == null || Anchor == null || RootLayout == null)
            return;

        await Task.Delay(50); 

        var anchorBounds = Anchor.GetBoundsRelativeTo(RootLayout);

        double popupW = 200;
        double popupH = 150;

        double desiredX = anchorBounds.Left + anchorBounds.Width - popupW;
        double desiredY = anchorBounds.Top + anchorBounds.Height;

        desiredY += 10;
        desiredX -= 3;

       

        AbsoluteLayout.SetLayoutBounds(PopupContent, new Rect(desiredX, desiredY, popupW, popupH));
        AbsoluteLayout.SetLayoutFlags(PopupContent, AbsoluteLayoutFlags.None);

        Debug.WriteLine($"[POPUP FINAL] X={desiredX}, Y={desiredY}");

        PopupContent.Opacity = 1;
        await Task.WhenAll(
            PopupContent.FadeTo(1, 150, Easing.CubicIn),
            PopupContent.TranslateTo(0, 0, 250, Easing.CubicOut)
        );

    }

    /// <summary>
    /// Closes the popup with an animation.
    /// </summary>
    private async void CloseWithAnimation()
    {
        if (PopupContent == null)
        {
            base.Close();
            return;
        }

        await Task.WhenAll(
            PopupContent.FadeTo(0, 200, Easing.CubicOut),
            PopupContent.TranslateTo(0, 10, 200, Easing.CubicIn)
        );

        PopupContent.Opacity = 0;
        base.Close();
    }

    public void Close() => CloseWithAnimation();

    
}
