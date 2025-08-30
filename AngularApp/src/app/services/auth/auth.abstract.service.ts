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

      // Salva os novos tokens retornados
      if (result.accessToken) this.tokenStorage.saveToken(result.accessToken);
      if (result.refreshToken) this.tokenStorage.saveRefreshToken(result.refreshToken);

      this.accessTokenSubject.next(result.accessToken);
      this.isAuthenticated$.next(true);
    } catch (error) {
      this.logout();
    }
  }

  protected refreshToken(token: string): Observable<IAuth> {
    return this.acessoService.refreshToken(token);
  }

  protected logout(): void {
    this.tokenStorage.signOut();
    this.accessTokenSubject.next(undefined);
    this.isAuthenticated$.next(false);
    this.router.navigate(['/']);
  }
}
