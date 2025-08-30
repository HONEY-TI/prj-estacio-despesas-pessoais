import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, filter, finalize, map, switchMap, take, throwError } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService, TokenStorageService } from '../app/services';
import { IAuth } from '../app/models';
import { LoadingComponent } from '../app/components';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class CustomInterceptor implements HttpInterceptor {
  private activeRequests = 0;
  private isModalOpen$ = new BehaviorSubject<boolean>(false);
  private refreshInProgress = false;
  private refreshCall$: Observable<IAuth> | null = null;
  private loadingModalRef: any = null;

  constructor(
    private tokenService: TokenStorageService,
    private authService: AuthService,
    private modalService: NgbModal) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.showLoader();

    // 🔹 Identifica se a requisição é para refresh token
    const isRefreshRequest = request.url.includes('/refreshtoken/');
    const modifiedRequest = isRefreshRequest ? request : this.setModifiedRequest(request);

    return next.handle(modifiedRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        // 🔹 Tenta refresh apenas se for 401 e não for o próprio endpoint de refresh
        if (error.status === 401 && !isRefreshRequest) {
          return this.handleRefreshToken(request, next);
        }
        return this.handleStandardError(error);
      }),
      finalize(() => this.hideLoader())
    );
  }

  /**
   * 🔹 Trata apenas erros normais (400, 403, 404 etc.)
   */
  private handleStandardError(error: HttpErrorResponse) {
    if (error.ok === false && error.status === 0) return throwError(() => 'Erro de conexão, tente mais tarde.');
    if (error.status === 400) return throwError(() => error.error);
    if (error.status === 403) return throwError(() => 'Acesso não autorizado!');
    if (error.status === 404) { 
      this.tokenService.signOut();
      return throwError(() => 'Recurso não encontrado.');
    }
    return throwError(() => 'Erro inesperado, atualize a página ou faça login novamente.');
  }

  /**
   * 🔹 Trata especificamente erro 401 (AccessToken inválido/expirado)
   * 🔹 Aguarda a conclusão de qualquer refresh em andamento antes de refazer a requisição
   */
  private handleRefreshToken(request: HttpRequest<any>, next: HttpHandler) {
    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) {
      this.tokenService.revokeRefreshToken();
      this.tokenService.signOut();
      this.tokenService.signOut();
      return throwError(() => 'Sessão expirada, faça login novamente.');
    }

    const isRefreshRequest = request.url.includes('/refreshtoken/');

    // 🔹 Se não existe refresh em andamento, inicia
    if (!this.refreshInProgress) {
      this.refreshInProgress = true;
      this.refreshCall$ = this.authService.refreshToken(refreshToken).pipe(
        finalize(() => {
          this.refreshInProgress = false;
          this.refreshCall$ = null;
        })
      );
    }

    // 🔹 Todas as requisições aguardam a mesma chamada de refresh
    return this.refreshCall$!.pipe(
      switchMap((auth: IAuth) => {
        this.tokenService.updateAccessToken(auth.accessToken);
        this.tokenService.saveRefreshToken(auth.refreshToken);

        // 🔹 Refaz a requisição original com novo accessToken, se não for refresh
        const req = isRefreshRequest ? request : this.setModifiedRequest(request);
        return next.handle(req);
      }),
      catchError(() => {
        this.tokenService.revokeRefreshToken();
        return throwError(() => 'Sessão expirada, faça login novamente.');
      })
    );
  }

  /**
   * 🔹 Clona a request e adiciona o Authorization Header
   */
  private setModifiedRequest(request: HttpRequest<any>) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${this.tokenService.getAccessToken()}`
      }
    });
  }

  /**
   * 🔹 Mostra o modal de Loading enquanto houver requests ativas
   */
  private showLoader(): void {
    this.activeRequests++;
    if (!this.isModalOpen$.value) {
      this.isModalOpen$.next(true);
      this.loadingModalRef = this.modalService.open(LoadingComponent, {
        centered: true,
        fullscreen: true,
        windowClass: 'loading-modal',
        backdrop: 'static',
        keyboard: false
      });
    }
  }

  /**
   * 🔹 Esconde o modal de Loading quando todas requests finalizarem
   */
  private hideLoader(): void {
    if (this.activeRequests > 0) this.activeRequests--;
    if (this.activeRequests === 0 && this.isModalOpen$.value) {
      this.isModalOpen$.next(false);
      if (this.loadingModalRef) {
        this.loadingModalRef.close();
        this.loadingModalRef = null;
      }
    }
  }
}
