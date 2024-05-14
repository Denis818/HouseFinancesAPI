namespace Infraestructure.Data.Configurations
{
    public class CompanyConnectionStrings
    {
        public CompanyInfo[] List { get; set; }
    }

    public class CompanyInfo
    {
        public string NomeDominio { get; set; }
        public string ConnectionString { get; set; }
    }
}
