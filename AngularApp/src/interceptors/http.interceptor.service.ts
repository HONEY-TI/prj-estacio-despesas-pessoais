import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, filter, finalize, map, switchMap, take, throwError } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService, TokenStorageService } from '../app/services';
import { IAuth } from '../app/models';
import { LoadingComponent } from '../app/components';

@Injectable({
  providedIn: 'root'
})
export class CustomInterceptor implements HttpInterceptor {
  private activeRequests: number = 0;
  private isModalOpen$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  private loadingModalRef: any = null;

  constructor(
    private tokenService: TokenStorageService,
    private authService: AuthService,
    private modalService: NgbModal
  ) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.showLoader();
    const modifiedRequest = this.setModifiedRequest(request);

    return next.handle(modifiedRequest).pipe(
      map((response: HttpResponse<any>) => {
        if (response.status === 200) {
          return response;
        }
        return response;
      }),
      catchError((error: HttpErrorResponse) => {
        if (error.ok === false && error.status === 0) {
          return throwError(() => 'Erro de conexão tente mais tarde.');
        } else if (error.status === 400) {
          return throwError(() => error.error);
        } else if (error.status === 401) {
          return this.handleError(request, next);
        } else if (error.status === 403) {
          return throwError(() => 'Acesso não autorizado!');
        } else if (error.status === 404) {
          return this.handleError(request, next);
        }
        return throwError(() => 'Erro tente atualizar a página ou realize novamente o login.');
      }),
      finalize(() => {
        this.hideLoader();
      })
    );
  }

  private handleError(request: HttpRequest<any>, next: HttpHandler) {
    this.refreshTokenSubject.next(null);

    const auth = this.tokenService.getRefreshToken();
    if (auth) {
      return this.authService.refreshToken(auth).pipe(
        switchMap((auth: IAuth) => {
          this.tokenService.saveToken(auth.accessToken);
          this.tokenService.saveRefreshToken(auth.refreshToken);
          this.refreshTokenSubject.next(auth.refreshToken);
          this.hideLoader();
          return next.handle(this.setModifiedRequest(request));
        }),
        catchError(() => {
          sessionStorage.clear();
          this.tokenService.signOut();
          return throwError(
            () =>
              'Erro de autenticação, tente atualizar a página ou realize novamente o login.'
          );
        })
      );
    }

    return this.refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => next.handle(this.setModifiedRequest(request)))
    );
  }

  private setModifiedRequest(request: HttpRequest<any>) {

    return request.clone({
      url: `${request.url}`,
      setHeaders: {
        Authorization: `Bearer ${sessionStorage.getItem('@access-token')}`
      }
    });
  }

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

  private hideLoader(): void {
    if (this.activeRequests > 0) {
      this.activeRequests--;
    }
    if (this.activeRequests === 0 && this.isModalOpen$.value) {
      this.isModalOpen$.next(false);
      if (this.loadingModalRef) {
        this.loadingModalRef.close(); 
        this.loadingModalRef = null;
      }
    }
  }
}