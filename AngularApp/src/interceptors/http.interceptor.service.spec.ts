import { TestBed, fakeAsync, flush } from '@angular/core/testing';
import { TokenStorageService, AuthService, CustomInterceptor } from '../app/services';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { IAuth } from '../app/models';
import { LoadingComponent } from '../app/components';

describe('CustomInterceptor', () => {
  let interceptor: CustomInterceptor;
  let tokenService: jasmine.SpyObj<TokenStorageService>;
  let authService: jasmine.SpyObj<AuthService>;
  let modalService: jasmine.SpyObj<NgbModal>;
  let handler: jasmine.SpyObj<HttpHandler>;
  let modalCloseSpy: jasmine.Spy;

  beforeEach(() => {
    const tokenSpy = jasmine.createSpyObj('TokenStorageService', [
      'getAccessToken', 'getRefreshToken', 'updateAccessToken', 'saveRefreshToken', 'revokeRefreshToken', 'signOut'
    ]);
    const authSpy = jasmine.createSpyObj('AuthService', ['refreshToken']);
    const modalSpy = jasmine.createSpyObj('NgbModal', ['open']);
    const handlerSpy = jasmine.createSpyObj('HttpHandler', ['handle']);

    TestBed.configureTestingModule({
      providers: [
        CustomInterceptor,
        { provide: TokenStorageService, useValue: tokenSpy },
        { provide: AuthService, useValue: authSpy },
        { provide: NgbModal, useValue: modalSpy },
      ]
    });

    interceptor = TestBed.inject(CustomInterceptor);
    tokenService = TestBed.inject(TokenStorageService) as jasmine.SpyObj<TokenStorageService>;
    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    modalService = TestBed.inject(NgbModal) as jasmine.SpyObj<NgbModal>;
    handler = handlerSpy;

    // Criar spy do modal.close
    modalCloseSpy = jasmine.createSpy('close');
    const modalRefMock: Partial<NgbModalRef> = { close: modalCloseSpy };
    modalService.open.and.returnValue(modalRefMock as NgbModalRef);

  });

  it('should show loader and hide loader after request completes', fakeAsync(() => {
    const request = new HttpRequest('GET', '/test');
    handler.handle.and.returnValue(of({} as HttpEvent<any>));

    interceptor.intercept(request, handler).subscribe();
    flush();

    expect(handler.handle).toHaveBeenCalled();
    expect(modalService.open).toHaveBeenCalledWith(LoadingComponent, jasmine.any(Object));
    expect(modalCloseSpy).toHaveBeenCalled(); // garante que o modal foi fechado
  }));

  it('should clone request and add Authorization header', fakeAsync(() => {
    const request = new HttpRequest('GET', '/test');
    tokenService.getAccessToken.and.returnValue('abc123');
    handler.handle.and.returnValue(of({} as HttpEvent<any>));

    interceptor.intercept(request, handler).subscribe();
    flush();

    const clonedReq = handler.handle.calls.mostRecent().args[0] as HttpRequest<any>;
    expect(clonedReq.headers.get('Authorization')).toBe('Bearer abc123');
  }));

  it('should handle 401 error and refresh token successfully', fakeAsync(() => {
    const request = new HttpRequest('GET', '/test');
    tokenService.getRefreshToken.and.returnValue('refreshToken');
    const refreshResponse: IAuth = { authenticated: true, accessToken: 'newToken', refreshToken: 'newRefreshToken', created: '', expiration: '' };
    authService.refreshToken.and.returnValue(of(refreshResponse));
    tokenService.getAccessToken.and.returnValue('oldToken');

    // First call returns 401
    handler.handle.and.returnValues(
      throwError(() => new HttpErrorResponse({ status: 401, url: '/test' })),
      of({} as HttpEvent<any>) // retried request
    );

    interceptor.intercept(request, handler).subscribe();
    flush();

    expect(authService.refreshToken).toHaveBeenCalledWith('refreshToken');
    expect(tokenService.updateAccessToken).toHaveBeenCalledWith('newToken');
    expect(tokenService.saveRefreshToken).toHaveBeenCalledWith('newRefreshToken');
    expect(modalCloseSpy).toHaveBeenCalled();
  }));

  it('should handle 401 error and no refresh token', fakeAsync(() => {
    const request = new HttpRequest('GET', '/test');
    tokenService.getRefreshToken.and.returnValue(null);
    handler.handle.and.returnValue(throwError(() => new HttpErrorResponse({ status: 401, url: '/test' })));

    let error: any;
    interceptor.intercept(request, handler).subscribe({ error: e => error = e });
    flush();

    expect(error).toBe('Sessão expirada, faça login novamente.');
    expect(tokenService.signOut).toHaveBeenCalled();
    expect(tokenService.revokeRefreshToken).toHaveBeenCalled();
    expect(modalCloseSpy).toHaveBeenCalled();
  }));

  it('should handle standard errors correctly', fakeAsync(() => {
    const request = new HttpRequest('GET', '/test');
    handler.handle.and.returnValue(throwError(() => new HttpErrorResponse({ status: 400, error: 'Bad request' })));

    let error: any;
    interceptor.intercept(request, handler).subscribe({ error: e => error = e });
    flush();

    expect(error).toBe('Bad request');
    expect(modalCloseSpy).toHaveBeenCalled();
  }));

  it('should call signOut on 404 error', fakeAsync(() => {
    const request = new HttpRequest('GET', '/test');
    handler.handle.and.returnValue(throwError(() => new HttpErrorResponse({ status: 404 })));

    let error: any;
    interceptor.intercept(request, handler).subscribe({ error: e => error = e });
    flush();

    expect(error).toBe('Recurso não encontrado.');
    expect(tokenService.signOut).toHaveBeenCalled();
    expect(modalCloseSpy).toHaveBeenCalled();
  }));

  it('should log 500 errors and throw error', fakeAsync(() => {
    spyOn(console, 'log'); // espionando log

    const request = new HttpRequest('GET', '/test');
    handler.handle.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 500, error: 'Internal' }))
    );

    let error: any;
    interceptor.intercept(request, handler).subscribe({ error: e => error = e });
    flush();

    expect(console.log).toHaveBeenCalledWith(jasmine.any(String));
    expect(error).toBe('Internal');
    expect(modalCloseSpy).toHaveBeenCalled();
  }));

});
