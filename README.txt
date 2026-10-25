============================================================
 CAFETERIA AROMA - BACKEND + FRONTEND
============================================================

Proyecto de cafeteria con:

- Backend: ASP.NET Core + Entity Framework Core + SQLite
- Frontend: Angular 22 + Tailwind CSS

Estructura de esta carpeta:

  cafeteria-aroma/
  ├── backend/     -> API, controladores, modelos y base SQLite
  ├── frontend/    -> aplicacion web en Angular
  ├── .gitignore   -> archivos que no se suben a GitHub
  └── README.txt   -> este archivo

Los dos proyectos deben estar encendidos al mismo tiempo para que
la pagina funcione correctamente.

Los comandos de este README se ejecutan desde la raiz de cafeteria-aroma,
salvo que se indique otra cosa.


============================================================
 1. CAMBIOS REALIZADOS
============================================================

BACKEND
-------
- Se cambio la base de datos de PostgreSQL a SQLite.
- La base se guarda en:
  backend/src/Cafeteria.Api/cafeteria.db
- Se elimino la dependencia Npgsql y la cadena de conexion de PostgreSQL.
- Se conservo la base existente y el usuario:
  mambo@gmail.com / 123456
- La API sigue usando sesiones con token y PasswordHasher.
- Se quitaron los comentarios del codigo fuente.
- Se actualizo la documentacion.

FRONTEND
--------
- Se creo el registro de usuarios.
- Se creo el inicio y cierre de sesion.
- Se conecto el menu con las APIs de categorias y productos.
- Se agrego el filtro por categoria.
- Se agrego el carrito de compras.
- El carrito se guarda en localStorage.
- Se agrego la confirmacion de pedidos para llevar o en mesa.
- Se agrego el historial de pedidos del cliente.
- Se agrego la cancelacion de pedidos pendientes.
- Se agrego el panel de administracion.
- El administrador puede ver el resumen de ventas.
- El administrador puede cambiar el estado de los pedidos.
- El administrador puede crear, editar, agotar y eliminar productos.
- El administrador puede crear, editar y eliminar categorias.
- Se actualizaron las rutas de Angular.
- Se quitaron los comentarios del codigo fuente.
- Se actualizo la documentacion.

PRUEBAS REALIZADAS
------------------
- Backend compilado con dotnet build.
- Frontend compilado con npm run build.
- Pruebas de Angular con npm test -- --watch=false.
- Flujo completo probado con Playwright:
  registro, login, carrito, pedido, cancelacion, administracion,
  cambio de estados y CRUD de productos y categorias.

Al momento de dejar el proyecto, la base tenia:
- 3 usuarios
- 4 categorias
- 16 productos
- 0 pedidos


============================================================
 2. COMO FUNCIONA
============================================================

FLUJO DEL CLIENTE
-----------------
1. El cliente entra al menu.
2. Puede filtrar los productos por categoria.
3. Agrega productos al carrito.
4. El carrito se guarda en localStorage.
5. Entra al carrito y elige:
   - Para llevar o en mesa.
   - Numero de mesa si es en mesa.
   - Efectivo o tarjeta.
   - Notas opcionales.
6. Si no ha iniciado sesion, se redirige al login.
7. Despues del login vuelve al carrito.
8. Confirma el pedido.
9. El pedido aparece en "Mis pedidos".
10. Si el pedido sigue pendiente, el cliente puede cancelarlo.

FLUJO DEL ADMINISTRADOR
-----------------------
1. Inicia sesion con una cuenta de rol admin.
2. En la barra superior aparece el enlace "Admin".
3. En el panel puede ver:
   - Pedidos del dia.
   - Ventas del dia.
   - Pedidos pendientes.
   - Pedidos en preparacion.
   - Pedidos listos.
   - Cantidad de clientes.
   - Cantidad de productos.
   - Productos mas vendidos.
4. Puede administrar productos y categorias.
5. En "Ver pedidos" puede cambiar el estado:
   Pendiente -> EnPreparacion -> Listo -> Entregado
   Tambien puede cancelar un pedido.

AUTENTICACION
-------------
- El backend crea un token cuando el usuario inicia sesion.
- El frontend guarda el token en localStorage.
- Las peticiones protegidas envian:
  Authorization: Bearer TOKEN
- Las sesiones duran 7 dias.
- Al cerrar sesion se elimina la sesion del backend y el token del navegador.

