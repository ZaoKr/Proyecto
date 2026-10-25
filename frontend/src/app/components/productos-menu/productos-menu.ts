import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Categoria, Producto } from '../../models/producto';
import { CarritoService } from '../../services/carrito.service';
import { CatalogoService } from '../../services/catalogo.service';
import { ProductoCard } from '../producto-card/producto-card';

@Component({
  imports: [CurrencyPipe, ProductoCard, RouterLink],
  selector: 'app-productos-menu',
  styleUrl: './productos-menu.css',
  templateUrl: './productos-menu.html',
})
export class ProductosMenu {
  private readonly catalogo = inject(CatalogoService);
  protected readonly carrito = inject(CarritoService);

  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly productos = signal<Producto[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly mensaje = signal('');
  protected readonly categoriaSeleccionada = signal<number | null>(null);

  private temporizador?: ReturnType<typeof setTimeout>;

  constructor() {
    this.cargar();
  }

  protected cargar(categoriaId: number | null = null): void {
    this.categoriaSeleccionada.set(categoriaId);
    this.cargando.set(true);
    this.error.set('');

    forkJoin({
      categorias: this.catalogo.listarCategorias(),
      productos: this.catalogo.listarProductos(categoriaId ?? undefined),
    }).subscribe({
      next: ({ categorias, productos }) => {
        this.categorias.set(categorias);
        this.productos.set(productos);
        this.cargando.set(false);
      },
      error: () => {
        this.error.set(
          'No se pudo conectar con la API. Revisa que el backend esté encendido en http://localhost:5196.',
        );
        this.cargando.set(false);
      },
    });
  }

  protected agregar(producto: Producto): void {
    this.carrito.agregar(producto);
    this.mensaje.set(producto.nombre + ' agregado al carrito');

    if (this.temporizador) {
      clearTimeout(this.temporizador);
    }

    this.temporizador = setTimeout(() => this.mensaje.set(''), 2000);
  }

  protected claseFiltro(activo: boolean): string {
    const base = 'rounded-full border px-4 py-2 text-sm font-medium transition-colors';

    if (activo) {
      return `${base} border-amber-700 bg-amber-700 text-white`;
    }

    return `${base} border-amber-200 bg-white text-amber-800 hover:bg-amber-50`;
  }
}
