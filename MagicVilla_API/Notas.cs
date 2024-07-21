

/*ORM: object relational mapping
  
 para usar la base de datos de sql, vamos a nugets y bajamos los siguientes:

-microsoft.entityFrameworkCore.SqlServer  (incluye el nuget "basico")
-microsoft.entityFrameworkCore.Tools

recuerda indicar la misma version que el proyecto.

tambi�n hay que descargar:

- microsoft sql server (developer ver.)
- microsoft sql server management studio
  
se abre el management studio y en "server name" ponemos localhost; en autentication, "windows auth.", y en encryption, optional & trust cert
  
1.- una vez hecho eso, se crea dentro de "Datos" o similar "ApplicationDbContext". ah� indicaremos qu� modelos usar� nuestra BD
2.- luego vamos a appsettings.json y ponemos las cosas que salen ahi.
3.- se conecta la cadena de conexi�n con nuestro DBcontext. vamos a program.cs y a�adimos lo que esta ahi.
4.- dentro de applicationDbContext, creamos un constructor (ctor +tab) con los datos indicados alli.
5.- creamos la BD. se hace mediante los comandos "add-migration" y "update-database" en la consola del package manager (tools -> nuget package mgr -> activar consola)
para crear la bd, ponemos en esa consola "add-migration CrearBaseDatos" (lo ultimo es el nombre, da igual)
esto habr� creado un nuevo folder en el proyecto llamado "Migrations". dentro hay un script para crear la bd, PERO AUN NO LA HA CREADO. 
6.- para que se ejecute este script, hay que poner update-database.
7.- para a�adir contenido "base" a nuestra BD, vamos a applicationDbContext y ahi a�adimos villas tal como esta mostrado. 
luego se hace un add-migration AlimentarTabla y a continuacion otro update-database para hacer los inserts correspondientes.
8.- vamos al controller e inyectamos lo siguiente:

        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db)
        {
            _loggerFer = logger;
            _dbFer = db;
        }

esto lo que hace es inyectar el servicio de appDbContext en nuestro controlador, conectando �ste (y el CRUD) con la DB.

9.- modificamos correspondientemente los CRUD del controlador para que, en lugar del Store, usen la db. (esta comentado lo antiguo).

  
  
  
  
  
  
  
  
  
 */

