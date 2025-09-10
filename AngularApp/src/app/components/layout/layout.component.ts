import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { AuthService, MenuService,  ImagemPerfilService } from "../../services";
@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss'],
  standalone: false
})

export class LayoutComponent implements OnInit {
  urlPerfilImage: string = '../../../../assets/perfil_static.png';
  constructor(
    private authService: AuthService,
    private router: Router,
    public menuService: MenuService,
    private imagemPerfilService: ImagemPerfilService) { }

  public initialize = (): void => {
    this.imagemPerfilService.getImagemPerfilUsuario()
      .subscribe({
        next: (response: ArrayBuffer) => {
          if (!response || response.byteLength === 0) {
            this.urlPerfilImage = '../../../../assets/perfil_static.png'; // usa padrão
          } else {
            const blob = new Blob([response], { type: 'image/png' });
            this.urlPerfilImage = URL.createObjectURL(blob);
          }
        },
        error: () => {
          this.urlPerfilImage = '../../../../assets/perfil_static.png';
        }
      });
  }
  
  public selectMenu(menu: number) {
    this.menuService.selectMenu(menu, this.router);
  }

  public onLogoutClick() {
    this.authService.tokenStorage.signOut();
    this.router.navigate(['/']);
  }

  public ngOnInit(): void {
    this.initialize();
  }
}
