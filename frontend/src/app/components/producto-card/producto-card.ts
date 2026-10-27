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

  readonly imgUrlDefault = computed(() => {
    return this.producto().imgUrl || 'https://img.magnific.com/vector-gratis/taza-cafe-granos-cafe_78370-7090.jpg?semt=ais_hybrid&w=740&q=80';
  });



}
