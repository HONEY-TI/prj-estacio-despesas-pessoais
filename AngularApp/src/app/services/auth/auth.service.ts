import { Injectable } from '@angular/core';
import { TokenStorageService } from '../token/token.storage.service';
import { AcessoService } from '../api';
import { AuthServiceBase } from './auth.abstract.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends AuthServiceBase {
  public accessToken$ = this.accessTokenSubject.asObservable();

  constructor(
    public override tokenStorage: TokenStorageService,
    protected override acessoService: AcessoService,
    protected override router: Router) {
      super(acessoService, tokenStorage, router);

      const token = this.tokenStorage.getAccessToken();
      if (token) {
        this.accessTokenSubject.next(token);
        this.isAuthenticated$.next(true);
      } else {
        this.tokenStorage.clearSessionStorage();
      }
  }

}
