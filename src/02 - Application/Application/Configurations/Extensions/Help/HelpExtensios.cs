namespace Application.Configurations.Extensions.Help
{
    public static class HelpExtensios
    {
        public static decimal RoundTo(this decimal soucer, int decimalPlaces)
            => Math.Round(soucer, decimalPlaces);
    }
}
