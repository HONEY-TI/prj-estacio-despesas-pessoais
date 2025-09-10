import { ComponentFixture, TestBed, fakeAsync, flush } from '@angular/core/testing';
import { ChangeAvatarComponent } from './change-avatar.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { from, of, throwError } from 'rxjs';
import { AlertComponent, AlertType } from '../../../components';
import { ImagemPerfilService, UsuarioService } from '../../../services';
import { MockImagemPerfil } from '../../../../__mock__';

describe('Unit Test ChangeAvatarComponent', () => {
  let component: ChangeAvatarComponent;
  let fixture: ComponentFixture<ChangeAvatarComponent>;
  let mockImagemPerfilService: MockImagemPerfil;
  let mockImagemPerfilUsuario: any;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ChangeAvatarComponent],
      imports: [CommonModule, HttpClientTestingModule, ReactiveFormsModule],
      providers: [FormBuilder, AlertComponent, UsuarioService, NgbActiveModal]
    });
    fixture = TestBed.createComponent(ChangeAvatarComponent);
    component = fixture.componentInstance;
    mockImagemPerfilService = TestBed.inject(ImagemPerfilService) as unknown as MockImagemPerfil;
    mockImagemPerfilUsuario = new MockImagemPerfil().getImagemPerfilUsuario();
    fixture.detectChanges();
  });

  it('should create Change Avatar Component', () => {
    // Assert
    expect(component).toBeTruthy();
  });

  it('should initialize Change Avatar Component', fakeAsync(() => {
    // Arrange
    const mockArrayBuffer = new ArrayBuffer(8);
    const spyOnGetImagemPerfilUsuario = spyOn(mockImagemPerfilService, 'getImagemPerfilUsuario').and.returnValue(of(mockArrayBuffer));

    // Act
    component.initialize();
    flush();

    // Assert
    expect(spyOnGetImagemPerfilUsuario).toHaveBeenCalled();
    expect(component.urlPerfilImage).not.toBeNull();
    expect(component.urlPerfilImage).not.toEqual('../../../../assets/perfil_static.png');
    expect(component.urlPerfilImage.startsWith('blob:')).toBeTrue();
  }));

  it('should initialize Change Avatar Component adn Thnrows Error ', fakeAsync(() => {
    // Arrange
    const errorMessage = 'Fake Error Message Initialize Chart';
    const spyOnGetImagemPerfilUsuario = spyOn(mockImagemPerfilService, 'getImagemPerfilUsuario').and.returnValue(throwError(errorMessage));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act
    component.initialize();

    // Assert
    expect(spyOnGetImagemPerfilUsuario).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, errorMessage, AlertType.Warning);
  }));

  it('should handle avatar upload', () => {
    // Arrange
    const mockFile = new File(['mock file'], 'avatar.jpg', { type: 'image/jpeg' });
    const event = { target: { files: [mockFile] } };

    // Act
    component.handleAvatarUpload(event);

    // Assert
    expect(component.file).toEqual(mockFile);
    expect(component.urlPerfilImage).not.toBeNull();
    expect(component.fileLoaded).toBeTruthy();
  });

  it('should handle and create imagem Perfil successfully', fakeAsync(() => {
    // Arrange
    let mockImagemPerfilUsuario: any = new MockImagemPerfil().getImagemPerfilUsuario();
    let mockFile = new File(['mock file'], 'avatar.jpg', { type: 'image/jpeg' });
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'updateImagemPerfilUsuario').and.returnValue(of(mockImagemPerfilUsuario));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act    
    component.file = mockFile;
    component.handleImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).toHaveBeenCalled();
    expect(spyOnImagemPerfilService).toHaveBeenCalledWith(mockFile);
    expect(component.file).toBeNull();
    expect(component.fileLoaded).toBeFalsy();
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, 'Imagem adicionada com sucesso!', AlertType.Success);
  }));

  it('should try to create imagem Perfil without upload image', fakeAsync(() => {
    // Arrange
    let errorMessage = 'Fake Error Message nto uplaod image';
    let mockFile = null;
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'updateImagemPerfilUsuario').and.returnValue(throwError(errorMessage));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act
    component.file = mockFile;
    component.handleImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).not.toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, 'É preciso carregar uma nova imagem!', AlertType.Warning);
  }));

  it('should handle and try create imagem Perfil and throws error', fakeAsync(() => {
    // Arrange
    let errorMessage = 'Fake Error Message ';
    let mockFile = new File(['mock file'], 'avatar.jpg', { type: 'image/jpeg' });
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'updateImagemPerfilUsuario').and.returnValue(throwError(errorMessage));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act
    component.file = mockFile;
    component.handleImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).toHaveBeenCalled();
    expect(spyOnImagemPerfilService).toHaveBeenCalledWith(mockFile);
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, errorMessage, AlertType.Warning);
  }));

  it('should handle and update imagem Perfil successfully', fakeAsync(() => {
    // Arrange
    const mockFile = new File(['mock file content'], 'avatar.jpg', { type: 'image/jpeg' });
    const mockResponse = new ArrayBuffer(8); 
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'updateImagemPerfilUsuario').and.returnValue(of(mockResponse));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act    
    component.file = mockFile;
    component.handleImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).toHaveBeenCalled();
    expect(spyOnImagemPerfilService).toHaveBeenCalledWith(mockFile);
    expect(component.file).toBeNull();
    expect(component.fileLoaded).toBeFalsy();
    expect(component.urlPerfilImage).not.toEqual('../../../../assets/perfil_static.png');
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, 'Imagem adicionada com sucesso!', AlertType.Success);
  }));

  it('should handle and try update imagem Perfil and throws error', fakeAsync(() => {
    // Arrange
    let errorMessage = 'Fake Error Message updateImagemPerfilUsuario';
    let mockFile = new File(['mock file'], 'avatar.jpg', { type: 'image/jpeg' });
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'updateImagemPerfilUsuario').and.returnValue(throwError(errorMessage));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act
    component.file = mockFile;
    component.handleImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).toHaveBeenCalled();
    expect(spyOnImagemPerfilService).toHaveBeenCalledWith(mockFile);
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, errorMessage, AlertType.Warning);
  }));

  xit('should handle and delete imagem Perfil successfully', fakeAsync(() => {
    // Arrange
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'deleteImagemPerfilUsuario').and.returnValue(from(Promise.resolve(true)));
    const alertOpenSpy = spyOn(component.modalAlert, 'open').and.callThrough();

    // Act
    component.handleDeleteImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, 'Imagem de perfil usuário excluída com sucesso!', AlertType.Success);
    expect(component.file).toBeNull();
    expect(component.fileLoaded).toBeFalsy();

  }));

  xit('should handle and try delete imagem Perfil and throws error', fakeAsync(() => {
    // Arrange
    let errorMessage = 'Fake Error Message deleteImagemPerfilUsuario';
    const spyOnImagemPerfilService = spyOn(mockImagemPerfilService, 'deleteImagemPerfilUsuario').and.returnValue(throwError(errorMessage));
    const alertOpenSpy = spyOn(TestBed.inject(AlertComponent), 'open');

    // Act
    component.handleDeleteImagePerfil();
    flush();

    // Assert
    expect(spyOnImagemPerfilService).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalled();
    expect(alertOpenSpy).toHaveBeenCalledWith(AlertComponent, errorMessage, AlertType.Warning);
  }));
});