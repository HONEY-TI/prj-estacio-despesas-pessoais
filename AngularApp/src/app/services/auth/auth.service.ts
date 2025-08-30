import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TokenStorageService } from '../token/token.storage.service';
import { AcessoService } from '../api';
import { IAuth } from '../../models';
import { AuthServiceBase } from './auth.abstract.service';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends AuthServiceBase {
  public accessToken$ = this.accessTokenSubject.asObservable();

  constructor(
    protected override tokenStorage: TokenStorageService,
    protected override acessoService: AcessoService,
    protected override router: Router) {
      super(acessoService, tokenStorage, router);

      const token = this.tokenStorage.getAccessToken();
      if (token) {
        this.accessTokenSubject.next(token);
        this.isAuthenticated$.next(true);
      } else {
        this.clearSessionStorage();
      }
  }

  private setAccessToken(token?: string) {
    this.accessTokenSubject.next(token);
  }

  public clearSessionStorage() {
    this.setAccessToken(undefined);
    sessionStorage.clear();
  }

  public isAuthenticated(): boolean {
    const token = this.tokenStorage.getAccessToken() ?? this.accessTokenSubject.getValue();
    if (!token) {
      this.clearSessionStorage();
      return false;
    }
    return true;
  }

  public createAccessToken(auth: IAuth): boolean {
    try {
      this.tokenStorage.saveToken(auth.accessToken);
      this.tokenStorage.saveRefreshToken(auth.refreshToken);
      this.setAccessToken(auth.accessToken);
      this.isAuthenticated$.next(true);
      return true;
    } catch {
      return false;
    }
  }

  public override refreshToken(token: string): Observable<IAuth> {
    return this.acessoService.refreshToken(token);
  }
}
