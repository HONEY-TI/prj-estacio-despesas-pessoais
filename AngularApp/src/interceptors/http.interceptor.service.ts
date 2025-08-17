import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { BehaviorSubject, Observable, catchError, filter, finalize, map, switchMap, take, throwError } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService, TokenStorageService } from '../app/shared/services';
import { IAuth } from '../app/shared/models';
import { LoadingComponent } from '../app/shared/components';

@Injectable({
  providedIn: 'root'
})

export class CustomInterceptor implements HttpInterceptor {
  private activeRequests: number = 0;
  private isModalOpen$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private tokenService: TokenStorageService, private authService: AuthService, private modalService: NgbModal) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.showLoader();
    const modifiedRequest = this.addTokenHeader(request);

    return next.handle(modifiedRequest).pipe(
      map((response: HttpResponse<any>) => {
        if (response.status === 200) {
          this.hideLoader();
          return response;
        }
        return response;
      }),
      catchError((error: HttpErrorResponse) => {
        this.hideLoader();
        if (error.ok === false && error.status === 0) {
          return throwError(() => 'Erro de conexão tente mais tarde.');
        }
        else if (error.status === 400) {
          return throwError(() => error.error);
        }
        else if (error.status === 401) {
          return this.handleError(request, next);
        }
        else if (error.status === 403) {
          return throwError(() => 'Acesso não Autorizado!');
        }
        else if (error.status === 404) {
          return this.handleError(request, next);
        }
        //console.log(error);
        return throwError(() => 'Erro tente atualizar a página ou realize novamente o login..');
      }));
  }

  private handleError(request: HttpRequest<any>, next: HttpHandler) {
    this.refreshTokenSubject.next(null);

    const auth = this.tokenService.getRefreshToken();
    if (auth)
      return this.authService.refreshToken(auth).pipe(
        switchMap((auth: IAuth) => {
          this.tokenService.saveUser(auth);
          this.tokenService.saveToken(auth.accessToken);
          this.tokenService.saveRefreshToken(auth.refreshToken);
          this.refreshTokenSubject.next(auth.refreshToken);
          this.hideLoader();
          return next.handle(this.addTokenHeader(request));

        }),
        catchError((err) => {
          sessionStorage.clear();
          this.tokenService.signOut();
          return throwError(() => 'Erro de autenticação, tente atualizar a página ou realize novamente o login.');

        })
      );
    return this.refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request)))
    );
  }

  private addTokenHeader(request: HttpRequest<any>) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${this.tokenService.getToken()}`,
        'Content-Type': 'application/json'
      }
    });
  }

  private showLoader(): void {
    this.activeRequests++;
    if (this.activeRequests === 0) {
      this.isModalOpen$.next(true);
      const modalRef = this.modalService.open(LoadingComponent, {
        centered: true,
        fullscreen: true,
        windowClass: 'loading-modal',
      });
      modalRef.result.then(
        () => { },
        () => {
          if (!this.isModalOpen$.value) {
            this.activeRequests = 0;
          }
        }
      );
    }
  }

  private hideLoader(): void {
    if (this.activeRequests >= 0) {
      this.activeRequests--;
    }
    else {
      this.isModalOpen$.next(false);
      this.modalService.dismissAll();
      this.activeRequests = 0;

    }
  }
}
