import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { API_URL } from '../api.config';
import { SesionRespuesta, Usuario } from '../models/usuario';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  readonly usuario = signal<Usuario | null>(this.leerUsuario());
  readonly token = signal<string | null>(localStorage.getItem('token'));
  readonly logueado = computed(() => this.usuario() !== null);
  readonly esAdmin = computed(() => this.usuario()?.rol === 'admin');

  iniciarSesion(email: string, password: string): Observable<SesionRespuesta> {
    return this.http.post<SesionRespuesta>(`${API_URL}/auth/login`, { email, password }).pipe(
      tap((respuesta) => this.guardarSesion(respuesta)),
    );
  }

  registrar(nombre: string, email: string, password: string): Observable<SesionRespuesta> {
    return this.http
      .post<SesionRespuesta>(`${API_URL}/auth/registro`, { nombre, email, password })
      .pipe(tap((respuesta) => this.guardarSesion(respuesta)));
  }

  cerrarSesion(): void {
    this.http.post(`${API_URL}/auth/logout`, {}, { headers: this.headers() }).subscribe({
      error: () => {},
    });

    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
    this.token.set(null);
    this.usuario.set(null);
  }

  headers(): HttpHeaders {
    return new HttpHeaders({ Authorization: `Bearer ${this.token() ?? ''}` });
  }

  private guardarSesion(respuesta: SesionRespuesta): void {
    localStorage.setItem('token', respuesta.token);
    localStorage.setItem('usuario', JSON.stringify(respuesta.usuario));
    this.token.set(respuesta.token);
    this.usuario.set(respuesta.usuario);
  }

  private leerUsuario(): Usuario | null {
    const texto = localStorage.getItem('usuario');

    if (!texto) {
      return null;
    }

    try {
      return JSON.parse(texto) as Usuario;
    } catch {
      return null;
    }
  }
}
