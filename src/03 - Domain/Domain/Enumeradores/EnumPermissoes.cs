using System.ComponentModel;

namespace Domain.Enumeradores
{
    public enum EnumPermissoes
    {
        /// <summary>
        /// Vizualizar acessos do usuario
        /// </summary>
        [Description("Vizualizar acessos do usuario")]
        USU_000001 = 1,
        [Description("Vizualizar logs de requests")]
        USU_000002
    }
}
