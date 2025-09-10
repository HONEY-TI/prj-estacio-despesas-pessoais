import { of, throwError } from 'rxjs';

export class MockImagemPerfil {
  private mockBuffer: ArrayBuffer;

  constructor() {
    const data = new TextEncoder().encode("fake image content");
    this.mockBuffer = data.buffer;
  }

  getImagemPerfilUsuario() {
    return of(this.mockBuffer);
  }

  updateImagemPerfilUsuario(file: File) {
    return of(this.mockBuffer);
  }

  deleteImagemPerfilUsuario() {
    return of(true);
  }
}
