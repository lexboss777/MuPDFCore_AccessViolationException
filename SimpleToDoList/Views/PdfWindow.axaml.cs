using Avalonia.Controls;
using MuPDFCore;
using MuPDFCore.MuPDFRenderer;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleToDoList.Views
{
    public partial class PdfWindow : Window
    {
        int maxPageNumber;

        int pageNumber;

        MuPDFContext Context;

        MuPDFDocument Document;

        PDFRenderer PDFRenderer => this.FindControl<PDFRenderer>("MuPDFRenderer");

        public PdfWindow()
        {
            InitializeComponent();

            Context = new MuPDFContext();

            LoadPdf();
        }

        async Task DownloadFile(HttpClient httpClient, string url, string filePath)
        {
            using var stream = await httpClient.GetStreamAsync(url);
            using var fileStream = new FileStream(filePath, FileMode.CreateNew);
            await stream.CopyToAsync(fileStream);
        }

        async Task LoadPdf()
        {
            try
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Repro", "sample.pdf");

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                if (!File.Exists(path))
                {
                    using var http = new HttpClient();
                    await DownloadFile(http, "https://pdfobject.com/pdf/sample.pdf", path);
                }

                Document = new MuPDFDocument(Context, path);

                pageNumber = 0;
                maxPageNumber = Document.Pages.Count;

                UpdatePage();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        async Task UpdatePage()
        {
            BackBtn.IsEnabled = pageNumber > 0;
            NextBtn.IsEnabled = pageNumber < maxPageNumber - 1;

            PageNumTextBlock.Text = $"{pageNumber + 1} / {maxPageNumber}";

            await PDFRenderer.InitializeAsync(Document, pageNumber: pageNumber, includeAnnotations: false, ocrLanguage: null);
        }

        #region Handlers

        void Window_Closing(object? sender, WindowClosingEventArgs e)
        {
            // dispose the Document and Context. The PDFRenderer will dispose itself when it detects that it has been detached from the logical tree.
            Document?.Dispose();
            Context?.Dispose();
        }

        void Back_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (pageNumber > 0)
                pageNumber--;

            UpdatePage();
        }

        void Next_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (pageNumber < maxPageNumber - 1)
                pageNumber++;

            UpdatePage();
        }

        void Close_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // dispose the Document and Context. The PDFRenderer will dispose itself when it detects that it has been detached from the logical tree.
            Document?.Dispose();
            Context?.Dispose();

            Close();
        }

        #endregion
    }
}
