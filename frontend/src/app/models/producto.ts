export interface Categoria {
  id: number;
  nombre: string;
  cantidadProductos?: number;
}

export interface Producto {
  id: number;
  nombre: string;
  descripcion: string;
  precio: number;
  disponible: boolean;
  categoriaId: number;
  categoria: Categoria;
}

export interface ProductoRequest {
  nombre: string;
  descripcion: string;
  precio: number;
  disponible: boolean;
  categoriaId: number;
}
