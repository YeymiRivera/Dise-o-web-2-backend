using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TarijaReadApp.ViewModels;

namespace TarijaReadApp.Services;

public class ReportService
{
    public byte[] GenerarExcelLibros(List<LibroDto> libros)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Catálogo de Libros");

        hoja.Cell(1, 1).Value = "Título";
        hoja.Cell(1, 2).Value = "Autor";
        hoja.Cell(1, 3).Value = "ISBN";
        hoja.Cell(1, 4).Value = "Categoría";
        hoja.Range("A1:D1").Style.Font.Bold = true;
        hoja.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.LightBlue;

        for (int i = 0; i < libros.Count; i++)
        {
            hoja.Cell(i + 2, 1).Value = libros[i].Titulo;
            hoja.Cell(i + 2, 2).Value = libros[i].Autor;
            hoja.Cell(i + 2, 3).Value = libros[i].ISBN ?? "-";
            hoja.Cell(i + 2, 4).Value = libros[i].Categoria;
        }

        hoja.Columns().AdjustToContents();

        using var memoria = new MemoryStream();
        libro.SaveAs(memoria);
        return memoria.ToArray();
    }

    public byte[] GenerarPdfLibros(List<LibroDto> libros)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text("Reporte de Catálogo - TarijaRead")
                    .FontSize(20).SemiBold().FontColor("#2c3e50");

                page.Content().PaddingVertical(15).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(2);
                        c.RelativeColumn(2);
                    });

                    table.Header(h =>
                    {
                        h.Cell().Text("Título").SemiBold();
                        h.Cell().Text("Autor").SemiBold();
                        h.Cell().Text("Categoría").SemiBold();
                    });

                    foreach (var libro in libros)
                    {
                        table.Cell().Text(libro.Titulo);
                        table.Cell().Text(libro.Autor);
                        table.Cell().Text(libro.Categoria);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Generado el ");
                    x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                });
            });
        });

        return documento.GeneratePdf();
    }
}