CARRITO
-------
- El carrito se guarda con la clave "carrito" en localStorage.
- Al agregar un producto se acumula la cantidad.
- Se pueden cambiar cantidades o quitar productos.
- Al confirmar el pedido se envia POST /api/pedidos.
- El backend calcula el total.
- Despues de crear el pedido, el carrito se vacia.


============================================================
 3. REQUISITOS
============================================================

- .NET SDK 10
- Node.js 22 o superior
- npm

Comprobar versiones:

  dotnet --version
  node --version
  npm --version


============================================================
 4. COMO EJECUTAR EL BACKEND
============================================================

Abrir una terminal:

  cd backend
  dotnet run --project src/Cafeteria.Api

La API queda disponible en:

  http://localhost:5196

Comprobar que funciona:

  curl http://localhost:5196/api/estado

Respuesta esperada:

  {"estado":"ok","baseDatos":"SQLite","hora":"..."}

La primera vez se crean las tablas y los datos de ejemplo.
La carpeta ya incluye cafeteria.db, asi que se conserva el usuario mambo.

Para detener el backend:

  Ctrl + C


============================================================
 5. COMO EJECUTAR EL FRONTEND
============================================================

Abrir otra terminal:

  cd frontend
  npm install
  npm start

Abrir en el navegador:

  http://localhost:4200

Si el puerto 4200 esta ocupado:

  npm start -- --port 4201

La direccion de la API esta en:

  frontend/src/app/api.config.ts

  export const API_URL = 'http://localhost:5196/api';

Para detener el frontend:

  Ctrl + C


============================================================
 6. USUARIOS DE PRUEBA
============================================================

ADMINISTRADOR
  Correo:     admin@cafeteria.com
  Contrasena: Admin123!

CLIENTE
  Correo:     cliente@cafeteria.com
  Contrasena: Cliente123!

CLIENTE MAMBO
  Correo:     mambo@gmail.com
  Contrasena: 123456

Las cuentas nuevas que se registran desde el frontend tienen rol cliente.


============================================================
 7. COMO CHECAR LOS DOS PROYECTOS
============================================================

CHECKLIST DEL BACKEND
---------------------
[ ] El backend inicia sin errores.
[ ] http://localhost:5196/api/estado responde "ok".
[ ] http://localhost:5196/api/categorias devuelve 4 categorias.
[ ] http://localhost:5196/api/productos devuelve 16 productos.
[ ] El login de mambo@gmail.com / 123456 devuelve un token.
[ ] El login de admin@cafeteria.com / Admin123! devuelve un token.
[ ] dotnet build termina sin errores.

CHECKLIST DEL FRONTEND
----------------------
[ ] http://localhost:4200 abre correctamente.
[ ] El menu muestra los 16 productos.
[ ] El filtro por categoria funciona.
[ ] Se puede agregar un producto al carrito.
[ ] El contador del carrito aumenta.
[ ] El carrito conserva los productos al recargar la pagina.
[ ] Se puede iniciar sesion con mambo@gmail.com / 123456.
[ ] Se puede confirmar un pedido.
[ ] El pedido aparece en "Mis pedidos".
[ ] Se puede cancelar un pedido pendiente.
[ ] Se puede cerrar sesion.
[ ] Se puede iniciar sesion como administrador.
[ ] El enlace "Admin" aparece con el usuario administrador.
[ ] El resumen de administracion muestra datos.
[ ] Se puede cambiar el estado de un pedido.
[ ] Se puede crear una categoria.
[ ] Se puede editar una categoria.
[ ] Se puede crear un producto.
[ ] Se puede editar un producto.
[ ] Se puede marcar un producto como agotado.
[ ] Se puede activar otra vez.
[ ] Se puede eliminar un producto.
[ ] Se puede eliminar una categoria.
[ ] npm run build termina sin errores.
[ ] npm test -- --watch=false pasa las pruebas.

PRUEBA RAPIDA DEL BACKEND CON CURL
----------------------------------
Estado:

  curl http://localhost:5196/api/estado

Categorias:

  curl http://localhost:5196/api/categorias

Productos:

  curl http://localhost:5196/api/productos

Login:

  curl -X POST http://localhost:5196/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"mambo@gmail.com","password":"123456"}'

