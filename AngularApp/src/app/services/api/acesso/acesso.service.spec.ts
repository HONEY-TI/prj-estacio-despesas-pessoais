import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AcessoService, CustomInterceptor } from '../..';
import { environment } from '../../../../environments/environment';
import { IAcesso, ILogin } from '../../../models';

describe('Unit Test AcessoService', () => {

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers:[AcessoService,
        { provide: HTTP_INTERCEPTORS, useClass: CustomInterceptor, multi: true, }
      ]
    });
  });

  it('should be created', inject([AcessoService], (service: AcessoService) => {
    expect(service).toBeTruthy();
  }));

  it('should send a POST request to the Acesso/SignIn endpoint', inject(
    [AcessoService, HttpTestingController],
    (service: AcessoService, httpMock: HttpTestingController) => {
      const loginData: ILogin = {
        email: 'user@example.com',
        senha: 'teste',
      };

      const mockResponse = { message: true };
      service.signIn(loginData).subscribe((response: any) => {
        expect(response).toBeTruthy();
      });
      const expectedUrl = `${environment.BASE_URL}/Acesso/SignIn`;
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
      httpMock.verify();
    }
  ));

  it('should send a POST request to the /Acesso endpoint', inject(
    [AcessoService, HttpTestingController],
    (service: AcessoService, httpMock: HttpTestingController) => {
      const AcessoData: IAcesso = {
        nome: 'Teste ',
        sobreNome: 'Usuario',
        telefone: '(21) 9999-9999',
        email: 'user@example.com',
        senha: '12345',
        confirmaSenha: '12345'
      };

      const mockResponse = { message: true };
      service.createUsuario(AcessoData).subscribe((response: any) => {
        expect(response).toBeTruthy();
      });
      const expectedUrl = `${environment.BASE_URL}/Acesso`;
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
      httpMock.verify();
    }
  ));

  it('should send a POST request to the /Acesso/ChangePassword endpoint', inject(
    [AcessoService, HttpTestingController],
    (service: AcessoService, httpMock: HttpTestingController) => {
      const login: ILogin = {
        senha: '12345',
        confirmaSenha: '12345'
      };

      const mockResponse = { message: true };
      service.changePassword(login).subscribe((response: any) => {
        expect(response).toBeTruthy();
      });

      const expectedUrl = `${environment.BASE_URL}/Acesso/ChangePassword`;
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('POST');
      req.flush(mockResponse);
      httpMock.verify();
    }
  ));
});