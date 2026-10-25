export interface Usuario {
  id: number;
  nombre: string;
  email: string;
  rol: string;
}

export interface SesionRespuesta {
  token: string;
  usuario: Usuario;
}
