namespace Application.Extensions.Help
{
    public static class HelpExtensios
    {
        public static double RoundTo(this double soucer, int doublePlaces) =>
            Math.Round(soucer, doublePlaces);
    }
}