Resumen de administracion:

  TOKEN=$(curl -s -X POST http://localhost:5196/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"admin@cafeteria.com","password":"Admin123!"}' \
    | python3 -c 'import sys,json; print(json.load(sys.stdin)["token"])')

  curl http://localhost:5196/api/admin/resumen \
    -H "Authorization: Bearer $TOKEN"


============================================================
 8. RUTAS DEL FRONTEND
============================================================

  /            Menu de productos
  /login       Inicio de sesion
  /registro    Crear cuenta
  /carrito     Carrito y confirmacion del pedido
  /pedidos     Historial del cliente o todos los pedidos si es admin
  /admin       Resumen, productos y categorias


============================================================
 9. BASE DE DATOS SQLITE
============================================================

Archivo:

  backend/src/Cafeteria.Api/cafeteria.db

IMPORTANTE:
- Esta base ya viene incluida en la carpeta.
- No borrar cafeteria.db si quieres conservar el usuario mambo y los datos.
- Si se borra, la API crea otra base con los datos de ejemplo, pero se pierde
  el usuario mambo@gmail.com.
- Para hacer un respaldo:

  cp backend/src/Cafeteria.Api/cafeteria.db cafeteria.db.bak

Ver los datos con sqlite3:

  sqlite3 backend/src/Cafeteria.Api/cafeteria.db

Dentro de SQLite:

  .tables
  SELECT * FROM Usuarios;
  SELECT * FROM Categorias;
  SELECT * FROM Productos;
  SELECT * FROM Pedidos;
  .quit


============================================================
 10. COMANDOS DE COMPILACION Y PRUEBAS
============================================================

BACKEND:

  cd backend
  dotnet build

FRONTEND:

  cd frontend
  npm run build
  npm test -- --watch=false


============================================================
 11. COMO SUBIR ESTA CARPETA A GITHUB
============================================================

Importante:
- Sube solamente la carpeta cafeteria-aroma.
- No subas la carpeta /mnt/Terciario completa, porque tiene juegos,
  videos y otros archivos que no pertenecen al proyecto.

Pasos:

1. Entrar a la carpeta:

  cd cafeteria-aroma

2. Iniciar git:

  git init

3. Agregar los archivos:

  git add .

4. Crear el primer commit:

  git commit -m "Proyecto Cafeteria Aroma con Angular y SQLite"

5. Cambiar a la rama main:

  git branch -M main

6. Crear el repositorio en GitHub y copiar su direccion.

7. Conectar el repositorio:

  git remote add origin https://github.com/TU_USUARIO/TU_REPOSITORIO.git

8. Subir:

  git push -u origin main

Si el repositorio ya existe, no ejecutes git init otra vez. Solo agrega,
haz commit y push.

El archivo .gitignore ya excluye:

  node_modules/
  dist/
  .angular/
  bin/
  obj/
  coverage/
  .vscode/

La base cafeteria.db si se sube al repositorio para conservar el usuario mambo.


============================================================
 12. PROBLEMAS COMUNES
============================================================

ERROR: address already in use
------------------------------
Significa que el servidor ya esta encendido en ese puerto.

Comprobar puertos:

  ss -ltnp | grep -E ':5196|:4200'

Detener los procesos:

  lsof -ti:5196 | xargs -r kill
  lsof -ti:4200 | xargs -r kill

Si no tienes lsof:

  fuser -k 5196/tcp
  fuser -k 4200/tcp

Luego vuelve a iniciar el backend y el frontend.

EL MENU DICE "No se pudo conectar con la API"
---------------------------------------------
El backend esta apagado o no responde. Comprobar:

  curl http://localhost:5196/api/estado

Si no responde, iniciar el backend otra vez.

NO APARECE EL ENLACE ADMIN
--------------------------
El usuario debe tener rol admin. Usar:

  admin@cafeteria.com / Admin123!

ERROR DE CORS
-------------
La API permite peticiones desde localhost y 127.0.0.1 en cualquier puerto.
Para produccion se debe limitar CORS al dominio real del frontend.


============================================================
 13. NOTAS
============================================================

- El proyecto usa componentes standalone de Angular.
- Los estilos usan Tailwind CSS 4.
- El backend no usa JWT; usa sesiones con token.
- El esquema se crea con EnsureCreated(), no con migraciones.
- Si se cambian las clases de Models, hay que borrar cafeteria.db para
  que se creen las tablas nuevas.
- El proyecto esta pensado para aprender y para un trabajo de practica.

============================================================
 FIN DEL README
============================================================
