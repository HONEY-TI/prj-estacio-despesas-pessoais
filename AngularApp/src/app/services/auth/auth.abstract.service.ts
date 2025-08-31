import { Injectable } from '@angular/core';
import { BehaviorSubject, firstValueFrom, Observable } from 'rxjs';
import { Router } from '@angular/router';
import { TokenStorageService } from '../token/token.storage.service';
import { AcessoService } from '../api';
import { IAuth } from '../../models';

@Injectable({
  providedIn: 'root'
})
export abstract class AuthServiceBase {
  public isAuthenticated$ = new BehaviorSubject<boolean>(false);
  public accessTokenSubject = new BehaviorSubject<string | undefined>(undefined);
  public route: string = 'Acesso';

  constructor(
    protected acessoService: AcessoService,
    protected tokenStorage: TokenStorageService,
    protected router: Router) { }

  async autoLogin(): Promise<void> {
    const refreshToken = this.tokenStorage.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return;
    }

    try {
      const result: IAuth = await firstValueFrom(this.refreshToken(refreshToken));

      if (result.accessToken) this.tokenStorage.saveToken(result.accessToken);
      if (result.refreshToken) this.tokenStorage.saveRefreshToken(result.refreshToken);

      this.accessTokenSubject.next(result.accessToken);
      this.isAuthenticated$.next(true);
      this.router.navigate(['/dashboard']);
    } catch (error) {
      this.logout();
    }
  }

  protected setAccessToken(token?: string) {
    this.accessTokenSubject.next(token);
  }

  protected logout(): void {
    this.tokenStorage.signOut();
    this.accessTokenSubject.next(undefined);
    this.isAuthenticated$.next(false);
    this.router.navigate(['/']);
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

  public clearSessionStorage() {
    this.setAccessToken(undefined);
    sessionStorage.clear();
  }

  public refreshToken(token: string): Observable<IAuth> {
    return this.acessoService.refreshToken(token);
  }


  public isAuthenticated(): boolean {
    const token = this.tokenStorage.getAccessToken() ?? this.accessTokenSubject.getValue();
    if (!token) {
      this.clearSessionStorage();
      return false;
    }
    return true;
  }
}
