using Application.Interfaces.Utilities;

namespace Application.Utilities
{
    public class Mapper : IMapper
    {
        public T Map<T>(object origem)
        {
            ArgumentNullException.ThrowIfNull(origem);

            var tipoOrigem = origem.GetType();
            var tipoDestino = typeof(T);

            return (T)MapObjetoParaEntidade(origem, tipoOrigem, tipoDestino);
        }

        public void Map<TOrigem, TDestino>(TOrigem origem, TDestino destino)
            where TOrigem : class
            where TDestino : class
        {
            ArgumentNullException.ThrowIfNull(origem);
            ArgumentNullException.ThrowIfNull(destino);

            MapObjetoParaObjeto(origem, destino, typeof(TOrigem), typeof(TDestino));
        }

        private object MapObjetoParaEntidade(object origem, Type tipoOrigem, Type tipoDestino)
        {
            var mapa = new Mapa(tipoOrigem, tipoDestino);

            var objetoDestino = Activator.CreateInstance(mapa.TipoDestino);

            CopiarPropriedades(origem, objetoDestino, mapa);

            return objetoDestino;
        }

        private void MapObjetoParaObjeto(
            object origem,
            object destino,
            Type tipoOrigem,
            Type tipoDestino
        )
        {
            var mapa = new Mapa(tipoOrigem, tipoDestino);

            CopiarPropriedades(origem, destino, mapa);
        }

        private void CopiarPropriedades(object origem, object destino, Mapa mapa)
        {
            foreach(var propOrigem in mapa.TipoOrigem.GetProperties())
            {
                var propDestino = mapa.TipoDestino.GetProperty(propOrigem.Name);
                if(propDestino != null
                    && propDestino.CanWrite
                    && propOrigem.PropertyType == propDestino.PropertyType)
                {
                    var valor = propOrigem.GetValue(origem);
                    propDestino.SetValue(destino, valor);
                }
            }
        }
    }

    public record Mapa(Type TipoOrigem, Type TipoDestino);
}
