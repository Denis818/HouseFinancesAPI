using System.ComponentModel;

namespace Domain.Enumeradores
{
    public enum EnumPermissoes
    {
        /// <summary>
        /// Permissão para atualizar.
        /// </summary>
        [Description("Permissao para atualizar.")]
        USU_000001 = 1,

        /// <summary>
        /// Permissão para deletar.
        /// </summary>
        [Description("Permissao para deletar.")]
        USU_000002 = 2,

        /// <summary>
        /// Permissão para criar.
        /// </summary>
        [Description("Permissao para criar.")]
        USU_000003 = 3,

        /// <summary>
        /// Permissão para atualizar outros membros.
        /// </summary>
        [Description("Permissao para atualizar outros membros.")]
        USU_000004 = 4,

        /// <summary>
        /// Permissão para deletar outros membros.
        /// </summary>
        [Description("Permissao para deletar outros membros.")]
        USU_000005 = 5,
    }
}
