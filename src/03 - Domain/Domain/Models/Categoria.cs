namespace Domain.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public ICollection<Despesa> Despesas { get; set; } = [];

    }
}

/*INSERT INTO Categorias (Nome) 
VALUES 
('Almoço/Janta'),
('Casa'),
('Limpeza'),
('Aluguel'),
('Lanches'),
('Higiêne Pessoal'),
('Internet'),
('Água'),
('Energia Elétrica');*/
