import { CurrencyPipe } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { PedidoRequest } from '../../models/pedido';
import { AuthService } from '../../services/auth.service';
import { CarritoService } from '../../services/carrito.service';
import { PedidoService } from '../../services/pedido.service';

@Component({
  imports: [CurrencyPipe, FormsModule, RouterLink],
  selector: 'app-carrito',
  templateUrl: './carrito.html',
})
export class Carrito {
  protected readonly carrito = inject(CarritoService);
  protected readonly auth = inject(AuthService);

  private readonly pedidos = inject(PedidoService);
  private readonly router = inject(Router);

  protected tipo = 'ParaLlevar';
  protected numeroMesa: number | null = null;
  protected metodoPago = 'Efectivo';
  protected notas = '';
  protected error = '';
  protected enviando = false;

  protected confirmar(): void {
    if (!this.auth.logueado()) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: '/carrito' } });
      return;
    }

    if (this.carrito.cantidad() === 0) {
      this.error = 'El carrito está vacío.';
      return;
    }

    if (this.tipo === 'Mesa' && (!this.numeroMesa || this.numeroMesa < 1)) {
      this.error = 'Escribe el número de mesa.';
      return;
    }

    const pedido: PedidoRequest = {
      tipo: this.tipo,
      numeroMesa: this.tipo === 'Mesa' ? Number(this.numeroMesa) : null,
      metodoPago: this.metodoPago,
      notas: this.notas.trim(),
      items: this.carrito.contenido().map((item) => ({
        productoId: item.producto.id,
        cantidad: item.cantidad,
        notas: '',
      })),
    };

    this.enviando = true;
    this.error = '';

    this.pedidos.crear(pedido).subscribe({
      next: () => {
        this.carrito.vaciar();
        this.router.navigate(['/pedidos'], { queryParams: { creado: 1 } });
      },
      error: (respuesta) => {
        this.error = respuesta.error?.mensaje ?? 'No se pudo crear el pedido.';
        this.enviando = false;
      },
    });
  }
}
