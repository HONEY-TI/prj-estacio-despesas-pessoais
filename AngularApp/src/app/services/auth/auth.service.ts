import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { TokenStorageService } from '..';
import { IAuth } from '../../models';
import { HttpClient } from '@angular/common/http';
import { AcessoService } from '../api';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private accessTokenSubject = new BehaviorSubject<string | undefined>(undefined);
  private route: string =  'Acesso';

  accessToken$ = this.accessTokenSubject.asObservable();

  constructor(private tokenStorage: TokenStorageService, private acessoService: AcessoService) {
    try {
      const accessToken = this.tokenStorage.getAccessToken();
      if (accessToken) {
        this.setAccessToken(accessToken);
      } else {
        this.clearSessionStorage();
      }
    } catch {
      this.clearSessionStorage();
    }
  }

  private setAccessToken(token: string | undefined) {
    this.accessTokenSubject.next(token);
  }

  public clearSessionStorage() {
    this.setAccessToken(undefined);
    sessionStorage.clear();
  }

  public isAuthenticated(): boolean {
    const accessToken = this.tokenStorage.getAccessToken() ?? this.accessTokenSubject.getValue();
    if (accessToken === null || accessToken === undefined) {
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
      return true;
    } catch (error) {
      return false;
    }
  }

  public refreshToken(token: string): Observable<IAuth>{
    return this.acessoService.refreshToken(token);
  }
}
