CAFETERIA AROMA

Este proyecto es una aplicacion web para una cafeteria. Permite ver el menu, armar un carrito, confirmar pedidos y administrar productos, categorias y pedidos.

La aplicacion tiene dos programas separados:

- Backend: recibe las peticiones, valida los datos y guarda todo en SQLite.
- Frontend: muestra la interfaz en el navegador y llama al backend por HTTP.

Los dos tienen que estar encendidos para usar la aplicacion completa. El backend puede funcionar sin el frontend, y el frontend puede abrirse sin el backend, pero en ese caso no puede cargar ni guardar datos.


QUE HACE EL PROYECTO

Como cliente:

- Ver el menu y filtrar por categoria.
- Agregar productos al carrito.
- Cambiar cantidades o quitar productos.
- Elegir si el pedido es para llevar o en mesa.
- Indicar el numero de mesa cuando corresponde.
- Elegir efectivo o tarjeta.
- Escribir notas para el pedido.
- Confirmar el pedido.
- Ver el historial de pedidos.
- Cancelar un pedido mientras siga pendiente.

Como administrador:

- Ver pedidos del dia, ventas, pedidos pendientes, pedidos en preparacion y pedidos listos.
- Ver la cantidad de clientes y de productos.
- Ver los productos mas vendidos del dia.
- Ver todos los pedidos de todos los clientes.
- Cambiar el estado de un pedido.
- Crear, editar y eliminar productos.
- Marcar un producto como agotado o volver a activarlo.
- Crear, editar y eliminar categorias.

El proyecto no procesa pagos reales. El metodo de pago solo se guarda como dato. Tampoco hace delivery ni reservas de mesas. Solo maneja pedidos para llevar o para consumir en una mesa.


ESTRUCTURA DEL PROYECTO

backend/
  Cafeteria.slnx
  src/
    Cafeteria.Api/
      Program.cs
      Cafeteria.Api.csproj
      appsettings.json
      appsettings.Development.json
      Properties/
        launchSettings.json
      Controllers/
        ControladorBase.cs
        AuthController.cs
        CategoriasController.cs
        ProductosController.cs
        PedidosController.cs
        AdminController.cs
      Data/
        CafeteriaContext.cs
        DatosIniciales.cs
      Models/
        Usuario.cs
        Sesion.cs
        Categoria.cs
        Producto.cs
        Pedido.cs
        PedidoItem.cs
      cafeteria.db

frontend/
  angular.json
  package.json
  package-lock.json
  src/
    main.ts
    index.html
    styles.css
    app/
      app.config.ts
      app.routes.ts
      app.ts
      app.html
      api.config.ts
      components/
        producto-card/
        productos-menu/
      models/
        usuario.ts
        producto.ts
        pedido.ts
      pages/
        login/
        registro/
        carrito/
        pedidos/
        admin/
      services/
        auth.service.ts
        catalogo.service.ts
        carrito.service.ts
        pedido.service.ts
        admin.service.ts

.gitignore
README.txt


COMO FUNCIONA EL BACKEND

El backend esta hecho con ASP.NET Core y .NET 10. Usa Entity Framework Core para hablar con SQLite y PasswordHasher para guardar las contrasenas de forma segura.

El arranque esta en Program.cs. Ese archivo hace lo siguiente:

- Registra los controladores con AddControllers.
- Calcula la ruta del archivo cafeteria.db dentro de la carpeta del proyecto.
- Configura Entity Framework Core con UseSqlite.
- Registra PasswordHasher para las contrasenas.
- Configura CORS para permitir peticiones desde localhost.
- Ejecuta EnsureCreated para crear las tablas si la base no existe.
- Ejecuta DatosIniciales.Cargar para crear datos de ejemplo la primera vez.
- Activa CORS y mapea los controladores.
- Deja el servidor escuchando en el puerto 5196.

La base de datos se llama cafeteria.db y esta dentro de backend/src/Cafeteria.Api. SQLite guarda todo en ese archivo, asi que no hace falta instalar un servidor de base de datos aparte.

