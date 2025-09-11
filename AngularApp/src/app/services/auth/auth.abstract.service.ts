import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { TokenStorageService } from '../token/token.storage.service';
import { IAuth } from '../../models';
import { HttpClient } from '@angular/common/http';
import { AcessoService } from '../api/acesso/acesso.service';

@Injectable({ providedIn: 'root' })

export abstract class AuthServiceBase {
  public accessTokenSubject = new BehaviorSubject<string | undefined>(undefined);
  isAuthenticated$ = new BehaviorSubject<boolean>(false);
  protected route: string = 'api/Acesso'

  constructor(
    protected httpClient: HttpClient,
    protected tokenStorage: TokenStorageService,
    private router: Router,
    public acessoService: AcessoService) {

    const token = this.tokenStorage.getAccessToken();
    if (token) {
      this.accessTokenSubject.next(token);
      this.isAuthenticated$.next(true);
    } else {
      this.accessTokenSubject.next(undefined);
      this.isAuthenticated$.next(false);
    }
  }

  get accessToken$(): Observable<string | undefined> {
    return this.accessTokenSubject.asObservable();
  }

  login(auth: IAuth): void {
    this.tokenStorage.saveAccessToken(auth.accessToken);
    this.tokenStorage.saveRefreshToken(auth.refreshToken);
    this.accessTokenSubject.next(auth.accessToken);
    this.isAuthenticated$.next(true);
  }

async autoLogin(): Promise<void> {
    const refreshToken = this.tokenStorage.getRefreshToken();
    if (!refreshToken) {
      this.logout();

      return;
    }

    try {
      const result: IAuth = await firstValueFrom(this.acessoService.refreshToken(refreshToken));

      if (result.accessToken) this.tokenStorage.saveAccessToken(result.accessToken);
      if (result.refreshToken) this.tokenStorage.saveRefreshToken(result.refreshToken);

      this.accessTokenSubject.next(result.accessToken);
      this.isAuthenticated$.next(true);
      this.router.navigate(['/dashboard']);
    } catch  {
      this.router.navigate(['/']);
    }
  }

  logout(): void {
    this.accessTokenSubject.next(undefined);
    this.isAuthenticated$.next(false);
    this.tokenStorage.clear();
  }

  isAuthenticated(): boolean {
    const token = this.accessTokenSubject.getValue() ?? this.tokenStorage.getAccessToken();
    return !!token;
  }
}
