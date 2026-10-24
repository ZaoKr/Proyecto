import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProductosMenu } from './components/productos-menu/productos-menu';

@Component({
  imports: [RouterOutlet, ProductosMenu],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('Frontend');
}
