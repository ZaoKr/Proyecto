import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { CarritoService } from './services/carrito.service';

@Component({
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly auth = inject(AuthService);
  protected readonly carrito = inject(CarritoService);
  private readonly router = inject(Router);

  protected cerrarSesion(): void {
    this.auth.cerrarSesion();
    this.router.navigate(['/']);
  }
}