Las tablas se crean a partir de las clases de la carpeta Models. CafeteriaContext define las tablas y las relaciones. Por ejemplo:

- Un usuario puede tener varias sesiones.
- Una categoria puede tener varios productos.
- Un pedido pertenece a un usuario.
- Un pedido tiene varios items.
- Cada item apunta a un producto.

Tambien hay reglas para no romper el historial. No se puede borrar una categoria que todavia tiene productos. No se puede borrar un producto que aparece en un pedido. No se puede borrar un usuario que tiene pedidos.

DatosIniciales solo carga datos si la tabla de usuarios esta vacia. Si ya hay usuarios, no toca nada. Eso evita duplicar productos o cuentas cada vez que arranca el backend.

Los controladores reciben las peticiones HTTP y devuelven JSON. Estas son las partes principales:

- AuthController: registro, login, logout y datos del usuario actual.
- CategoriasController: lista, crea, edita y elimina categorias.
- ProductosController: lista, busca, crea, edita, cambia la disponibilidad y elimina productos.
- PedidosController: crea pedidos, lista pedidos, muestra un pedido y cambia su estado.
- AdminController: devuelve el resumen de ventas y pedidos para el panel de administracion.
- ControladorBase: tiene funciones comunes para leer el token y saber que usuario esta haciendo la peticion.

El controlador base lee la cabecera Authorization. Espera un valor con la forma Bearer TOKEN. Despues busca ese token en la tabla de sesiones y comprueba que no haya vencido. Si la sesion existe, devuelve el usuario. Si no existe, responde con error 401. Si el usuario no es administrador, los endpoints de administracion responden con error 403.

El login funciona asi:

- El usuario envia correo y contrasena.
- El backend busca el usuario por correo.
- PasswordHasher verifica la contrasena contra el hash guardado.
- Si es correcta, se crea una sesion con un token aleatorio.
- La sesion vence a los 7 dias.
- El backend devuelve el token y los datos publicos del usuario.
- El frontend guarda el token y lo envia en las siguientes peticiones protegidas.

Las contrasenas no se guardan en texto plano. Se guarda un hash. El campo PasswordHash tiene JsonIgnore para que nunca salga en una respuesta de la API.

Los pedidos se crean en PedidosController. El backend no confia en el total que muestra el frontend. Recibe los productos y las cantidades, busca el precio actual de cada producto en la base y calcula el total otra vez. Tambien revisa que el producto exista, que este disponible y que la cantidad este entre 1 y 20.

Los estados de un pedido son:

Pendiente
EnPreparacion
Listo
Entregado
Cancelado

El cliente solo puede cancelar un pedido que este Pendiente. El administrador puede avanzar al siguiente estado o cancelar mientras el pedido no este entregado ni cancelado. No se pueden saltar estados.

El resumen del administrador se calcula con consultas a la base. Cuenta los pedidos del dia, suma las ventas sin contar los cancelados, cuenta los pedidos pendientes, en preparacion y listos, cuenta los clientes y los productos, y arma una lista con los productos mas vendidos del dia.


COMO FUNCIONA EL FRONTEND

El frontend esta hecho con Angular 22, TypeScript y Tailwind CSS. Usa servicios para llamar a la API, signals para guardar el estado de la pantalla y localStorage para conservar el carrito y la sesion.

El arranque esta en main.ts. Ese archivo inicia el componente principal App con la configuracion de app.config.ts. Esa configuracion registra el HttpClient, el router y el manejo de errores del navegador.

Las rutas estan en app.routes.ts:

- / muestra el menu.
- /login muestra el inicio de sesion.
- /registro muestra el formulario de registro.
- /carrito muestra el carrito y la confirmacion del pedido.
- /pedidos muestra el historial del cliente o todos los pedidos si el usuario es administrador.
- /admin muestra el panel de administracion.
- Cualquier otra ruta vuelve al menu.

