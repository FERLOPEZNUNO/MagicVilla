using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Models.Dto
{

    //en el DTO solo ponemos los atributos que queremos mostrar. aqui por ejemplo NO ponemos fecha creación, solo id y name
    public class VillaCreateDto
    {
        //el id es auto, no hace falta.
        //   public int Id { get; set; }

        //esto de abajo son dataannotations. sirve para establecer otras limitaciones, como por ej maximo 30 caracteres en el name de la villa
        //también es requerido, no se puede dejar en blanco
        [Required]
        [MaxLength(30)]

        public string Nombre { get; set; }
        public string Detalle { get; set; }
        public double Tarifa { get; set; }
        public int MetrosCuadrados { get; set; }
        public int Ocupantes { get; set; }
        public string ImagenUrl { get; set; }
        public string Amenidad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}