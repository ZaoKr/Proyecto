import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { Resumen } from '../models/pedido';
import { Categoria, Producto, ProductoRequest } from '../models/producto';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  resumen(): Observable<Resumen> {
    return this.http.get<Resumen>(`${API_URL}/admin/resumen`, {
      headers: this.auth.headers(),
    });
  }

  crearProducto(producto: ProductoRequest): Observable<Producto> {
    return this.http.post<Producto>(`${API_URL}/productos`, producto, {
      headers: this.auth.headers(),
    });
  }

  editarProducto(id: number, producto: ProductoRequest): Observable<Producto> {
    return this.http.put<Producto>(`${API_URL}/productos/${id}`, producto, {
      headers: this.auth.headers(),
    });
  }

  eliminarProducto(id: number): Observable<unknown> {
    return this.http.delete(`${API_URL}/productos/${id}`, {
      headers: this.auth.headers(),
    });
  }

  cambiarDisponibilidad(id: number, disponible: boolean): Observable<Producto> {
    return this.http.patch<Producto>(
      `${API_URL}/productos/${id}/disponibilidad`,
      { disponible },
      { headers: this.auth.headers() },
    );
  }

  crearCategoria(nombre: string): Observable<Categoria> {
    return this.http.post<Categoria>(
      `${API_URL}/categorias`,
      { nombre },
      { headers: this.auth.headers() },
    );
  }

  editarCategoria(id: number, nombre: string): Observable<Categoria> {
    return this.http.put<Categoria>(
      `${API_URL}/categorias/${id}`,
      { nombre },
      { headers: this.auth.headers() },
    );
  }

  eliminarCategoria(id: number): Observable<unknown> {
    return this.http.delete(`${API_URL}/categorias/${id}`, {
      headers: this.auth.headers(),
    });
  }
}
