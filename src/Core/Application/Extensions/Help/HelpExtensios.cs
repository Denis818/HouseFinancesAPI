using System.Globalization;

namespace Application.Extensions.Help
{
    public static class HelpExtensios
    {
        public static double RoundTo(this double soucer, int doublePlaces)
        {
            if (soucer <= 0)
                return soucer;

            return Math.Round(soucer, doublePlaces);
        }

        public static double RountToZeroIfNegative(this double soucer) => Math.Max(soucer, 0);

        public static string ToFormatPrBr(this double soucer) =>
            soucer.ToString("F2", new CultureInfo("pt-BR"));
    }
}
