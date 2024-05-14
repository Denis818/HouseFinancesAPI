using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Application.Helpers
{
    public class PdfTableHelper(PdfTableStyle pdfTableStyle = null)
    {
        public PdfTableStyle PdfTableStyle { get; } = pdfTableStyle ?? new PdfTableStyle();

        public void CreateTitleDocument(Document doc, string title)
        {
            Paragraph titleDocument = new Paragraph(title)
                .SetFont(PdfTableStyle.FontTitleDocument)
                .SetFontSize(PdfTableStyle.FontSizeTitle)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(20);

            doc.Add(titleDocument);
        }

        public void CreateTable(Document doc, string title, Dictionary<string, string> values)
        {
            Table table = new Table(UnitValue.CreatePercentArray(PdfTableStyle.NumColumns))
                .SetWidth(UnitValue.CreatePercentValue(PdfTableStyle.WidthPercentage))
                .SetHorizontalAlignment(HorizontalAlignment.CENTER);

            Cell titleCell = CreateTitle(title);
            table.AddHeaderCell(titleCell);

            AddColumnsToTable(table, values);

            doc.Add(table);
            doc.Add(new Paragraph(" ").SetMarginBottom(5));
        }

        private Cell CreateTitle(string title)
        {
            Cell cell = new Cell(1, PdfTableStyle.NumColumns)
                .Add(
                    new Paragraph(title)
                        .SetFont(PdfTableStyle.FontTitle)
                        .SetFontSize(PdfTableStyle.FontSizeHeaderTable)
                )
                .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

            return cell;
        }

        private void AddColumnsToTable(Table table, Dictionary<string, string> columns)
        {
            foreach(var value in columns)
            {
                var keyColumn = new Cell()
                    .Add(
                        new Paragraph(value.Key)
                            .SetFont(PdfTableStyle.FontColumns)
                            .SetFontSize(PdfTableStyle.FontSizeColumns)
                    )
                    .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

                table.AddCell(keyColumn);

                var valueColumn = new Cell()
                    .Add(
                        new Paragraph(value.Value)
                            .SetFont(PdfTableStyle.FontColumns)
                            .SetFontSize(PdfTableStyle.FontSizeColumns)
                    )
                    .SetBackgroundColor(PdfTableStyle.BackgroundColor)
                    .SetPadding(3)
                    .SetBorder(new SolidBorder(PdfTableStyle.BorderWidth));

                table.AddCell(valueColumn);
            }
        }
    }

    public class PdfTableStyle
    {
        public PdfFont FontTitle { get; set; }
        public PdfFont FontColumns { get; set; }
        public PdfFont FontTitleDocument { get; set; }

        public sbyte FontSizeTitle { get; set; }
        public sbyte FontSizeHeaderTable { get; set; }
        public sbyte FontSizeColumns { get; set; }

        public Color BackgroundColor { get; set; }
        public float BorderWidth { get; set; }
        public sbyte WidthPercentage { get; set; }
        public sbyte NumColumns { get; set; }

        public PdfTableStyle(
            PdfFont fontTitle = null,
            PdfFont fontColumns = null,
            PdfFont fontTitleDocument = null,
            sbyte? fontSizeTitle = null,
            sbyte? fontSizeHeaderTable = null,
            sbyte? fontSizeColumns = null,
            Color backgroundColor = null,
            float? borderWidth = null,
            sbyte? widthPercentage = null,
            sbyte? numColumns = null
        )
        {
            FontTitle = fontTitle ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            FontColumns = fontColumns ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            FontTitleDocument =
                fontTitleDocument ?? PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            FontSizeTitle = fontSizeTitle ?? 16;
            FontSizeHeaderTable = fontSizeHeaderTable ?? 12;
            FontSizeColumns = fontSizeColumns ?? 11;
            BackgroundColor = backgroundColor ?? new DeviceRgb(249, 249, 249);
            BorderWidth = borderWidth ?? 1.5f;
            WidthPercentage = widthPercentage ?? 80;
            NumColumns = numColumns ?? 2;
        }
    }
}
