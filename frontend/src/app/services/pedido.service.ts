import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { API_URL } from '../api.config';
import { Pedido, PedidoRequest } from '../models/pedido';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class PedidoService {
  private readonly http = inject(HttpClient);
  private readonly auth = inject(AuthService);

  crear(pedido: PedidoRequest): Observable<Pedido> {
    return this.http.post<Pedido>(`${API_URL}/pedidos`, pedido, {
      headers: this.auth.headers(),
    });
  }

  listar(): Observable<Pedido[]> {
    return this.http.get<Pedido[]>(`${API_URL}/pedidos`, {
      headers: this.auth.headers(),
    });
  }

  cambiarEstado(id: number, estado: string): Observable<Pedido> {
    return this.http.patch<Pedido>(
      `${API_URL}/pedidos/${id}/estado`,
      { estado },
      { headers: this.auth.headers() },
    );
  }
}
