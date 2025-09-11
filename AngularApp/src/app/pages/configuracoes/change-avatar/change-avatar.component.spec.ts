import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChangeAvatarComponent } from './change-avatar.component';
import { ImagemPerfilService } from '../../../services';
import { AlertComponent, AlertType } from '../../../components';
import { of, throwError } from 'rxjs';

describe('ChangeAvatarComponent', () => {
  let component: ChangeAvatarComponent;
  let fixture: ComponentFixture<ChangeAvatarComponent>;
  let imagemPerfilServiceSpy: jasmine.SpyObj<ImagemPerfilService>;
  let alertSpy: jasmine.SpyObj<AlertComponent>;

  beforeEach(async () => {
    imagemPerfilServiceSpy = jasmine.createSpyObj('ImagemPerfilService', [
      'getImagemPerfilUsuario',
      'updateImagemPerfilUsuario'
    ]);
    alertSpy = jasmine.createSpyObj('AlertComponent', ['open']);
    imagemPerfilServiceSpy.getImagemPerfilUsuario.and.returnValue(of(null));

    await TestBed.configureTestingModule({
      declarations: [ChangeAvatarComponent],
      providers: [
        { provide: ImagemPerfilService, useValue: imagemPerfilServiceSpy },
        { provide: AlertComponent, useValue: alertSpy }
      ]
    });

    fixture = TestBed.createComponent(ChangeAvatarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should initialize with default image when service returns null', () => {
    // Arrange
    imagemPerfilServiceSpy.getImagemPerfilUsuario.and.returnValue(of(null as any));

    // Act
    component.initialize();

    // Assert
    expect(component.urlPerfilImage).toBe('../../../../assets/perfil_static.png');
  });

  it('should update urlPerfilImage when service returns an image', () => {
    // Arrange
    const buffer = new ArrayBuffer(8);
    imagemPerfilServiceSpy.getImagemPerfilUsuario.and.returnValue(of(buffer));

    // Act
    component.initialize();

    // Assert
    expect(component.urlPerfilImage).toContain('blob:');
  });

  it('should call AlertComponent when service throws in initialize', () => {
    // Arrange
    imagemPerfilServiceSpy.getImagemPerfilUsuario.and.returnValue(throwError(() => 'service error'));

    // Act
    component.initialize();

    // Assert
    expect(alertSpy.open).toHaveBeenCalledWith(
      AlertComponent,
      'service error',
      AlertType.Warning
    );
  });

  it('should load file in handleAvatarUpload', () => {
    // Arrange
    const file = new File(['content'], 'avatar.png', { type: 'image/png' });
    const event = { target: { files: [file] } };

    // Act
    component.handleAvatarUpload(event);

    // Assert
    expect(component.file).toBe(file);
    expect(component.fileLoaded).toBeTrue();
    expect(component.urlPerfilImage).toContain('blob:');
  });

  it('should save new profile image in handleImagePerfil (update)', () => {
    // Arrange
    const file = new File(['content'], 'avatar.png', { type: 'image/png' });
    const buffer = new ArrayBuffer(8);
    component.file = file;
    component.urlPerfilImage = 'someImage.png';
    imagemPerfilServiceSpy.updateImagemPerfilUsuario.and.returnValue(of(buffer));

    // Act
    component.handleImagePerfil();

    // Assert
    expect(alertSpy.open).toHaveBeenCalledWith(
      AlertComponent,
      'Imagem de perfil usuário alterada com sucesso!',
      AlertType.Success
    );
    expect(component.file).toBeNull();
    expect(component.fileLoaded).toBeFalse();
  });

  it('should warn when no file is selected in handleImagePerfil', () => {
    // Arrange
    component.file = null;

    // Act
    component.handleImagePerfil();

    // Assert
    expect(alertSpy.open).toHaveBeenCalledWith(
      AlertComponent,
      'É preciso carregar uma nova imagem!',
      AlertType.Warning
    );
  });

  it('should handle error in handleImagePerfil', () => {
    // Arrange
    const file = new File(['content'], 'avatar.png', { type: 'image/png' });
    component.file = file;
    component.urlPerfilImage = '../../../../assets/perfil_static.png';
    imagemPerfilServiceSpy.updateImagemPerfilUsuario.and.returnValue(
      throwError(() => 'upload error')
    );

    // Act
    component.handleImagePerfil();

    // Assert
    expect(alertSpy.open).toHaveBeenCalledWith(
      AlertComponent,
      'upload error',
      AlertType.Warning
    );
  });
});
