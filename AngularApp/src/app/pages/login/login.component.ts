import { Component, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { map, catchError, of } from "rxjs";
import { AlertComponent, AlertType } from "../../components";
import { ILogin, IAuth } from "../../models";
import { AuthService, AcessoService, AuthGoogleService } from "../../services";
import { isNativeMobile } from "../../utils/platform.utils";
import { Platform } from '@ionic/angular';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: false
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  showPassword = false;
  eyeIconClass: string = 'bi-eye';

  constructor(
    private platform: Platform,
    private formbuilder: FormBuilder,
    public router: Router,
    public acessoService: AcessoService,
    public authProviderService: AuthService,
    public authProviderGoogleService: AuthGoogleService,
    public modalALert: AlertComponent) {
    this.platform.ready().then(() => {
      if (isNativeMobile()) {
        const elements = document.querySelectorAll('.g_signin');
        elements.forEach(el => el.remove());
      }
    });
  }

  public ngOnInit(): void {
    this.loginForm = this.formbuilder.group({
      email: ["user@example.com", [Validators.required, Validators.email]],
      senha: ["12345T!", [Validators.required, Validators.nullValidator]]
    }) as FormGroup;
  }

  public onLoginClick(): void {
    let login: ILogin = this.loginForm.getRawValue();

    this.acessoService.signIn(login).pipe(
      map((response: IAuth) => {
        if (response.authenticated) {
          return { success: true, response: response };
        } else {
          return { success: false, error: response };
        }
      }),
      catchError((error) => {
        const errorMsg = (error && typeof error.error === 'string')
          ? error.error
          : 'Erro inesperado, tente novamente';
        return of({ success: false, error: errorMsg });
      })
    ).subscribe((result: { success: boolean, response?: IAuth, error?: any }) => {
      if (result.success) {
        this.authProviderService.login(result.response!);
        this.router.navigate(['/dashboard']);
      } else {
        this.modalALert.open(AlertComponent, result.error, AlertType.Warning);
      }
    });
  }

  public onGoogleLoginClick(): void {
    this.authProviderGoogleService.handleGoogleLogin().subscribe({
      next: (auth: IAuth) => {
        if (auth.authenticated)
          this.router.navigate(['/dashboard']);
      },
      error: (errorMessage: string) => {
        this.modalALert.open(AlertComponent, errorMessage, AlertType.Warning);
      }
    });
  }

  public onTooglePassword(): void {
    this.showPassword = !this.showPassword;
    this.eyeIconClass = (this.eyeIconClass === 'bi-eye') ? 'bi-eye-slash' : 'bi-eye';
  }
}
