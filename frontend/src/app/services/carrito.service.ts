import { Injectable, computed, signal } from '@angular/core';
import { Producto } from '../models/producto';

export interface ItemCarrito {
  producto: Producto;
  cantidad: number;
}

@Injectable({ providedIn: 'root' })
export class CarritoService {
  private readonly items = signal<ItemCarrito[]>(this.leer());

  readonly contenido = this.items.asReadonly();
  readonly cantidad = computed(() => this.items().reduce((total, item) => total + item.cantidad, 0));
  readonly total = computed(() =>
    this.items().reduce((total, item) => total + item.producto.precio * item.cantidad, 0),
  );

  agregar(producto: Producto): void {
    const actuales = this.items();
    const existente = actuales.find((item) => item.producto.id === producto.id);

    if (existente) {
      if (existente.cantidad >= 20) {
        return;
      }

      this.items.set(
        actuales.map((item) =>
          item.producto.id === producto.id ? { ...item, cantidad: item.cantidad + 1 } : item,
        ),
      );
    } else {
      this.items.set([...actuales, { producto, cantidad: 1 }]);
    }

    this.guardar();
  }

  cambiarCantidad(productoId: number, cantidad: number): void {
    if (cantidad < 1) {
      this.quitar(productoId);
      return;
    }

    if (cantidad > 20) {
      cantidad = 20;
    }

    this.items.set(
      this.items().map((item) =>
        item.producto.id === productoId ? { ...item, cantidad } : item,
      ),
    );
    this.guardar();
  }

  quitar(productoId: number): void {
    this.items.set(this.items().filter((item) => item.producto.id !== productoId));
    this.guardar();
  }

  vaciar(): void {
    this.items.set([]);
    localStorage.removeItem('carrito');
  }

  private guardar(): void {
    localStorage.setItem('carrito', JSON.stringify(this.items()));
  }

  private leer(): ItemCarrito[] {
    const texto = localStorage.getItem('carrito');

    if (!texto) {
      return [];
    }

    try {
      return JSON.parse(texto) as ItemCarrito[];
    } catch {
      return [];
    }
  }
}
