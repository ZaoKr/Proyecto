import { CurrencyPipe } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { Producto } from '../../models/producto';

@Component({
  imports: [CurrencyPipe],
  selector: 'app-producto-card',
  styleUrl: './producto-card.css',
  templateUrl: './producto-card.html',
})
export class ProductoCard {
  readonly producto = input.required<Producto>();
  readonly agregar = output<Producto>();

  private readonly iconosPorProducto: Record<string, string> = {
    'Café americano': '☕',
    Cappuccino: '☕',
    Latte: '☕',
    'Chocolate caliente': '🍫',
    'Té verde': '🍵',
    'Cold brew': '🧊',
    'Frappé de café': '🥤',
    'Limonada frozen': '🍋',
    'Jugo de naranja': '🍊',
    Croissant: '🥐',
    Cheesecake: '🍰',
    'Muffin de arándanos': '🧁',
    Brownie: '🍫',
    'Sándwich de pollo': '🥪',
    'Empanada de carne': '🥟',
    'Ensalada César': '🥗',
  };

  protected readonly icono = computed(() => {
    const producto = this.producto();
    const iconoProducto = this.iconosPorProducto[producto.nombre];

    if (iconoProducto) {
      return iconoProducto;
    }

    switch (producto.categoria.nombre) {
      case 'Bebidas calientes':
        return '☕';
      case 'Bebidas frías':
        return '🧋';
      case 'Repostería':
        return '🥐';
      case 'Comida':
        return '🥪';
      default:
        return '🍽️';
    }
  });
}
