using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Application.Helpers
{
    public class PdfTableHelper
    {
        private readonly PdfFont FontTitle = PdfFontFactory.CreateFont(
            StandardFonts.HELVETICA_BOLD
        );
        private readonly PdfFont FontColumns = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
        private readonly PdfFont FontTitleDocument = PdfFontFactory.CreateFont(
            StandardFonts.HELVETICA_BOLD
        );

        private readonly sbyte FontSizeTitle = 16;
        private readonly sbyte FontSizeHeaderTable = 12;
        private readonly sbyte FontSizeColumns = 11;

        private readonly Color BackgroundColor = new DeviceRgb(249, 249, 249);

        private readonly float BorderWidth = 1.5f;
        private readonly sbyte WidthPercentage = 80;
        private readonly sbyte NumColumns = 2;

        public void CreateTitleDocument(Document doc, string title)
        {
            Paragraph titleDocument = new Paragraph(title)
                .SetFont(FontTitleDocument)
                .SetFontSize(FontSizeTitle)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);

            doc.Add(titleDocument);
        }

        public void CreateTable(Document doc, string title, Dictionary<string, string> values)
        {
            Table table = new Table(UnitValue.CreatePercentArray(NumColumns))
                .SetWidth(UnitValue.CreatePercentValue(WidthPercentage))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            Cell titleCell = CreateTitle(title);
            table.AddHeaderCell(titleCell);

            AddColumnsToTable(table, values);

            doc.Add(table);
            doc.Add(new Paragraph(" ").SetMarginBottom(5));
        }

        private Cell CreateTitle(string title)
        {
            Cell cell = new Cell(1, NumColumns)
                .Add(new Paragraph(title).SetFont(FontTitle).SetFontSize(FontSizeHeaderTable))
                .SetBackgroundColor(BackgroundColor)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(BorderWidth));

            return cell;
        }

        private void AddColumnsToTable(Table table, Dictionary<string, string> columns)
        {
            foreach(var value in columns)
            {
                var keyColumn = new Cell()
                    .Add(new Paragraph(value.Key).SetFont(FontColumns).SetFontSize(FontSizeColumns))
                    .SetBackgroundColor(BackgroundColor)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(BorderWidth));

                table.AddCell(keyColumn);

                var valueColumn = new Cell()
                    .Add(
                        new Paragraph(value.Value).SetFont(FontColumns).SetFontSize(FontSizeColumns)
                    )
                    .SetBackgroundColor(BackgroundColor)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(BorderWidth));

                table.AddCell(valueColumn);
            }
        }
    }
}