La barra de navegacion esta en app.html. Muestra el menu, el carrito y el nombre del usuario. Si hay una sesion iniciada, muestra Mis pedidos y el boton Salir. Si el usuario es administrador, tambien muestra el enlace Admin. Si no hay sesion, muestra Iniciar sesion y Registrarse.

La direccion de la API esta en api.config.ts. Cuando la pagina se abre con el servidor de Angular en el puerto 4200 o 4201, usa http://localhost:5196/api. En la version publicada usa /api, porque el mismo backend sirve la pagina y la API.

Los servicios usan esa direccion para armar las peticiones. Por ejemplo, AuthService llama a /auth/login, CatalogoService llama a /productos y PedidoService llama a /pedidos.

Los servicios principales son:

- AuthService: maneja el login, el registro, el cierre de sesion y el token.
- CatalogoService: obtiene categorias y productos.
- CarritoService: guarda los productos del carrito, las cantidades y el total.
- PedidoService: crea pedidos, lista pedidos y cambia el estado de un pedido.
- AdminService: obtiene el resumen y hace las operaciones de productos y categorias.

AuthService guarda el token y el usuario en localStorage. Tambien tiene dos valores calculados: logueado y esAdmin. logueado indica si hay un usuario guardado. esAdmin indica si el rol es admin.

CarritoService guarda los productos en un signal. La cantidad y el total se calculan con computed. Cuando se agrega un producto que ya existe, se suma la cantidad. El maximo por producto es 20. El carrito se guarda en localStorage con la clave carrito.

Las pantallas principales son:

- ProductosMenu: carga categorias y productos al mismo tiempo, muestra las tarjetas y permite filtrar por categoria.
- ProductoCard: muestra un producto y emite un evento cuando se pulsa Agregar.
- Login: valida los campos y guarda la sesion.
- Registro: valida los campos, confirma la contrasena y crea la cuenta.
- Carrito: muestra los productos, permite cambiar cantidades y confirma el pedido.
- Pedidos: muestra el historial, los estados y los botones para cancelar o cambiar el estado.
- Admin: muestra el resumen, el formulario de productos y el formulario de categorias.

El carrito funciona sin sesion, pero para confirmar el pedido el usuario tiene que iniciar sesion. Si no lo ha hecho, la pagina lo manda al login y despues lo devuelve al carrito. El carrito no se pierde porque esta guardado en localStorage.

Cuando el usuario confirma el pedido, el frontend envia los productos y las cantidades al backend. Si la respuesta es correcta, vacia el carrito y va a la pagina de pedidos. Si hay un error, muestra el mensaje que devuelve la API y deja el carrito como estaba.

El panel de administracion solo carga datos si el usuario tiene rol admin. Si una persona entra a la ruta /admin sin ese rol, la pagina muestra un aviso. De todas formas, la seguridad real esta en el backend, porque cada endpoint de administracion revisa el token y el rol antes de hacer cambios.

Las paginas no usan guards de ruta. Cada componente revisa si hay sesion o si el usuario es administrador. Si alguien modifica localStorage para ponerse rol admin, la pagina podria mostrar el enlace, pero el backend seguiria rechazando las operaciones porque el rol real esta guardado en la base.


COMO SE COMUNICAN EL BACKEND Y EL FRONTEND

El frontend no se conecta directamente a SQLite. Solo hace peticiones HTTP al backend. El backend es el unico que abre la base de datos.

El recorrido de una peticion es asi:

1. El usuario hace algo en la pagina.
2. Un componente de Angular llama a un servicio.
3. El servicio usa HttpClient para enviar una peticion HTTP.
4. La peticion llega al backend en el puerto 5196.
5. ASP.NET Core la dirige al controlador correspondiente.
6. El controlador valida los datos y usa Entity Framework Core.
7. Entity Framework Core consulta o modifica SQLite.
8. El controlador devuelve una respuesta JSON.
9. Angular recibe la respuesta.
10. El componente actualiza sus signals y la pantalla cambia.

Un ejemplo de login:

POST http://localhost:5196/api/auth/login
Content-Type: application/json

