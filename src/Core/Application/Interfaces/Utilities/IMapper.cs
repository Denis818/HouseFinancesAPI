namespace Application.Interfaces.Utilities
{
    public interface IMapper
    {
        T Map<T>(object origem);
        void Map<TOrigem, TDestino>(TOrigem origem, TDestino destino)
            where TOrigem : class
            where TDestino : class;
    }

}
