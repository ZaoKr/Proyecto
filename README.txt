CAFETERIA AROMA - BACKEND + FRONTEND

Proyecto de cafeteria full-stack:

- Backend: ASP.NET Core + Entity Framework Core + SQLite
- Frontend: Angular 22 + Tailwind CSS

Estructura de la carpeta:

- backend/ -> API, controladores, modelos y base SQLite
- frontend/ -> aplicacion web en Angular
- .gitignore -> archivos fuera del control de versiones
- README.txt -> este archivo

Los dos proyectos deben correr al mismo tiempo para que la app funcione.
Los comandos se ejecutan desde la raiz de cafeteria-aroma, salvo aviso.


============================================================
1. CAMBIOS REALIZADOS
============================================================

Backend
-------
- Migracion completa de PostgreSQL a SQLite.
- Base de datos ubicada en backend/src/Cafeteria.Api/cafeteria.db.
- Eliminada la dependencia Npgsql y las cadenas de conexion externas.
- Preservada la base existente y el usuario clave: mambo@gmail.com / 123456.
- Autenticacion basada en sesiones con token y PasswordHasher.
- Codigo fuente limpio, sin comentarios de relleno.

Frontend
--------
- Modulos completos de registro, inicio y cierre de sesion.
- Conexion del menu con las APIs de categorias y productos.
- Filtro dinamico por categoria y carrito de compras persistente en localStorage.
- Selector de modalidad (llevar o mesa), numero de mesa, metodo de pago y notas opcionales.
- Historial de pedidos del cliente con opcion de cancelacion para ordenes pendientes.
- Panel de administradores con metricas de ventas, control de estados de pedidos y CRUD total de productos y categorias, incluyendo marcarlos como agotados.
- Rutas actualizadas en Angular y limpieza del codigo fuente.

Pruebas realizadas
------------------
- Backend validado con dotnet build.
- Frontend compilado con npm run build y testeado con npm test -- --watch=false.
- Flujo E2E probado con Playwright: registro, login, carrito, pedidos, cancelacion, panel de administracion y gestion de inventario.

Estado al empaquetar:

- 3 usuarios
- 4 categorias
- 16 productos
- 0 pedidos


============================================================
2. FUNCIONAMIENTO DEL SISTEMA
============================================================

Flujo del cliente
-----------------
1. Entra al menu principal y filtra por categoria si lo desea.
2. Agrega productos al carrito. Se guardan localmente.
3. Va al carrito y define si es para llevar o en mesa, el numero de mesa, el medio de pago y las notas.
4. Si no ha iniciado sesion, el sistema lo manda al login y lo regresa al carrito.
5. Confirma el pedido. El carrito se vacia automaticamente.
6. Revisa el estado en "Mis pedidos" y puede cancelarlo mientras siga pendiente.

Flujo del administrador
-----------------------
1. Entra con credenciales con rol admin.
2. Accede al enlace "Admin" en la barra de navegacion.
3. Visualiza las metricas en tiempo real: pedidos del dia, ventas, pendientes, en preparacion, listos, clientes, productos y top de ventas.
4. Gestiona el inventario: crear, editar, eliminar y agotar productos y categorias.
5. Actualiza el ciclo de vida de los pedidos (Pendiente -> EnPreparacion -> Listo -> Entregado) o los cancela si hace falta.

Autenticacion y sesiones
------------------------
- El backend emite un token al iniciar sesion.
- El navegador lo almacena y lo adjunta en cada peticion protegida mediante Authorization: Bearer TOKEN.
- Las sesiones expiran a los 7 dias o al cerrar sesion explicitamente.

Carrito
-------
- Clave en localStorage: carrito.
- Acumula cantidades y muestra el total en pantalla.
- Al confirmar, el backend vuelve a calcular el total con los precios actuales mediante POST /api/pedidos.


============================================================
3. REQUISITOS TECNICOS
============================================================

- .NET SDK 10
- Node.js 22 o superior
- npm

Verifica las versiones con:

  dotnet --version
  node --version
  npm --version


============================================================
4. EJECUTAR EL BACKEND
============================================================

Abre una terminal:

  cd backend
  dotnet run --project src/Cafeteria.Api

