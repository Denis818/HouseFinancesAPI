using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Application.Helpers
{
    public class PdfTableHelper
    {
        private readonly Font FontTitle = new(Font.FontFamily.HELVETICA, 11, Font.BOLD);
        private readonly Font FontColumns = new(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
        private readonly Font FontTitleDocument = new(Font.FontFamily.HELVETICA, 16, Font.BOLD);

        private readonly BaseColor BackgroundColor = new(249, 249, 249);

        private readonly float BorderWidth = 1.5f;
        private readonly int Alinhamento = Element.ALIGN_CENTER;
        private readonly int Padding = 7;
        private readonly int WidthPercentage = 70;
        private readonly int NumColumns = 2;
        private readonly int Colspan = 2;


        public void CreateTitleDocument(Document doc, string title)
        {
            Paragraph titleDocument = new(title, FontTitleDocument)
            {
                Alignment = Alinhamento,
                SpacingAfter = 50
            };

            doc.Add(titleDocument);
        }

        public void CreateTable(Document doc, string title, Dictionary<string, string> values)
        {
            PdfPTable pdfTable = new(NumColumns) { WidthPercentage = WidthPercentage };

            PdfPCell titleTable = CreateTitle(title);

            pdfTable.AddCell(titleTable);

            AddItemInTable(pdfTable, values);

            doc.Add(pdfTable);
            doc.Add(new Paragraph(" "));
        }

        public PdfPCell CreateTitle(string title)
        {
            return new(new Phrase(title, FontTitle))
            {
                Colspan = Colspan,
                BorderWidth = BorderWidth,
                HorizontalAlignment = Alinhamento,
                BackgroundColor = BackgroundColor,
                Padding = Padding,
            };
        }

        public PdfPCell CreateColumn(string content)
        {
            PdfPCell cell =
                new(new Phrase(content, FontColumns))
                {
                    BorderWidth = BorderWidth,
                    Padding = Padding,
                    BackgroundColor = BackgroundColor,
                };
            return cell;
        }

        public void AddItemInTable(PdfPTable table, Dictionary<string, string> columns)
        {
            foreach(var column in columns)
            {
                table.AddCell(CreateColumn(column.Key));
                table.AddCell(CreateColumn(column.Value));
            }
        }
    }
}
