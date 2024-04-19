using Application.Helpers;
using Domain.Dtos.Categoria;
using Domain.Models.Finance;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Application.Services.Finance
{
    public class RelatorioDespesasPdfAppServices
    {
        public PdfTableHelper _pdfTable = new();

        public byte[] DownloadPdfRelatorioDeDespesas(
            List<Despesa> despesas,
            List<Membro> membros,
            CategoriaIdsDto categoriaIds,
            int idJhon,
            int idPeu
        )
        {
            #region Valores calculados
            var listAluguel = despesas.Where(despesa =>
                despesa.CategoriaId == categoriaIds.IdAluguel
            );

            var listMembroForaJhon = membros.Where(membro => membro.Id != idJhon).ToList();
            var listMembroForaJhonPeu = listMembroForaJhon
                .Where(membro => membro.Id != idPeu)
                .ToList();

            double? parcelaApartamento = listAluguel
                .Where(aluguel =>
                    aluguel.Item.Contains("ap ponto", StringComparison.CurrentCultureIgnoreCase)
                )
                ?.First()
                ?.Preco;

            double? parcelaCaixa = listAluguel
                .Where(aluguel =>
                    aluguel.Item.Contains("caixa", StringComparison.CurrentCultureIgnoreCase)
                )
                ?.First()
                ?.Preco;

            double? contaDeLuz = despesas
                .Where(despesa => despesa.CategoriaId == categoriaIds.IdContaDeLuz)
                ?.First()
                ?.Preco;

            double? condominio = despesas
                .Where(despesa => despesa.CategoriaId == categoriaIds.IdCondominio)
                ?.First()
                ?.Preco;

            double? totalAptoMaisCaixa = parcelaApartamento + parcelaCaixa;

            double? totalLuzMaisCondominio = contaDeLuz + condominio;

            double? totalAptoMaisCaixaAbate300Peu = totalAptoMaisCaixa - 300; //300 aluguel cobrado do peu
            double? totalLuzMaisCondominioAbate100Estacionamento = totalLuzMaisCondominio - 100; //estacionamento alugado

            double? valorAptoMaisCaixaParaCadaMembro =
                totalAptoMaisCaixaAbate300Peu / listMembroForaJhonPeu.Count;

            double? valorLuzMaisCondominioParaCadaMembro =
                totalLuzMaisCondominioAbate100Estacionamento / listMembroForaJhon.Count;

            double? valorParaMembrosForaPeu =
                valorAptoMaisCaixaParaCadaMembro + valorLuzMaisCondominioParaCadaMembro;

            double? valorParaDoPeu = 300 + valorLuzMaisCondominioParaCadaMembro;
            #endregion

            using MemoryStream memoryStream = new();
            using(Document doc = new(PageSize.A4))
            {
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                _pdfTable.CreateTitleDocument(doc, "Relatório detalhado dos valores divididos");

                TableValoresIniciais(doc, parcelaApartamento, parcelaCaixa, contaDeLuz, condominio);

                TableCalculos(
                    doc,
                    totalAptoMaisCaixa,
                    totalLuzMaisCondominio,
                    totalAptoMaisCaixaAbate300Peu,
                    totalLuzMaisCondominioAbate100Estacionamento
                );

                TableParcelaCaixaApto(doc, listMembroForaJhonPeu, valorAptoMaisCaixaParaCadaMembro);

                TableContaLuzAndCondominio(
                    doc,
                    listMembroForaJhon,
                    valorLuzMaisCondominioParaCadaMembro
                );

                TableValoresParaCada(
                    doc,
                    listMembroForaJhon,
                    valorParaMembrosForaPeu,
                    valorParaDoPeu
                );

                doc.Close();
            }

            return memoryStream.ToArray();
        }

        public void TableValoresIniciais(
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

        public void TableCalculos(
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

        public void TableParcelaCaixaApto(
            Document doc,
            List<Membro> listMembroForaJhonPeu,
            double? valorAptoMaisCaixaParaCadaMembro
        )
        {
            Dictionary<string, string> columnsAptoCaixaParaCada = [];

            foreach(var membro in listMembroForaJhonPeu)
            {
                columnsAptoCaixaParaCada.Add(membro.Nome, $"R$ {valorAptoMaisCaixaParaCadaMembro:F2}");
            }

            _pdfTable.CreateTable(
                doc,
                "Parcela do Apto e Caixa para cada",
                columnsAptoCaixaParaCada
            );
        }

        public void TableContaLuzAndCondominio(
            Document doc,
            List<Membro> listMembroForaJhon,
            double? valorLuzMaisCondominioParaCadaMembro
        )
        {
            Dictionary<string, string> columnsLuzCondParaCada = [];

            foreach(var membro in listMembroForaJhon)
            {
                columnsLuzCondParaCada.Add(membro.Nome, $"R$ {valorLuzMaisCondominioParaCadaMembro:F2}");
                
            }

            _pdfTable.CreateTable(
                doc,
                "Conta de Luz e Condomínio para cada",
                columnsLuzCondParaCada
            );
        }

        public void TableValoresParaCada(
            Document doc,
            List<Membro> listMembroForaJhon,
            double? valorParaMembrosForaPeu,
            double? valorParaDoPeu
        )
        {
            Dictionary<string, string> columnsTotalParaCada = [];

            foreach(var membro in listMembroForaJhon)
            {
                var valorParaCada = valorParaMembrosForaPeu;

                if(membro.Id == 2)
                {
                    valorParaCada = valorParaDoPeu;
                }

                columnsTotalParaCada.Add(membro.Nome, $"R$ {valorParaCada:F2}");                               
            }
            _pdfTable.CreateTable(doc, "Valor que cada um deve pagar", columnsTotalParaCada);
        }
    }
}
