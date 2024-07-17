using MagicVilla_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Datos;

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
        [HttpGet("Id")]
        public ActionResult <VillaDto> GetVilla(int id)
        {
            return  Ok (VillaStore.villaList.FirstOrDefault(villa => villa.Id == id));
        }




    }
}
    