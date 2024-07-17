namespace MagicVilla_API.Models.Dto
{

    //en el DTO solo ponemos los atributos que queremos mostrar. aqui por ejemplo NO ponemos fecha creación, solo id y name
    public class VillaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}