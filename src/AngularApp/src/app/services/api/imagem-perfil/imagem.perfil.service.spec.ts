import { TestBed, inject } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CustomInterceptor, ImagemPerfilService } from '../..';
import { environment } from '../../../../environments/environment';

describe('ImagemPerfilService Unit Test', () => {
  let file: File;
  let service: ImagemPerfilService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    file = new File(['Mock File Test'], 'image.png', { type: 'image/jpg' });

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ImagemPerfilService,
        { provide: HTTP_INTERCEPTORS, useClass: CustomInterceptor, multi: true }
      ]
    });

    service = TestBed.inject(ImagemPerfilService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should send a GET request to getImagemPerfilUsuario', () => {
    const mockResponse = new ArrayBuffer(8);

    service.getImagemPerfilUsuario().subscribe((response: ArrayBuffer) => {
      expect(response).toBeTruthy();
      expect(response.byteLength).toBe(mockResponse.byteLength);
    });

    const expectedUrl = `${environment.BASE_URL}/Usuario/GetProfileImage`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('GET');

    req.flush(mockResponse);
  });

  it('should send a PUT request to updateImagemPerfilUsuario', () => {
    const mockResponse = new ArrayBuffer(16);

    service.updateImagemPerfilUsuario(file).subscribe((response: ArrayBuffer) => {
      expect(response).toBeTruthy();
      expect(response.byteLength).toBe(mockResponse.byteLength);
    });

    const expectedUrl = `${environment.BASE_URL}/Usuario/UpdateProfileImage`;
    const req = httpMock.expectOne(expectedUrl);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body.has('file')).toBeTrue();

    req.flush(mockResponse);
  });

  it('should return cached image if already fetched', () => {
    const cached = new ArrayBuffer(4);
    // Forçar cache
    (service as any).cachedImage = cached;

    service.getImagemPerfilUsuario().subscribe((response: ArrayBuffer) => {
      expect(response).toBe(cached);
    });

    httpMock.expectNone(`${environment.BASE_URL}/Usuario/GetProfileImage`);
  });
});