{
  "email": "admin@cafeteria.com",
  "password": "Admin123!"
}

Si todo esta bien, el backend responde con un token y los datos del usuario. El frontend guarda el token y lo envia en las peticiones protegidas asi:

Authorization: Bearer TOKEN

Un ejemplo de pedido:

POST http://localhost:5196/api/pedidos
Authorization: Bearer TOKEN
Content-Type: application/json

{
  "tipo": "Mesa",
  "numeroMesa": 4,
  "metodoPago": "Efectivo",
  "notas": "Sin azucar",
  "items": [
    {
      "productoId": 1,
      "cantidad": 2,
      "notas": ""
    }
  ]
}

El backend revisa el token, valida los datos, calcula el total y guarda el pedido. Despues devuelve el pedido creado.

El navegador aplica una regla de seguridad llamada CORS. En local, el frontend esta en el puerto 4200 y el backend en el 5196, asi que son dos origenes distintos. El backend tiene una politica CORS que permite peticiones desde localhost y desde 127.0.0.1. En la version publicada con Docker, el backend sirve el frontend compilado y la API desde el mismo origen, por eso no hay problema de CORS.


COMO SE EJECUTA

Hace falta tener instalado:

- .NET SDK 10
- Node.js 22 o superior
- npm

Para comprobar las versiones:

dotnet --version
node --version
npm --version

Para arrancar el backend, abre una terminal desde la raiz del proyecto:

cd backend
dotnet run --project src/Cafeteria.Api

El backend queda disponible en http://localhost:5196.

Para comprobar que esta funcionando:

curl http://localhost:5196/api/estado

La respuesta esperada es un JSON con estado ok, baseDatos SQLite y la hora.

Para arrancar el frontend, abre otra terminal desde la raiz del proyecto:

cd frontend
npm install
npm start

El frontend queda disponible en http://localhost:4200. Si ese puerto esta ocupado, se puede usar otro:

npm start -- --port 4201

Para detener cualquiera de los dos, presiona Ctrl + C en su terminal.


PUBLICARLO EN INTERNET

El proyecto incluye un Dockerfile que compila el frontend y el backend juntos. El backend sirve la pagina y la API desde la misma direccion, asi que no hace falta configurar CORS ni publicar dos sitios separados.

En Railway:

1. Crear una cuenta con GitHub.
2. Crear un proyecto nuevo y elegir Deploy from GitHub repo.
3. Seleccionar el repositorio del proyecto.
4. Agregar un volumen y montarlo en /data.
5. Agregar la variable DB_PATH con el valor /data/cafeteria.db.
6. Esperar el deploy.
7. Generar un dominio publico desde la opcion de networking o domains.

La primera vez, si el volumen esta vacio, la aplicacion copia cafeteria.db al volumen. Despues todos los cambios quedan guardados ahi. Si el servicio se reinicia o se vuelve a desplegar, la base no se pierde.

La aplicacion lee la variable PORT que asigna el hosting y escucha en ese puerto. No hay que cambiar nada en el codigo.

Si se usa otro hosting que acepte Docker, se puede usar el mismo Dockerfile y montar un volumen en /data. Sin volumen, SQLite se borra cada vez que el servicio se reinicia.


USUARIOS DE PRUEBA

- Administrador: admin@cafeteria.com / Admin123!
- Cliente: cliente@cafeteria.com / Cliente123!

La base tambien conserva los usuarios que ya existian antes de empaquetar el proyecto. No borres cafeteria.db si quieres mantenerlos.

Los usuarios nuevos que se registran desde la pagina entran con rol cliente. El rol administrador no se puede elegir desde el formulario.


COMO PROBAR EL PROYECTO

Backend:

- Abrir http://localhost:5196/api/estado y comprobar que responde.
- Abrir http://localhost:5196/api/categorias y comprobar que devuelve las categorias.
- Abrir http://localhost:5196/api/productos y comprobar que devuelve los productos.
- Probar el login con el usuario administrador y con el cliente.
- Ejecutar dotnet build y comprobar que no hay errores.

