import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { AlertComponent, AlertType } from '../../../components';
import { ImagemPerfilService } from '../../../services';

@Component({
  selector: 'app-change-avatar',
  templateUrl: './change-avatar.component.html',
  styleUrls: ['./change-avatar.component.scss'],
  standalone: false
})

export class ChangeAvatarComponent implements OnInit {
  @Input() handleAvatarUploaded: (event: Event) => void;
  formAvatar: FormGroup;
  file: File | null = null;
  fileLoaded = false;
  public urlPerfilImage: string = '../../../../assets/perfil_static.png' ;

  constructor(
    public imagemPerfilService: ImagemPerfilService,
    public modalAlert: AlertComponent
  ) {
    this.formAvatar = new FormGroup({
      uploadPhoto: new FormControl(null),
    });
  }

  ngOnInit(): void {
    this.initialize();
  }

  initialize = (): void => {
    this.imagemPerfilService.getImagemPerfilUsuario()
      .subscribe({
        next: (response: ArrayBuffer) => {
          if (response && response !== undefined && response !== null) {
          const blob = new Blob([response], { type: 'image/png' });
          // Cria uma URL para o Blob
          this.urlPerfilImage = URL.createObjectURL(blob);
        } else {
          this.urlPerfilImage = '../../../../assets/perfil_static.png';
        }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  handleAvatarUpload = (event: any): void => {
    const uploadedFile = event.target.files?.[0];
    if (uploadedFile) {
      this.file = uploadedFile;
      this.urlPerfilImage = URL.createObjectURL(uploadedFile);
      this.fileLoaded = true;
    }
  }

  handleImagePerfil = (): void => {
    if (this.file !== null) {
      if (this.urlPerfilImage === undefined || this.urlPerfilImage === null || this.urlPerfilImage === '../../../../assets/perfil_static.png')  {
        this.imagemPerfilService.updateImagemPerfilUsuario(this.file)
          .subscribe({
            next: (response) => {
              if (response) {
                this.file = null;
                this.fileLoaded = false;
                const blob = new Blob([response], { type: 'image/png' });
                this.urlPerfilImage = URL.createObjectURL(blob);
                this.modalAlert.open(AlertComponent, 'Imagem adicionada com sucesso!', AlertType.Success);
              }
            },
            error: (errorMessage: string) => {
              this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
            }
          });
      }
      else {
        this.imagemPerfilService.updateImagemPerfilUsuario(this.file)
          .subscribe({
            next: (response) => {
              if (response) {
                this.file = null;
                this.fileLoaded = false;
                const blob = new Blob([response], { type: 'image/png' });
                this.urlPerfilImage = URL.createObjectURL(blob);
                this.modalAlert.open(AlertComponent, 'Imagem de perfil usuário alterada com sucesso!', AlertType.Success);
              }
            },
            error: (errorMessage: string) => {
              this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
            }
          });
      }
    }
    else {
      this.modalAlert.open(AlertComponent, 'É preciso carregar uma nova imagem!', AlertType.Warning);
    }
  }

  handleDeleteImagePerfil = (): void => {
    fetch('../../../../assets/perfil_static.png')
      .then(res => res.blob())
      .then(blob => {
        const file = new File([blob], 'perfil_static.png', { type: 'image/png' });

        this.imagemPerfilService.updateImagemPerfilUsuario(file)
          .subscribe({
            next: (response: ArrayBuffer) => {
              this.file = null;
              this.fileLoaded = false;
              const blobResponse = new Blob([response], { type: 'image/png' });
              this.urlPerfilImage = URL.createObjectURL(blobResponse);
              this.modalAlert.open(AlertComponent, 'Imagem de perfil resetada para padrão!', AlertType.Success);
            },
            error: (errorMessage: string) => {
              this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
            }
          });
      });
  }
}
