import { Routes } from '@angular/router';
import { ProductosMenu } from './components/productos-menu/productos-menu';
import { Admin } from './pages/admin/admin';
import { Carrito } from './pages/carrito/carrito';
import { Login } from './pages/login/login';
import { Pedidos } from './pages/pedidos/pedidos';
import { Registro } from './pages/registro/registro';

export const routes: Routes = [
  { path: '', component: ProductosMenu },
  { path: 'login', component: Login },
  { path: 'registro', component: Registro },
  { path: 'carrito', component: Carrito },
  { path: 'pedidos', component: Pedidos },
  { path: 'admin', component: Admin },
  { path: '**', redirectTo: '' },
];
