import { Injectable } from '@angular/core';
import { TokenStorageService } from '../token/token.storage.service';
import { Router } from '@angular/router';
import { AuthServiceBase } from './auth.abstract.service';
import { HttpClient } from '@angular/common/http';
import { AcessoService } from '../api/acesso/acesso.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends AuthServiceBase {

  constructor(
    httpClient: HttpClient,
    tokenStorage: TokenStorageService,
    router: Router,
    acessoService: AcessoService ) {
    super(httpClient, tokenStorage, router, acessoService);
    const token = this.tokenStorage.getAccessToken();
    if (token) {
      this.accessTokenSubject.next(token);
      this.isAuthenticated$.next(true);
    }
  }

}
