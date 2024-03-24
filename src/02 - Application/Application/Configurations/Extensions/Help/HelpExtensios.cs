namespace Application.Configurations.Extensions.Help
{
    public static class HelpExtensios
    {
        public static decimal RoundTo(this decimal value, int decimalPlaces)
            => Math.Round(value, decimalPlaces);

    }
}