Frontend:

- Abrir http://localhost:4200.
- Comprobar que se ven los productos.
- Probar el filtro por categoria.
- Agregar productos al carrito.
- Recargar la pagina y comprobar que el carrito sigue ahi.
- Iniciar sesion y confirmar un pedido.
- Revisar el pedido en Mis pedidos.
- Cancelar un pedido pendiente.
- Entrar con el usuario administrador.
- Revisar el resumen, los productos y las categorias.
- Cambiar el estado de un pedido.
- Crear, editar y eliminar un producto o una categoria de prueba.
- Ejecutar npm run build y npm test -- --watch=false.

Tambien se probo el flujo completo con Playwright: registro, login, carrito, pedido, cancelacion, panel de administracion, cambio de estados y gestion de productos y categorias.


BASE DE DATOS

La base esta en:

backend/src/Cafeteria.Api/cafeteria.db

Se incluye en el proyecto para conservar los usuarios y los productos que ya estaban cargados. Si se borra, el backend crea otra base con los datos iniciales, pero se pierden las cuentas y los cambios que se hayan hecho.

Para hacer un respaldo:

cp backend/src/Cafeteria.Api/cafeteria.db cafeteria.db.bak

Para abrir la base con la herramienta de SQLite:

sqlite3 backend/src/Cafeteria.Api/cafeteria.db

Dentro de SQLite se pueden ver las tablas con:

.tables

Y se pueden hacer consultas como:

SELECT * FROM Usuarios;
SELECT * FROM Productos;
SELECT * FROM Pedidos;

Para salir:

.quit

Si se cambian las clases de Models, hay que borrar cafeteria.db para que se creen las tablas nuevas. EnsureCreated no actualiza una base que ya existe.


ARCHIVOS QUE NO SE SUBEN

El archivo .gitignore deja fuera las carpetas y archivos que se generan al compilar o instalar dependencias:

- node_modules
- bin
- obj
- dist
- .angular
- coverage
- .vscode

La base cafeteria.db si se incluye. No se deben subir tokens, contrasenas ni archivos personales.


PROBLEMAS COMUNES

Address already in use:
El puerto ya esta ocupado por otro proceso. Se puede liberar con:

lsof -ti:5196 | xargs -r kill
lsof -ti:4200 | xargs -r kill

Error de conexion en el menu:
El backend esta apagado o la direccion de la API no es la correcta. Revisar que el backend este corriendo en el puerto 5196 y que api.config.ts apunte a esa direccion.

No aparece el boton Admin:
El usuario con el que se inicio sesion no tiene rol administrador. Probar con la cuenta de administrador.

No se puede eliminar un producto:
El producto aparece en algun pedido. En ese caso conviene marcarlo como agotado en lugar de borrarlo.

No se puede eliminar una categoria:
La categoria todavia tiene productos. Primero hay que mover o eliminar esos productos.

El token no funciona:
La sesion puede haber vencido. Cerrar sesion y volver a iniciar.

El navegador muestra un error de CORS:
El frontend se esta abriendo desde una direccion que el backend no permite. En local, usar localhost o 127.0.0.1.


COSAS A TENER EN CUENTA

- En local, el backend y el frontend son dos programas distintos y se levantan en terminales separadas. En la version publicada con Docker, el backend sirve el frontend compilado.
- El frontend necesita al backend para cargar datos, iniciar sesion y confirmar pedidos.
- El backend no necesita al frontend para funcionar. Se puede probar con curl o con Postman.
- El carrito se guarda en el navegador, no en la base, hasta que se confirma el pedido.
- El total del pedido se calcula en el backend, no en el frontend.
- Las contrasenas se guardan con hash, no en texto plano.
- Las sesiones se guardan en la base y vencen a los 7 dias.
- El proyecto esta pensado para uso local y para practica. La version con Docker se puede publicar con HTTPS, pero para un uso real conviene revisar el manejo de tokens, cambiar las contrasenas de prueba y hacer respaldos de la base.

FIN
