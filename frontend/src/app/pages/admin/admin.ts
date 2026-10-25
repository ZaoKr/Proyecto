import { CurrencyPipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Resumen } from '../../models/pedido';
import { Categoria, Producto, ProductoRequest } from '../../models/producto';
import { AdminService } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';
import { CatalogoService } from '../../services/catalogo.service';

@Component({
  imports: [CurrencyPipe, FormsModule, RouterLink],
  selector: 'app-admin',
  templateUrl: './admin.html',
})
export class Admin {
  protected readonly auth = inject(AuthService);
  protected readonly resumen = signal<Resumen | null>(null);
  protected readonly productos = signal<Producto[]>([]);
  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly error = signal('');
  protected readonly mensaje = signal('');
  protected readonly cargando = signal(true);

  protected seccion: 'resumen' | 'productos' | 'categorias' = 'resumen';
  protected productoEditandoId: number | null = null;
  protected productoForm: ProductoRequest = {
    nombre: '',
    descripcion: '',
    precio: 0,
    disponible: true,
    categoriaId: 0,
  };
  protected categoriaEditandoId: number | null = null;
  protected categoriaNombre = '';

  private readonly adminService = inject(AdminService);
  private readonly catalogo = inject(CatalogoService);

  constructor() {
    if (!this.auth.esAdmin()) {
      this.cargando.set(false);
      return;
    }

    this.cargarTodo();
  }

  protected cargarTodo(): void {
    this.cargando.set(true);
    this.error.set('');

    forkJoin({
      resumen: this.adminService.resumen(),
      productos: this.catalogo.listarProductos(),
      categorias: this.catalogo.listarCategorias(),
    }).subscribe({
      next: ({ resumen, productos, categorias }) => {
        this.resumen.set(resumen);
        this.productos.set(productos);
        this.categorias.set(categorias);

        if (!this.productoForm.categoriaId && categorias.length > 0) {
          this.productoForm.categoriaId = categorias[0].id;
        }

        this.cargando.set(false);
      },
      error: () => {
        this.error.set('No se pudo cargar la administración.');
        this.cargando.set(false);
      },
    });
  }

  protected guardarProducto(): void {
    this.error.set('');
    this.mensaje.set('');

    if (
      !this.productoForm.nombre.trim() ||
      !this.productoForm.categoriaId ||
      this.productoForm.precio === null ||
      this.productoForm.precio === undefined ||
      this.productoForm.precio < 0
    ) {
      this.error.set('Completa el nombre, el precio y la categoría.');
      return;
    }

    const peticion = this.productoEditandoId
      ? this.adminService.editarProducto(this.productoEditandoId, this.productoForm)
      : this.adminService.crearProducto(this.productoForm);

    peticion.subscribe({
      next: () => {
        this.mensaje.set(this.productoEditandoId ? 'Producto actualizado.' : 'Producto creado.');
        this.limpiarProducto();
        this.cargarTodo();
      },
      error: (respuesta) => {
        this.error.set(respuesta.error?.mensaje ?? 'No se pudo guardar el producto.');
      },
    });
  }

  protected editarProducto(producto: Producto): void {
    this.productoEditandoId = producto.id;
    this.productoForm = {
      nombre: producto.nombre,
      descripcion: producto.descripcion,
      precio: producto.precio,
      disponible: producto.disponible,
      categoriaId: producto.categoriaId,
    };
    this.seccion = 'productos';
  }

  protected limpiarProducto(): void {
    this.productoEditandoId = null;
    this.productoForm = {
      nombre: '',
      descripcion: '',
      precio: 0,
      disponible: true,
      categoriaId: this.categorias()[0]?.id ?? 0,
    };
  }

  protected eliminarProducto(producto: Producto): void {
    if (!confirm('¿Eliminar ' + producto.nombre + '?')) {
      return;
    }

    this.adminService.eliminarProducto(producto.id).subscribe({
      next: () => {
        this.mensaje.set('Producto eliminado.');
        this.cargarTodo();
      },
      error: (respuesta) => {
        this.error.set(respuesta.error?.mensaje ?? 'No se pudo eliminar el producto.');
      },
    });
  }

  protected cambiarDisponibilidad(producto: Producto): void {
    this.adminService.cambiarDisponibilidad(producto.id, !producto.disponible).subscribe({
      next: () => {
        this.mensaje.set('Disponibilidad actualizada.');
        this.cargarTodo();
      },
      error: (respuesta) => {
        this.error.set(respuesta.error?.mensaje ?? 'No se pudo actualizar la disponibilidad.');
      },
    });
  }

  protected guardarCategoria(): void {
    this.error.set('');
    this.mensaje.set('');

    if (this.categoriaNombre.trim().length < 2) {
      this.error.set('Escribe un nombre de al menos 2 letras.');
      return;
    }

    const peticion = this.categoriaEditandoId
      ? this.adminService.editarCategoria(this.categoriaEditandoId, this.categoriaNombre.trim())
      : this.adminService.crearCategoria(this.categoriaNombre.trim());

    peticion.subscribe({
      next: () => {
        this.mensaje.set(
          this.categoriaEditandoId ? 'Categoría actualizada.' : 'Categoría creada.',
        );
        this.limpiarCategoria();
        this.cargarTodo();
      },
      error: (respuesta) => {
        this.error.set(respuesta.error?.mensaje ?? 'No se pudo guardar la categoría.');
      },
    });
  }

  protected editarCategoria(categoria: Categoria): void {
    this.categoriaEditandoId = categoria.id;
    this.categoriaNombre = categoria.nombre;
    this.seccion = 'categorias';
  }

  protected limpiarCategoria(): void {
    this.categoriaEditandoId = null;
    this.categoriaNombre = '';
  }

  protected eliminarCategoria(categoria: Categoria): void {
    if (!confirm('¿Eliminar ' + categoria.nombre + '?')) {
      return;
    }

    this.adminService.eliminarCategoria(categoria.id).subscribe({
      next: () => {
        this.mensaje.set('Categoría eliminada.');
        this.cargarTodo();
      },
      error: (respuesta) => {
        this.error.set(respuesta.error?.mensaje ?? 'No se pudo eliminar la categoría.');
      },
    });
  }

  protected claseSeccion(seccion: string): string {
    const base = 'rounded-lg px-4 py-2 text-sm font-medium';

    if (this.seccion === seccion) {
      return `${base} bg-amber-700 text-white`;
    }

    return `${base} border border-amber-200 bg-white text-amber-800 hover:bg-amber-50`;
  }
}
