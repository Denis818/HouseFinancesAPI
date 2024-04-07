namespace Domain.Models.Users
{
    public class Permissao
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public virtual ICollection<Usuario> Usuarios { get; set; } = [];
    }
}
