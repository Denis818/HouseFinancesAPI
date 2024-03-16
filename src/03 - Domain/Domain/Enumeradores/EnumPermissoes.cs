using System.ComponentModel;

namespace Domain.Enumeradores
{
    public enum EnumPermissoes
    {
        /// <summary>
        /// Permite ao usuário criar, atualizar e deletar Despesas e Membros
        /// </summary>
        [Description("Permite ao usuário criar, atualizar e deletar Despesas e Membros")]
        USU_000001 = 1,

        /// <summary>
        /// Vizualizar logs de requests
        /// </summary>
        [Description("Vizualizar logs de requests")]
        USU_000002 = 2
    }
}
