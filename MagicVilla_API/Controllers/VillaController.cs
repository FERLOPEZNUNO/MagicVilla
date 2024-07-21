using MagicVilla_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Datos;
using Microsoft.AspNetCore.JsonPatch;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //esto es 1 endpoint. éste devuelve TODAS las villas.
        //IEnumerable: nos va a devolver una lista.
        //Villa: va a ser una lista de villas.
        //GetVillas -> nombre del método (endpoint).
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //el endpoint ha de ser diferente cada vez. no puedo usar otro llamado igual a este.
        //el ActionResult nos permite que el método devuelva o bien una lista de villaDtos o bien una action (ok, 404, etc)
        public ActionResult <IEnumerable<VillaDto>> GetVillas()
        {
            //aqui el OK va a devolver 1 mensaje, junto con la lista, que diga que todo ha salido bien.
            return Ok (VillaStore.villaList);
        }
        // el firstorDefault comprueba toda la list, elemento a elemento. returneará el 1er (first) elemento que cumpla
        //la condición establecida en el paréntesis, es decir, que la id de ese objeto analizado sea igual
        //a la id pasada por param.
        //los "ProducesResponse..." es documentacion de los posibles mensajes/errores que pueden salir.
        //el {Id:int} es un "parametro variable de ruta". es lo que se introducirá y puede ser variable. tb se usará en el delete, por ej.
        //va siempre entre claves porque sí

        [HttpGet("{Id:int}", Name="GetVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult <VillaDto> GetVilla(int id)
        {
            //si el id solicitado es 0 saldrá un error badrequest.
            if (id == 0)
            {
                return BadRequest();
            }

            //creamos una variable "villa" que contendrá lo mismo que el return de abajo (que es el objeto cuya id se ha solicitado)
            // si dicha variable no existe, saldrá un 404 (not found).
            var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            return  Ok (VillaStore.villaList.FirstOrDefault(villa => villa.Id == id));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //el fromBody coge lo que introducirán en la solicitud (id, nombre, etc), que por lo general
        //estará en formato json, y lo convertirá en 1 instancia de VillaDto (deserializacion).
        public ActionResult<VillaDto> CrearVilla([FromBody] VillaDto villaNueva)
        {
            //justo abajo, relacionado con el Required de VillaDTo: se comprueba que el name introducido sea válido 
            //de acuerdo con la regla de que no puede ser más largo de 30 chars.
            //Si nuestro modelo NO es válido... 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //tb se pueden hacer validaciones personalizadas: en este caso, se consulta el storage
            //para ver si el nombre usado ya existe en otra villa.
            //Se itera por todos los nombres de villas hasta que se encuentre 1 con mismo nombre; ambos se ponen en lower
            //para facilitar la comparación. si la busqueda NO resulta null (es decir, si sí se encuentra nombre repe), se devuelve el error. 
            if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaNueva.Nombre.ToLower()) != null)
            {
                //para añadir una validacion personalizada; 2 params: nombre de la validacion y mensaje
                ModelState.AddModelError("repe", "nombre de villa ya existe, tontin");
                return BadRequest(ModelState); 
            }


            if (villaNueva == null)
            {
                return BadRequest();
            }
            //el id debe ser generado AUTO, por lo que si introducen id dará error 500; si es 0 está ok, ya que más adelante se cambia.
            //por ello cuando se introduzca el post hay que dejar en 0 la ID
            if (villaNueva.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            //para generar automaticamente el id: villaNueva.Id es la id de la villa que estamos creando.
            //Ahí asignaremos el num máximo de id que exista en el Storage. Para ello ordenamos la lista del Storage de forma
            //descendente por el ID (con la lambda) y cogemos el 1er dato que haya, que será el más grande, y le añadimos 1.
            //Este será el ID de nuestra nueva villa creada con este POST

            villaNueva.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;

            //para añadir 1 nuevo objeto a nuestra lista del storage:

            VillaStore.villaList.Add(villaNueva);

            //el CreatedAtRoute es tipo el ok, pero es el 201 en lugar del 200: indica que se ha creado exitosamente el elemento.
            //dentro del paréntesis indicamos 1.- el name el endpoint GetVilla, 2.-la id de la villa en cuestión y 3.- el objeto entero
            return CreatedAtRoute("GetVilla", new { id = villaNueva.Id }, villaNueva);

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //se usa Iactionresult en lugar de action... cuando se usan deletes, ya que hay que devolver "noContent...". para put, por ej, tb.
        public IActionResult DeleteVilla (int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(v=>v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            //si no se cumplen los errores de arriba, se borra la villa y se returnea el mensaje de éxito
            VillaStore.villaList.Remove(villa);

            //siempre que hagamos deletes hay que devolver un NO CONENT.    
            return NoContent();
        }

        //para actualizar hay PUT, que actualiza TODO el contenido del objeto, y PATCH, que solo actualiza 1 elemento
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //como params, recibe el id de la villa a modificar Y el objeto villa (frombody: es decir, cogerá los elementos de la solicitud
        //http y los convertirá en un objeto tipo VillaDto llamado villaActualizada).
        public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaActualizada)
        {
             if (villaActualizada ==null || id != villaActualizada.Id)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            villa.Nombre=villaActualizada.Nombre;
            villa.Ocupantes = villaActualizada.Ocupantes;
            villa.MetrosCuadrados = villaActualizada.MetrosCuadrados;

            return NoContent();

        }

        //para hacer el patch hay que bajarse nugets. tools -> nuget package manager  -> manage nuget pckgs
        //una vez instalados los nugets (ojo, version nuget y la del proyecto ha de ser = !!! ), vamos a program.cs
        //y ahi colocamos los nuevos servicios: detras de builder.Services.AddControllers() colocamos:  ..AddNewtonsoftJson();
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //como params, recibe el id de la villa a modificar Y un jsonPatchDocument).
        public IActionResult UpdateParcialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            patchDto.ApplyTo(villa, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return NoContent();

        }


    }
}
    