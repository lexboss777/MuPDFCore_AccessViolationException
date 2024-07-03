using System.Linq;
using Avalonia.Controls;
using SimpleToDoList.Services;
using SimpleToDoList.ViewModels;

namespace SimpleToDoList.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var pdfW = new PdfWindow();
        pdfW.ShowDialog(this);
    }
}