import { Producto } from './producto';

export interface PedidoItem {
  id: number;
  pedidoId: number;
  productoId: number;
  producto: Producto;
  cantidad: number;
  precioUnitario: number;
  notas: string;
}

export interface Pedido {
  id: number;
  usuarioId: number;
  usuario?: {
    id: number;
    nombre: string;
    email: string;
    rol: string;
  };
  tipo: string;
  numeroMesa: number | null;
  metodoPago: string;
  estado: string;
  total: number;
  notas: string;
  creadoEn: string;
  items: PedidoItem[];
}

export interface PedidoRequest {
  tipo: string;
  numeroMesa: number | null;
  metodoPago: string;
  notas: string;
  items: {
    productoId: number;
    cantidad: number;
    notas: string;
  }[];
}

export interface Resumen {
  pedidosHoy: number;
  ventasHoy: number;
  pendientes: number;
  enPreparacion: number;
  listos: number;
  clientes: number;
  productos: number;
  topProductos: {
    nombre: string;
    cantidad: number;
  }[];
}
