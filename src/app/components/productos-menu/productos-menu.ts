import { Component } from '@angular/core';
import { ProductoCard } from '../producto-card/producto-card';

@Component({
  imports: [ProductoCard],
  selector: 'app-productos-menu',
  styleUrl: './productos-menu.css',
  templateUrl: './productos-menu.html',
})
export class ProductosMenu {}
