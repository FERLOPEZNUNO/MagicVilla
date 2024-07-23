using MagicVilla_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Datos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {

        //logger: registra info, mensajes y errores
        // a las variables privadas se les pone guion bajo por convención
        private readonly ILogger<VillaController> _loggerFer;

        //inyectamos el servicio dbcontext:
        private readonly ApplicationDbContext _dbFer;

        //inyectamos el servicio mapper:
        private readonly IMapper _mapeador;

        //constructor donde vamos a inyectar dependencias: nuestro servicio de logger y el de la database, y el del maper.
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {
            _loggerFer = logger;
            _dbFer = db;
            _mapeador = mapper;
        }



        //esto es 1 endpoint. éste devuelve TODAS las villas.
        //IEnumerable: nos va a devolver una lista.
        //Villa: va a ser una lista de villas.
        //GetVillas -> nombre del método (endpoint).
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //el endpoint ha de ser diferente cada vez. no puedo usar otro llamado igual a este.
        //el ActionResult nos permite que el método devuelva o bien una lista de villaDtos o bien una action (ok, 404, etc)
        public async Task <ActionResult <IEnumerable<VillaDto>>> GetVillas()
        {
            //ejemplo uso del logger - el msg saldrá en la consola:
            _loggerFer.LogInformation("Obtener todas las villas");

            //creamos 1 lista para luego usar el mapper (IEnumberable es 1 interfaz que permite iterar en una coleccion):
            IEnumerable<Villa> villaList =  await _dbFer.Villas.ToListAsync();

            //aqui el OK va a devolver 1 mensaje, junto con la lista, que diga que todo ha salido bien.
            // antiguo, con store:
            // return Ok (VillaStore.villaList);

            //el de la DB: hacemos un tolist para que lo devuelva en forma de lista.
            //return Ok(_dbFer.Villas.ToList());

            //mejorado con async:
            //return Ok(await _dbFer.Villas.ToListAsync());  

            //con el mapper. devolvemos una lista IEnumberable de tipo VillaDto y obtendremos la info de villaList:
            return Ok(_mapeador.Map<IEnumerable<VillaDto>>(villaList));

        }
        // el firstorDefault comprueba toda la list, elemento a elemento. returneará el 1er (first) elemento que cumpla
        //la condición establecida en el paréntesis, es decir, que la id de ese objeto analizado sea igual
        //a la id pasada por param.
        //los "ProducesResponse..." es documentacion de los posibles mensajes/errores que pueden salir.
        //el {Id:int} es un "parametro variable de ruta". es lo que se introducirá y puede ser variable. tb se usará en el delete, por ej.
        //va siempre entre claves porque sí

        [HttpGet("Id:int", Name="GetVilla")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <ActionResult <VillaDto>> GetVilla(int id)
        {
            //si el id solicitado es 0 saldrá un error badrequest.
            if (id == 0)
            {
                return BadRequest();
            }

            //creamos una variable "villa" que contendrá lo mismo que el return de abajo (que es el objeto cuya id se ha solicitado)
            // si dicha variable no existe, saldrá un 404 (not found).
            //lo de abajo es para Store, no sirve con db.
            //var villa = VillaStore.villaList.FirstOrDefault(villa => villa.Id == id);

            var villa = await _dbFer.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            //sin automapper:
            //     return  Ok (villa);

            //con:
            return Ok(_mapeador.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //el fromBody coge lo que introducirán en la solicitud (id, nombre, etc), que por lo general
        //estará en formato json, y lo convertirá en 1 instancia de VillaDto (deserializacion).
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto villaNueva)
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

            //antiguo:
            /*
            if (VillaStore.villaList.FirstOrDefault(v => v.Nombre.ToLower() == villaNueva.Nombre.ToLower()) != null)
            {
                //para añadir una validacion personalizada; 2 params: nombre de la validacion y mensaje
                ModelState.AddModelError("repe", "nombre de villa ya existe, tontin");
                return BadRequest(ModelState); 
            }
            */

            //nuevo, con db:
            if (await _dbFer.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == villaNueva.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("repe", "nombre de villa ya existe, tontin");
                return BadRequest(ModelState);
            }


            if (villaNueva == null)
            {
                return BadRequest();
            }
            //el id debe ser generado AUTO, por lo que si introducen id dará error 500; si es 0 está ok, ya que más adelante se cambia.
            //por ello cuando se introduzca el post hay que dejar en 0 la ID


            //lo de abajo no hace falta con el VillaDtoCreate.
            /*if (villaNueva.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            */

            //para generar automaticamente el id: villaNueva.Id es la id de la villa que estamos creando.
            //Ahí asignaremos el num máximo de id que exista en el Storage. Para ello ordenamos la lista del Storage de forma
            //descendente por el ID (con la lambda) y cogemos el 1er dato que haya, que será el más grande, y le añadimos 1.
            //Este será el ID de nuestra nueva villa creada con este POST

            //antiguo, sin db
            //villaNueva.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;

            //para añadir 1 nuevo objeto a nuestra lista del storage:

            //antiguo, sin db
            //VillaStore.villaList.Add(villaNueva);

            //para DB: creamos 1 nuevo objeto villa en base a lo que recibe del dto (el id no, es automático). Todo esto es SIN mapeador, lo marcamos. Abajo con mapper.
            /* Villa modelo = new()
            {
                Nombre = villaNueva.Nombre,
                Detalle = villaNueva.Detalle,
                ImagenUrl = villaNueva.ImagenUrl,
                Ocupantes = villaNueva.Ocupantes,
                Tarifa = villaNueva.Tarifa,
                MetrosCuadrados = villaNueva.MetrosCuadrados,
                Amenidad = villaNueva.Amenidad
            };
            */

            //con mapper:
            Villa modelo = _mapeador.Map<Villa>(villaNueva);

            //para que la fecha salga bien:
            modelo.FechaCreacion = DateTime.Now;
            modelo.FechaActualizacion = DateTime.Now;

            //se añade al registro de la bd lo de arriba, esto hará 1 insert.
            await _dbFer.Villas.AddAsync(modelo);
            await _dbFer.SaveChangesAsync(); //<--- una especie de "commit"

            //el CreatedAtRoute es tipo el ok, pero es el 201 en lugar del 200: indica que se ha creado exitosamente el elemento.
            //dentro del paréntesis indicamos 1.- el name el endpoint GetVilla, 2.-la id de la villa en cuestión y 3.- el objeto entero
            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);

        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //se usa Iactionresult en lugar de action... cuando se usan deletes, ya que hay que devolver "noContent...". para put, por ej, tb.
        public async Task <IActionResult> DeleteVilla (int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            //antiguo, sin db
            //var villa = VillaStore.villaList.FirstOrDefault(v=>v.Id == id);

            //nuevo, con db:
            var villa = await _dbFer.Villas.FirstOrDefaultAsync(v => v.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            //si no se cumplen los errores de arriba, se borra la villa y se returnea el mensaje de éxito
            //esto de abajo no se usa con db.
            //VillaStore.villaList.Remove(villa);

            //para hacer el delete propiamente en la db:
            _dbFer.Villas.Remove(villa); // <-- remove siempre es síncrono.
            await _dbFer.SaveChangesAsync ();

            //siempre que hagamos deletes hay que devolver un NO CONENT, o cuando no se devuelva contenido: con el put tb, por ej.
            return NoContent();
        }

        //para actualizar hay PUT, que actualiza TODO el contenido del objeto, y PATCH, que solo actualiza 1 elemento
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //como params, recibe el id de la villa a modificar Y el objeto villa (frombody: es decir, cogerá los elementos de la solicitud
        //http y los convertirá en un objeto tipo VillaDto llamado villaActualizada).
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaActualizada)
        {
             if (villaActualizada ==null || id != villaActualizada.Id)
            {
                return BadRequest();
            }

            //las 4 lineas de abajo es para el store, no sirve con db:
            /*var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            villa.Nombre=villaActualizada.Nombre;
            villa.Ocupantes = villaActualizada.Ocupantes;
            villa.MetrosCuadrados = villaActualizada.MetrosCuadrados;  */

            //con db y sin mapper:
            /*
               Villa modelo = new()
            {
                Id = villaActualizada.Id,
                Nombre = villaActualizada.Nombre,
                Detalle = villaActualizada.Detalle,
                ImagenUrl = villaActualizada.ImagenUrl,
                Ocupantes = villaActualizada.Ocupantes,
                Tarifa = villaActualizada.Tarifa,
                MetrosCuadrados = villaActualizada.MetrosCuadrados,
                Amenidad = villaActualizada.Amenidad
            };
            */

            //con db Y mapper:
            Villa modelo = _mapeador.Map <Villa>(villaActualizada);


            //invocamos el metodo update enviándole el modelo de arriba y commiteamos:
            _dbFer.Villas.Update(modelo); //<--- siempre es síncrono.
            await _dbFer.SaveChangesAsync();

            return NoContent();
        }

        //para hacer el patch hay que bajarse nugets. tools -> nuget package manager  -> manage nuget pckgs
        //2 nugets: microsoft-aspnetcore.jsonpatch  y  microsoft.aspnetcore.mvc.newtonsoftJson
        //una vez instalados los nugets (ojo, version nuget y la del proyecto ha de ser = !!! ), vamos a program.cs
        //y ahi colocamos los nuevos servicios: detras de builder.Services.AddControllers() colocamos:  ..AddNewtonsoftJson();
        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //como params, recibe el id de la villa a modificar Y un jsonPatchDocument).
        public async Task<IActionResult> UpdateParcialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //abajo para store, no vale pa db:
            // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            //para db (el AsNoTracking se usa para operaciones de solo lectura cuando luego se va a volver a usar el mismo registro)
            var villa = await _dbFer.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            //creamos un villaDto y vamos a llenar sus propiedades en base a mi variable "villa" de forma temporal con lo que YA hay:
            //lo de abajo es sin mapper.
            /*
            VillaUpdateDto villadto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };
            */

            //con mapper: necesitamos un VillaUpdateDto, lo sacamos de "villa":
            VillaUpdateDto villadto = _mapeador.Map<VillaUpdateDto>(villa);

            //para usar el patch en el swagger, hay que poner los datos asi:
            //[
            //{
            //  "path": "/nombre",  <----- cosa que queremos cambiar
            //  "op": "replace",  <---- lo que queremos hacer 
            //  "value": "nuevo nombre"  <--- new valor
            //}
            //]

            //este de abajo es el patch para STORE:
            //patchDto.ApplyTo(villa, ModelState);

            //para db, es asi:
            patchDto.ApplyTo(villadto, ModelState);


            //Después de aplicar el parche, verifica si ModelState tiene errores de validación.
            //Si ModelState no es válido (es decir, contiene errores), devuelve una respuesta "BadRequest"
            //(400) con los detalles de los errores.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //creamos 1 modelo de tipo villa llamado "modelo" y llenamos sus propiedades. al haber pasado después del apply del patchdto,
            //esto contendrá lo que hay que modificar: tendrá las propiedades base en TODO menos en el campo modificado:

            //Villa modelo = new()
            //{
            //    Id = villadto.Id,
            //    Nombre = villadto.Nombre,
            //    Detalle = villadto.Detalle,
            //    ImagenUrl = villadto.ImagenUrl,
            //    Ocupantes = villadto.Ocupantes,
            //    Tarifa = villadto.Tarifa,
            //    MetrosCuadrados = villadto.MetrosCuadrados,
            //    Amenidad = villadto.Amenidad
            //};

            Villa modelo = _mapeador.Map<Villa>(villadto);

            //usamos el metodo update de la db para updatearla:
            _dbFer.Villas.Update(modelo);
            await  _dbFer.SaveChangesAsync();
       
            return NoContent();

        }


    }
}
    