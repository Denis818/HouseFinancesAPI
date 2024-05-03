using Application.Helpers;
using Domain.Dtos.Despesas.Relatorios;
using Domain.Models.Membros;
using iText.Kernel.Pdf;
using iText.Layout;

namespace Application.Services.Despesas.RelatorioPdf
{
    public class DespesaMoradiaPdfReport
    {
        private readonly PdfTableHelper _pdfTable = new();

        public byte[] GerarRelatorioDespesaMoradiaPdf(DetalhamentoDespesasMoradiaDto custosMoradia)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new PdfWriter(memoryStream);
            using var mergedDocument = new PdfDocument(writer);
            using var doc = new Document(mergedDocument);

            _pdfTable.CreateTitleDocument(doc, "Relatório detalhado dos valores divididos");

            TableValoresIniciais(
                doc,
                custosMoradia.CustosDespesasMoradia.ParcelaApartamento,
                custosMoradia.CustosDespesasMoradia.ParcelaCaixa,
                custosMoradia.CustosDespesasMoradia.ContaDeLuz,
                custosMoradia.CustosDespesasMoradia.Condominio
            );

            TableCalculos(
                doc,
                custosMoradia.DistribuicaoCustos.TotalAptoMaisCaixa,
                custosMoradia.DistribuicaoCustos.TotalLuzMaisCondominio,
                custosMoradia.DistribuicaoCustos.TotalAptoMaisCaixaAbate300Peu,
                custosMoradia.DistribuicaoCustos.TotalLuzMaisCondominioAbate100Estacionamento
            );

            TableParcelaCaixaApto(
                doc,
                custosMoradia.GrupoListMembrosDespesa.ListMembroForaJhon, //ListMembroForaJhon,
                custosMoradia.DistribuicaoCustos.ValorAptoMaisCaixaParaCadaMembro
            );

            TableContaLuzAndCondominio(
                doc,
                custosMoradia.GrupoListMembrosDespesa.ListMembroForaJhon,
                custosMoradia.DistribuicaoCustos.ValorLuzMaisCondominioParaCadaMembro
            );

            TableValoresParaCada(
                doc,
                custosMoradia.GrupoListMembrosDespesa.ListMembroForaJhon,
                custosMoradia.DistribuicaoCustos.ValorParaMembrosForaPeu,
                custosMoradia.DistribuicaoCustos.ValorParaDoPeu
            );

            doc.Close();

            return memoryStream.ToArray();
        }

        private void TableValoresIniciais(
            Document doc,
            double? parcelaApartamento,
            double? parcelaCaixa,
            double? contaDeLuz,
            double? condominio
        )
        {
            var columnsValoresIniciais = new Dictionary<string, string>
            {
                { "Parcela do Apartamento", $"R$ {parcelaApartamento:F2}" },
                { "Parcela da Caixa", $"R$ {parcelaCaixa:F2}" },
                { "Conta de Luz", $"R$ {contaDeLuz:F2}" },
                { "Condomínio", $"R$ {condominio:F2}" },
            };
            _pdfTable.CreateTable(doc, "Valores Iniciais", columnsValoresIniciais);
        }

        private void TableCalculos(
            Document doc,
            double? totalAptoMaisCaixa,
            double? totalLuzMaisCondominio,
            double? totalAptoMaisCaixaAbate300Peu,
            double? totalLuzMaisCondominioAbate100Estacionamento
        )
        {
            var columnsCalculos = new Dictionary<string, string>
            {
                { "Parcela Apto mais Caixa", $"R$ {totalAptoMaisCaixa:F2}" },
                { "Conta de Luz mais Condomínio", $"R$ {totalLuzMaisCondominio:F2}" },
                { "Apto mais Caixa menos R$ 300 do Peu", $"R$ {totalAptoMaisCaixaAbate300Peu:F2}" },
                {
                    "Conta de Luz e Condomínio menos R$ 100 do estacionamento",
                    $"R$ {totalLuzMaisCondominioAbate100Estacionamento:F2}"
                },
            };
            _pdfTable.CreateTable(doc, "Cálculos", columnsCalculos);
        }

        private void TableParcelaCaixaApto(
            Document doc,
            IList<Membro> listMembroForaJhon,
            double? valorAptoMaisCaixaParaCadaMembro
        )
        {
            Dictionary<string, string> columnsAptoCaixaParaCada = [];

            foreach(var membro in listMembroForaJhon)
            {
                var valorAluguel = valorAptoMaisCaixaParaCadaMembro;

                if(membro.Nome.Contains("Peu", StringComparison.CurrentCultureIgnoreCase))
                {
                    valorAluguel = 300;
                }

                columnsAptoCaixaParaCada.Add(membro.Nome, $"R$ {valorAluguel:F2}");
            }

            _pdfTable.CreateTable(
                doc,
                "Parcela do Apto e Caixa para cada",
                columnsAptoCaixaParaCada
            );
        }

        private void TableContaLuzAndCondominio(
            Document doc,
           IList<Membro> listMembroForaJhon,
            double? valorLuzMaisCondominioParaCadaMembro
        )
        {
            Dictionary<string, string> columnsLuzCondParaCada = [];

            foreach(var membro in listMembroForaJhon)
            {
                columnsLuzCondParaCada.Add(
                    membro.Nome,
                    $"R$ {valorLuzMaisCondominioParaCadaMembro:F2}"
                );
            }

            _pdfTable.CreateTable(
                doc,
                "Conta de Luz e Condomínio para cada",
                columnsLuzCondParaCada
            );
        }

        private void TableValoresParaCada(
            Document doc,
           IList<Membro> listMembroForaJhon,
            double? valorParaMembrosForaPeu,
            double? valorParaDoPeu
        )
        {
            Dictionary<string, string> columnsTotalParaCada = [];

            foreach(var membro in listMembroForaJhon)
            {
                var valorParaCada = valorParaMembrosForaPeu;

                if(membro.Nome.Contains("Peu", StringComparison.CurrentCultureIgnoreCase))
                {
                    valorParaCada = valorParaDoPeu;
                }

                columnsTotalParaCada.Add(membro.Nome, $"R$ {valorParaCada:F2}");
            }
            _pdfTable.CreateTable(doc, "Valor que cada um deve pagar", columnsTotalParaCada);
        }
    }
}
