import { Injectable } from '@angular/core';

const TOKEN_KEY = '@access-token';
const REFRESHTOKEN_KEY = '@refresh-token';

@Injectable({
  providedIn: 'root'
})

export class TokenStorageService {

  constructor() { }

  signOut(): void {
    sessionStorage.clear();
    location.reload();
  }

  public saveToken(token: string): void {
    sessionStorage.setItem(TOKEN_KEY, token);
  }

  public getToken(): string | null {
    return sessionStorage.getItem(TOKEN_KEY);
  }

  public saveRefreshToken(token: string): void {
    sessionStorage.setItem(REFRESHTOKEN_KEY, token);
  }

  public getRefreshToken(): string | null {
    return sessionStorage.getItem(REFRESHTOKEN_KEY);
  }
}
