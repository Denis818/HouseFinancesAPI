using Domain.Dtos.Categoria;
using Domain.Dtos.Finance;
using Domain.Interfaces.Services.Finance;
using Domain.Models.Finance;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Domain.Services
{
    public class DespesasDomainService : IDespesasDomainService
    {
        public double CalculaTotalDespesaForaAlmocoAluguelCondominioContaDeLuz(
            List<Despesa> despesas,
            CategoriaIdsDto categoriaIds
        )
        {
            double total = despesas
                .Where(d =>
                    d.CategoriaId != categoriaIds.IdAluguel
                    && d.CategoriaId != categoriaIds.IdCondominio
                    && d.CategoriaId != categoriaIds.IdContaDeLuz
                    && d.CategoriaId != categoriaIds.IdAlmoco
                )
                .Sum(d => d.Total);

            return total;
        }

        public RelatorioGastosDoMesDto GetRelatorioDeGastosDoMes(
            string mesAtual,
            CategoriaIdsDto categIds,
            List<Despesa> despesas
        )
        {
            double aluguelMaisCondominio = despesas
                .Where(d =>
                    d.Categoria.Id == categIds.IdAluguel || d.Categoria.Id == categIds.IdCondominio
                )
                .Sum(d => d.Total);

            double totalGeral = despesas.Sum(d => d.Total);

            double totalGastosGerais = totalGeral - aluguelMaisCondominio;

            return new RelatorioGastosDoMesDto(
                mesAtual,
                aluguelMaisCondominio,
                totalGastosGerais,
                totalGeral
            );
        }

        public (double, double) CalculaTotalAlmocoDivididoComJhon(
            List<Despesa> despesas,
            int idAlmoco,
            int todosMembros
        )
        {
            double almoco = despesas.Where(d => d.CategoriaId == idAlmoco).Sum(d => d.Total);

            double almocoParteDoJhon = almoco / todosMembros;

            double almocoAbatido = almoco - almocoParteDoJhon;

            return (almocoAbatido, almocoParteDoJhon);
        }

        public byte[] PdfValoresAluguelCondominioLuz(
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

                #region Valores Iniciais
                PdfPTable tableValoresIniciais = CreatePdfTable("Valores Iniciais", 2);
                var columnsValoresIniciais = new Dictionary<string, string>
                {
                    { "Parcela do Apartamento", $"R$ {parcelaApartamento:F2}" },
                    { "Parcela da Caixa", $"R$ {parcelaCaixa:F2}" },
                    { "Conta de Luz", $"R$ {contaDeLuz:F2}" },
                    { "Condomínio", $"R$ {condominio:F2}" },
                };
                AddItemInTable(tableValoresIniciais, columnsValoresIniciais);

                #endregion

                #region Calculos
                PdfPTable tableCalculos = CreatePdfTable("Cálculos", 2);
                var columnsCalculos = new Dictionary<string, string>
                {
                    { "Parcela Apto mais Caixa", $"R$ {totalAptoMaisCaixa:F2}" },
                    { "Conta de Luz mais Condomínio", $"R$ {totalLuzMaisCondominio:F2}" },
                    {
                        "Apto mais Caixa menos R$ 300 do Peu",
                        $"R$ {totalAptoMaisCaixaAbate300Peu:F2}"
                    },
                    {
                        "Conta de Luz e Condomínio menos R$ 100 do estacionamento",
                        $"R$ {totalLuzMaisCondominioAbate100Estacionamento:F2}"
                    },
                };
                AddItemInTable(tableCalculos, columnsCalculos);

                #endregion

                #region Parcela do Apto e Caixa para cada
                PdfPTable tableAptoCaixaParaCada = CreatePdfTable(
                    "Parcela do Apto e Caixa para cada",
                    2
                );
                foreach(var membro in listMembroForaJhonPeu)
                {
                    var columnsAptoCaixaParaCada = new Dictionary<string, string>
                    {
                        { membro.Nome, $"R$ {valorAptoMaisCaixaParaCadaMembro:F2}" }
                    };

                    AddItemInTable(tableAptoCaixaParaCada, columnsAptoCaixaParaCada);
                }
                #endregion

                #region Conta de Luz e Condomínio para cada
                PdfPTable tableLuzCondominioParaCada = CreatePdfTable(
                    "Conta de Luz e Condomínio para cada",
                    2
                );
                foreach(var membro in listMembroForaJhon)
                {
                    var columnsLuzCondominioParaCada = new Dictionary<string, string>
                    {
                        { membro.Nome, $"R$ {valorLuzMaisCondominioParaCadaMembro:F2}" }
                    };

                    AddItemInTable(tableLuzCondominioParaCada, columnsLuzCondominioParaCada);
                }
                #endregion

                #region Valor que cada um deve pagar
                PdfPTable tableTotalParaCada = CreatePdfTable("Valor que cada um deve pagar", 2);
                foreach(var membro in listMembroForaJhonPeu)
                {
                    var columnsTotalParaCada = new Dictionary<string, string>
                    {
                        { membro.Nome, $"R$ {valorParaMembrosForaPeu:F2}" }
                    };

                    AddItemInTable(tableTotalParaCada, columnsTotalParaCada);
                }
                AdicionarLinha(tableTotalParaCada, "Peu", $"R$ {valorParaDoPeu:F2}");
                #endregion

                PdfPTable[] pdfPTables =
                {
                    tableValoresIniciais,
                    tableCalculos,
                    tableAptoCaixaParaCada,
                    tableLuzCondominioParaCada,
                    tableTotalParaCada,
                };

                AddRangeTables(doc, pdfPTables);

                doc.Close();
            }

            return memoryStream.ToArray();
        }

        public void AddItemInTable(PdfPTable table, Dictionary<string, string> columns)
        {
            foreach(var column in columns)
            {
                AdicionarLinha(table, column.Key, column.Value);
            }
        }

        #region Metodos de Suporte para PDF
        private PdfPTable CreatePdfTable(string title, int numColumns)
        {
            PdfPTable pdfTable = new(numColumns) { WidthPercentage = 110 };

            PdfPCell titleTable = CreateTitle(title);

            pdfTable.AddCell(titleTable);

            return pdfTable;
        }

        private PdfPCell CreateTitle(string title)
        {
            Font boldFont = new(Font.FontFamily.HELVETICA, 11, Font.BOLD);

            return new(new Phrase(title, boldFont))
            {
                Colspan = 2,
                BorderWidth = 1.5f,
                HorizontalAlignment = Element.ALIGN_CENTER,
                BackgroundColor = new(247, 242, 255),
                Padding = 7,
            };
        }

        private PdfPCell CreateColumn(string content)
        {
            Font font = new(Font.FontFamily.HELVETICA, 10, Font.NORMAL);

            PdfPCell cell =
                new(new Phrase(content, font))
                {
                    BorderWidth = 1.5f,
                    Padding = 7,
                    BackgroundColor = new(247, 242, 255),
                };
            return cell;
        }

        private void AdicionarLinha(PdfPTable table, string coluna1, string coluna2)
        {
            PdfPCell cell1 = CreateColumn(coluna1);
            PdfPCell cell2 = CreateColumn(coluna2);

            table.AddCell(cell1);
            table.AddCell(cell2);
        }

        private void AddRangeTables(Document doc, PdfPTable[] pdfPTables)
        {
            foreach(var pdfPTable in pdfPTables)
            {
                doc.Add(pdfPTable);
                doc.Add(new Paragraph(" "));
            }
        }
        #endregion
    }
}
