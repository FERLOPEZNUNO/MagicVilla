using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MagicVilla_API.Datos
{


    public class ApplicationDbContext : DbContext
    { 
     
     //ahora indicamos que modelos queremos usar en nuestra BD:

        public DbSet<Villa> Villas { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options) 
        {
            
        }

        //esto es para tener datos "base" en la BD, ejemplos...

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Nombre = "Villa Real",
                    Detalle = "Detalle de la villa",
                    ImagenUrl = "",
                    Ocupantes = 5,
                    MetrosCuadrados = 180,
                    Tarifa = 100,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                },
                new Villa()
                {
                    Id = 2,
                    Nombre = "Villa Surreal",
                    Detalle = "Detalle de la villeke",
                    ImagenUrl = "",
                    Ocupantes = 12,
                    MetrosCuadrados = 550,
                    Tarifa = 600,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                }
                
                
                
                
                
                
                );
        }

    }
}