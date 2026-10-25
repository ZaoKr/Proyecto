import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  imports: [FormsModule, RouterLink],
  selector: 'app-login',
  templateUrl: './login.html',
})
export class Login {
  protected email = '';
  protected password = '';
  protected error = '';
  protected cargando = false;

  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected iniciarSesion(): void {
    if (!this.email.trim() || !this.password) {
      this.error = 'Completa el correo y la contraseña.';
      return;
    }

    this.cargando = true;
    this.error = '';

    this.auth.iniciarSesion(this.email.trim(), this.password).subscribe({
      next: () => {
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (respuesta) => {
        this.error = respuesta.error?.mensaje ?? 'No se pudo iniciar sesión.';
        this.cargando = false;
      },
    });
  }
}
