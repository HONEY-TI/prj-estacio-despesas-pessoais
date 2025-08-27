import { Injectable } from '@angular/core';
import { catchError, map, Observable, throwError } from 'rxjs';
import { IAuth, IGoogleAuth } from '../../models';
import { environment } from '../../../environments/environment';
import { AcessoService } from '../api';

declare const google: any;

@Injectable({
  providedIn: 'root'
})

export class AuthGoogleService {
  private clientId: string = environment.client_id;
  private initialized = false;

  constructor(private acessoService: AcessoService) {
    this.initializeGoogleLogin();
  }

  private isGoogleScriptLoaded(): boolean {
    return typeof google !== 'undefined' && google.accounts && google.accounts.id;
  }

  private initializeGoogleLogin(): void {
    if (this.initialized) return;

    if (this.isGoogleScriptLoaded()) {
      google.accounts.id.initialize({
        client_id: this.clientId,
        callback: (response: any) => {
          if (response.credential) {
            this.handleCredentialResponse(response).subscribe({
              next: () => { },
              error: (err) => console.error('Erro ao processar credencial:', err)
            });
          }
        }
      });

      this.initialized = true;
    } else {
      console.error('Google API não carregada corretamente.');
    }
  }

  handleGoogleLogin(): Observable<any> {
    return new Observable<IAuth>((observer) => {
      if (!this.isGoogleScriptLoaded()) {
        observer.error(new Error('Google API não carregada.'));
        return;
      }

      google.accounts.id.initialize({
        client_id: this.clientId,
        callback: (response: any) => {
          if (response.credential) {
            this.handleCredentialResponse(response)
              .pipe(
                map((res) => {
                  if (res.authenticated) return res;
                  throw new Error('Autenticação falhou');
                }),
                catchError((err) => throwError(() => new Error(err?.message || 'Erro desconhecido')))
              )
              .subscribe({
                next: (auth) => {
                  observer.next(auth);
                  observer.complete();
                },
                error: (err) => observer.error(err)
              });
          } else {
            observer.error(new Error('Erro ao fazer login com o Google: credencial inválida'));
            observer.complete();
          }
        }
      });

      google.accounts.id.prompt();
    });
  }

  private handleCredentialResponse(response: any): Observable<IAuth> {
    const userData = this.decodeJwt(response.credential);
    if (!userData.sub || !userData.email) {
      throw new Error('Token JWT inválido ou incompleto');
    }
    const authData: IGoogleAuth = {
      authenticated: true,
      created: new Date().toISOString(),
      expiration: this.calculateExpiration(),
      accessToken: response.credential,
      refreshToken: '',
      externalId: userData.sub,
      externalProvider: "Google",
      nome: userData.given_name,
      sobreNome: userData.family_name,
      telefone: null,
      email: userData.email      
    };

    return this.acessoService.signInWithGoogleAccount(authData).pipe(
      catchError(err => throwError(() => new Error(err?.message || 'Erro desconhecido handleCredentialResponse')))
    );
  }

  private calculateExpiration(): string {
    const expirationDate = new Date();
    expirationDate.setHours(expirationDate.getHours() + 1);
    return expirationDate.toISOString();
  }

  private decodeJwt(token: string): any {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  }
}
