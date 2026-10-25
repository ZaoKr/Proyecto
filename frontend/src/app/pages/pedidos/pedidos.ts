import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Pedido } from '../../models/pedido';
import { AuthService } from '../../services/auth.service';
import { PedidoService } from '../../services/pedido.service';

@Component({
  imports: [CurrencyPipe, DatePipe, RouterLink],
  selector: 'app-pedidos',
  templateUrl: './pedidos.html',
})
export class Pedidos {
  protected readonly auth = inject(AuthService);
  protected readonly pedidos = signal<Pedido[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly mensaje = signal('');
  protected readonly creado = inject(ActivatedRoute).snapshot.queryParamMap.get('creado') === '1';

  private readonly pedidoService = inject(PedidoService);

  constructor() {
    if (!this.auth.logueado()) {
      this.cargando.set(false);
      return;
    }

    this.cargar();
  }

  protected cargar(): void {
    this.cargando.set(true);
    this.error.set('');

    this.pedidoService.listar().subscribe({
      next: (pedidos) => {
        this.pedidos.set(pedidos);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar los pedidos.');
        this.cargando.set(false);
      },
    });
  }

  protected cancelar(id: number): void {
    if (!confirm('¿Cancelar este pedido?')) {
      return;
    }

    this.cambiarEstado(id, 'Cancelado');
  }

  protected cambiarEstado(id: number, estado: string): void {
    this.mensaje.set('');
    this.error.set('');

    this.pedidoService.cambiarEstado(id, estado).subscribe({
      next: () => {
        this.mensaje.set('Pedido actualizado.');
        this.cargar();
      },
      error: (respuesta) => {
        this.error.set(respuesta.error?.mensaje ?? 'No se pudo actualizar el pedido.');
      },
    });
  }

  protected siguientes(estado: string): string[] {
    switch (estado) {
      case 'Pendiente':
        return ['EnPreparacion', 'Cancelado'];
      case 'EnPreparacion':
        return ['Listo', 'Cancelado'];
      case 'Listo':
        return ['Entregado', 'Cancelado'];
      default:
        return [];
    }
  }

  protected textoEstado(estado: string): string {
    switch (estado) {
      case 'EnPreparacion':
        return 'En preparación';
      case 'Listo':
        return 'Listo';
      case 'Entregado':
        return 'Entregado';
      case 'Cancelado':
        return 'Cancelado';
      default:
        return 'Pendiente';
    }
  }

  protected claseEstado(estado: string): string {
    const base = 'rounded-full px-3 py-1 text-xs font-semibold';

    switch (estado) {
      case 'Pendiente':
        return `${base} bg-amber-100 text-amber-800`;
      case 'EnPreparacion':
        return `${base} bg-blue-100 text-blue-800`;
      case 'Listo':
        return `${base} bg-emerald-100 text-emerald-800`;
      case 'Entregado':
        return `${base} bg-gray-200 text-gray-700`;
      case 'Cancelado':
        return `${base} bg-red-100 text-red-700`;
      default:
        return `${base} bg-gray-100 text-gray-700`;
    }
  }
}
