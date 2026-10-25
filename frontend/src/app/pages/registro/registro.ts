import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  imports: [FormsModule, RouterLink],
  selector: 'app-registro',
  templateUrl: './registro.html',
})
export class Registro {
  protected nombre = '';
  protected email = '';
  protected password = '';
  protected confirmar = '';
  protected error = '';
  protected cargando = false;

  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected registrarse(): void {
    if (!this.nombre.trim() || !this.email.trim() || !this.password) {
      this.error = 'Completa todos los campos.';
      return;
    }

    if (this.password.length < 6) {
      this.error = 'La contraseña debe tener al menos 6 caracteres.';
      return;
    }

    if (this.password !== this.confirmar) {
      this.error = 'Las contraseñas no coinciden.';
      return;
    }

    this.cargando = true;
    this.error = '';

    this.auth.registrar(this.nombre.trim(), this.email.trim(), this.password).subscribe({
      next: () => this.router.navigate(['/']),
      error: (respuesta) => {
        this.error = respuesta.error?.mensaje ?? 'No se pudo crear la cuenta.';
        this.cargando = false;
      },
    });
  }
}
