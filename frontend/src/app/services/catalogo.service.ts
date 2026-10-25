import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { Categoria, Producto } from '../models/producto';

@Injectable({ providedIn: 'root' })
export class CatalogoService {
  private readonly http = inject(HttpClient);

  listarCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(`${API_URL}/categorias`);
  }

  listarProductos(categoriaId?: number): Observable<Producto[]> {
    let params = new HttpParams();

    if (categoriaId) {
      params = params.set('categoriaId', categoriaId);
    }

    return this.http.get<Producto[]>(`${API_URL}/productos`, { params });
  }
}
