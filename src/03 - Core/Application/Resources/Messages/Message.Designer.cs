﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Application.Resources.Messages {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Message {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Message() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Application.Resources.Messages.Message", typeof(Message).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ação inválida {0}..
        /// </summary>
        public static string AcaoNaoInvalida {
            get {
                return ResourceManager.GetString("AcaoNaoInvalida", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Essa categoria faz parta da regra de negócio. Não pode ser alterada..
        /// </summary>
        public static string AvisoCategoriaImutavel {
            get {
                return ResourceManager.GetString("AvisoCategoriaImutavel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Esse membro faz parta da regra de negócio. Não pode ser alterado..
        /// </summary>
        public static string AvisoMembroImutavel {
            get {
                return ResourceManager.GetString("AvisoMembroImutavel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Para cadastro de aluguel o campo &apos;item&apos; deve ser &apos;Parcela Ap Ponto&apos; ou &apos;Parcela Caixa&apos;..
        /// </summary>
        public static string CadastroAluguelIncorreto {
            get {
                return ResourceManager.GetString("CadastroAluguelIncorreto", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Campo {0} não está em um formato válido..
        /// </summary>
        public static string CampoFormatoInvalido {
            get {
                return ResourceManager.GetString("CampoFormatoInvalido", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deve existir pelo menos um Grupo..
        /// </summary>
        public static string DeletarUnicoGrupoDespesa {
            get {
                return ResourceManager.GetString("DeletarUnicoGrupoDespesa", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Despesa de {0} já adicionada ao grupo.
        /// </summary>
        public static string DespesaExistente {
            get {
                return ResourceManager.GetString("DespesaExistente", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Não há despesas {0} nesse mês..
        /// </summary>
        public static string DespesasNaoEncontradas {
            get {
                return ResourceManager.GetString("DespesasNaoEncontradas", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email não encontrado..
        /// </summary>
        public static string EmailNaoEncontrado {
            get {
                return ResourceManager.GetString("EmailNaoEncontrado", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ocorreu um erro ao {0}..
        /// </summary>
        public static string ErroAoSalvarNoBanco {
            get {
                return ResourceManager.GetString("ErroAoSalvarNoBanco", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} com id:{1} não existe..
        /// </summary>
        public static string IdNaoEncontrado {
            get {
                return ResourceManager.GetString("IdNaoEncontrado", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Modelo não é valido..
        /// </summary>
        public static string ModeloInvalido {
            get {
                return ResourceManager.GetString("ModeloInvalido", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} {1} já existe..
        /// </summary>
        public static string RegistroExistente {
            get {
                return ResourceManager.GetString("RegistroExistente", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Selecione um Grupo de Desesa..
        /// </summary>
        public static string SelecioneUmGrupoDesesa {
            get {
                return ResourceManager.GetString("SelecioneUmGrupoDesesa", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Senha inválida..
        /// </summary>
        public static string SenhaInvalida {
            get {
                return ResourceManager.GetString("SenhaInvalida", resourceCulture);
            }
        }
    }
}