La API responde en http://localhost:5196.

Prueba rapida de salud:

  curl http://localhost:5196/api/estado

Respuesta esperada:

  {"estado":"ok","baseDatos":"SQLite","hora":"..."}

Si la base no existe, la primera ejecucion crea las tablas y los datos por defecto. Como cafeteria.db ya esta incluido, el usuario mambo se mantiene intacto.

Para detenerlo presiona Ctrl + C.


============================================================
5. EJECUTAR EL FRONTEND
============================================================

Abre otra terminal:

  cd frontend
  npm install
  npm start

Abre en tu navegador http://localhost:4200. Si el puerto esta ocupado, usa:

  npm start -- --port 4201

La direccion de la API se configura en frontend/src/app/api.config.ts:

  export const API_URL = 'http://localhost:5196/api';

Para detenerlo presiona Ctrl + C.


============================================================
6. USUARIOS DE PRUEBA
============================================================

- ADMINISTRADOR: admin@cafeteria.com / Admin123!
- CLIENTE BASE: cliente@cafeteria.com / Cliente123!
- CLIENTE MAMBO: mambo@gmail.com / 123456

Cualquier registro nuevo desde la interfaz entra automaticamente con rol de cliente.


============================================================
7. CHECKLIST DE VALIDACION RAPIDA
============================================================

Backend
-------
[ ] El servidor arranca sin errores en el puerto 5196.
[ ] Los endpoints devuelven datos: /api/estado, /api/categorias (4), /api/productos (16).
[ ] El login funciona para mambo y para admin.
[ ] dotnet build compila sin errores.

Frontend
--------
[ ] La interfaz carga en el puerto 4200 con los 16 productos listados.
[ ] Los filtros, el carrito, la persistencia, el login, los pedidos y el panel admin funcionan al 100%.
[ ] npm run build y npm test -- --watch=false pasan sin fallos.

Pruebas express con curl
------------------------
Estado:

  curl http://localhost:5196/api/estado

Productos:

  curl http://localhost:5196/api/productos

Login de mambo:

  curl -X POST http://localhost:5196/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"mambo@gmail.com","password":"123456"}'


============================================================
8. RUTAS DISPONIBLES EN ANGULAR
============================================================

- / -> Menu de productos
- /login -> Iniciar sesion
- /registro -> Crear cuenta nueva
- /carrito -> Carrito y confirmacion de pedidos
- /pedidos -> Historial personal, o general si eres admin
- /admin -> Panel de control, inventario y categorias


============================================================
9. BASE DE DATOS SQLITE
============================================================

Ubicacion:

  backend/src/Cafeteria.Api/cafeteria.db

- No borres este archivo si quieres conservar los registros y el usuario mambo.
- Haz respaldos rapidos con:

  cp backend/src/Cafeteria.Api/cafeteria.db cafeteria.db.bak

- Inspecciona la base con la CLI de SQLite:

  sqlite3 backend/src/Cafeteria.Api/cafeteria.db


============================================================
10. COMO SUBIR CAMBIOS A GITHUB
============================================================

El repositorio del proyecto es:

  https://github.com/ZaoKr/Proyecto

Si ya clonaste el repositorio, solo agrega los cambios, haz commit y sube:

  git add .
  git commit -m "Actualizo el proyecto"
  git push origin main

Si estas creando la carpeta desde cero:

  cd cafeteria-aroma
  git init
  git add .
  git commit -m "Estructura base Cafeteria Aroma con Angular y SQLite"
  git branch -M main
  git remote add origin https://github.com/ZaoKr/Proyecto.git
  git push -u origin main

Si el repositorio remoto ya tiene commits y el local no comparte historial, sincroniza primero con:

  git pull origin main --allow-unrelated-histories --no-rebase


============================================================
11. PROBLEMAS COMUNES
============================================================

Address already in use:
El puerto esta ocupado. Liberalo con:

  lsof -ti:5196 | xargs -r kill
  lsof -ti:4200 | xargs -r kill

Error de conexion en el menu:
El backend esta apagado. Enciendelo y vuelve a intentar.

No sale el boton Admin:
Asegurate de iniciar sesion con la cuenta de administrador correcta.

============================================================
FIN DEL README
============================================================
