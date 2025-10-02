import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from "@angular/common/http/testing";
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import  dayjs from "dayjs";
import { CustomInterceptor, SaldoService } from '../..';
import { environment } from '../../../../environments/environment';

describe('SaldoService Unit Test', () => {

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [SaldoService,
        { provide: HTTP_INTERCEPTORS, useClass: CustomInterceptor, multi: true }
      ]
    });
  });

  it('should be created', inject([SaldoService], (service: SaldoService) => {
    expect(service).toBeTruthy();
  }));

  it('should send a getSaldo request to the Saldo endpoint', inject(
    [SaldoService, HttpTestingController],
    (service: SaldoService, httpMock: HttpTestingController) => {

      const mockResponse: number = 989.9;

      service.getSaldo().subscribe((response: any) => {
        expect(response).toBe(mockResponse);
      });

      const expectedUrl = `${environment.BASE_URL}/Saldo`;
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('GET');

      req.flush(mockResponse);
      httpMock.verify();
    }
  ));

  it('should send a getSaldoAnual request to the Saldo endpoint', inject(
    [SaldoService, HttpTestingController],
    (service: SaldoService, httpMock: HttpTestingController) => {

      const mockResponse: number = 2500.25;
      const mockAno = dayjs().format('YYYY-MM');

      service.getSaldoAnual(dayjs()).subscribe((response: any) => {
        expect(response).toBe(mockResponse);
      });

      const expectedUrl = `${environment.BASE_URL}/Saldo/ByAno/${mockAno}`;
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('GET');

      req.flush(mockResponse);
      httpMock.verify();
    }
  ));

  it('should send a getSaldoByMesAno request to the Saldo endpoint', inject(
    [SaldoService, HttpTestingController],
    (service: SaldoService, httpMock: HttpTestingController) => {

      const mockResponse: number = 84980.09;
      const mockMesAno = dayjs().format('YYYY-MM');

      service.getSaldoByMesANo(dayjs()).subscribe((response: any) => {
        expect(response).toBe(mockResponse);
      });

      const expectedUrl = `${environment.BASE_URL}/Saldo/ByMesAno/${mockMesAno}`;
      const req = httpMock.expectOne(expectedUrl);
      expect(req.request.method).toBe('GET');

      req.flush(mockResponse);
      httpMock.verify();
    }
  ));
});
