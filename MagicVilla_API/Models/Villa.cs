using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Models
{
    public class Villa
    {
        //para la base de datos se usa ESTO, el model, NO el dto.

        [Key] // <---- esto indica que id es la PK
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  // <- esto asigna el id (PK) automaticamente cuando se crean objetos
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        [Required]
        public double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public int MetrosCuadrados { get; set; }
        public string ImagenUrl { get; set; }
        public string Amenidad { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }

    }

}
