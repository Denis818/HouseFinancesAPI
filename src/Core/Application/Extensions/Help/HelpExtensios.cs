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
    }
}
