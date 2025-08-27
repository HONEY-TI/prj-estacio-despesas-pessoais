import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AbstractService } from '../base/AbstractService';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class ImagemPerfilService extends AbstractService {
  constructor(public httpClient: HttpClient) {
    const ROUTE = 'Usuario';
    super(ROUTE);
  }

  getImagemPerfilUsuario(): Observable<ArrayBuffer> {
    return this.httpClient.get(`${ this.routeUrl }/GetProfileImage`, { responseType: 'arraybuffer' });
  }

  updateImagemPerfilUsuario(file: File): Observable<ArrayBuffer> {
    const formData = new FormData();
    formData.append('file', file);
    return this.httpClient.put(`${ this.routeUrl }/UpdateProfileImage`, formData, { responseType: 'arraybuffer' });
  }
}
