import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AbstractService } from '../base/AbstractService';
import { Observable, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ImagemPerfilService extends AbstractService {
  private cachedImage: ArrayBuffer | null = null;

  constructor(public httpClient: HttpClient) {
    const ROUTE = 'Usuario';
    super(ROUTE);
  }

  getImagemPerfilUsuario(): Observable<ArrayBuffer> {
    if (this.cachedImage) {
      return of(this.cachedImage);
    }

    return this.httpClient
      .get(`${this.routeUrl}/GetProfileImage`, { responseType: 'arraybuffer' })
      .pipe(
        tap((response: ArrayBuffer) => {
          if (response && response.byteLength > 0) {
            this.cachedImage = response;
          }
        })
      );
  }

  updateImagemPerfilUsuario(file: File): Observable<ArrayBuffer> {
    const formData = new FormData();
    formData.append('file', file);

    return this.httpClient
      .put(`${this.routeUrl}/UpdateProfileImage`, formData, { responseType: 'arraybuffer' })
      .pipe(
        tap((response: ArrayBuffer) => {
          if (response && response.byteLength > 0) {
            this.cachedImage = response;
          }
        })
      );
  }
}
