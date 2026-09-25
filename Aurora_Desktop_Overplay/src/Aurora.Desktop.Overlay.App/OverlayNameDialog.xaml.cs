using System.Windows;

namespace Aurora.Desktop.Overlay.App;

public partial class OverlayNameDialog : Window
{
    public OverlayNameDialog(string initialName, string heading)
    {
        InitializeComponent();
        HeadingText.Text = heading;
        NameTextBox.Text = initialName;
        Loaded += (_, _) =>
        {
            NameTextBox.Focus();
            NameTextBox.SelectAll();
        };
    }

    public string OverlayName => NameTextBox.Text.Trim();

    private void ConfirmClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameTextBox.Text))
        {
            MessageBox.Show(this, "Enter a name for the overlay.", "Aurora Desktop Overlay",
                MessageBoxButton.OK, MessageBoxImage.Information);
            NameTextBox.Focus();
            return;
        }
        DialogResult = true;
    }
}
