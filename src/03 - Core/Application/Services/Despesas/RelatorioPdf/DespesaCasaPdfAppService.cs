using Application.Helpers;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Models.Membros;
using iText.Kernel.Pdf;
using iText.Layout;

namespace Application.Services.Despesas.RelatorioPdf
{
    public class DespesaCasaPdfAppService
    {
        private readonly PdfTableHelper _pdfTable = new();

        public byte[] GerarRelatorioDespesaCasaPdf(DistribuicaoCustosCasaDto custosCasaDto)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var mergedDocument = new PdfDocument(writer);
            using var doc = new Document(mergedDocument);

            _pdfTable.CreateTitleDocument(doc, "Relatório detalhado dos valores divididos");

            CreateTableCalculos(
                doc,
                custosCasaDto.TotalDespesaGerais,
                custosCasaDto.TotalAlmocoDividioComJhon,
                custosCasaDto.TotalAlmocoParteDoJhon,
                custosCasaDto.DespesaGeraisMaisAlmoco
            );

            CreateTableValoresParaCada(
                doc,
                custosCasaDto.IdJhon,
                custosCasaDto.Membros,
                custosCasaDto.DespesaGeraisMaisAlmocoDividioPorMembro,
                custosCasaDto.TotalAlmocoParteDoJhon
            );

            doc.Close();

            return memoryStream.ToArray();
        }

        public void CreateTableCalculos(
            Document doc,
            double totalDespesaGerais,
            double totalAlmocoDividioComJhon,
            double totalAlmocoParteDoJhon,
            double despesaGeraisMaisAlmoco
        )
        {
            var columnsValoresCalculados = new Dictionary<string, string>
            {
                { "Despesas gerais fora almoço", $"R$ {totalDespesaGerais:F2}" },
                { "Almoço divido com o Jhon", $"R$ {totalAlmocoDividioComJhon:F2}" },
                { "Almoço parte do Jhon", $"R$ {totalAlmocoParteDoJhon:F2}" },
                {
                    "Despesa gerais somado com Almoço divido com o Jhon",
                    $"R$ {despesaGeraisMaisAlmoco:F2}"
                },
            };

            _pdfTable.CreateTable(
                doc,
                "Despesas da casa (Fora despesa Habitacional como aluguel, luz etc...)",
                columnsValoresCalculados
            );
        }

        public void CreateTableValoresParaCada(
            Document doc,
            int idJhon,
            List<Membro> membros,
            double despesaGeraisMaisAlmocoDividioPorMembro,
            double totalAlmocoParteDoJhon
        )
        {
            Dictionary<string, string> columnsTotalParaCada = [];
            foreach (var membro in membros)
            {
                double valorParaCada = despesaGeraisMaisAlmocoDividioPorMembro;

                if (membro.Id == idJhon)
                {
                    valorParaCada = totalAlmocoParteDoJhon;
                }

                columnsTotalParaCada.Add(membro.Nome, $"{valorParaCada}");
            }

            _pdfTable.CreateTable(
                doc,
                "Valor que cada um deve pagar",
                columnsTotalParaCada
            );
        }
    }
}
