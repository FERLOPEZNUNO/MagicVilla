using MagicVilla_API.Models.Dto; 
namespace MagicVilla_API.Datos

{   
    //clase para almacenar datos de las villas en defecto de una BD
    //es estática!!
    public static class VillaStore
    {
        public static List<VillaDto> villaList = new List<VillaDto>
        {

            new VillaDto {Id=1, Nombre="CuloVille"},
            new VillaDto {Id=2, Nombre="SegweyVilla"}

        };
    }
}
