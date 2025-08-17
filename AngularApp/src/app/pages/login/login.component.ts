import { Component, ElementRef, OnInit, Renderer2 } from "@angular/core";
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { map, catchError } from "rxjs";
import { AlertComponent, AlertType } from "../../shared/components";
import { ILogin, IAuth } from "../../shared/models";
import { AuthService } from "../../shared/services";
import { AcessoService } from "../../shared/services/api";
import { AuthGoogleService } from "../../shared/services/auth/auth.google.service";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: false
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup & ILogin;
  showPassword = false;
  eyeIconClass: string = 'bi-eye';

  constructor(
    private renderer: Renderer2,
    private el: ElementRef,
    private formbuilder: FormBuilder,
    public router: Router,
    public acessoService: AcessoService,
    public authProviderService: AuthService,
    public authProviderGoogleService: AuthGoogleService,
    public modalALert: AlertComponent) { }

  ngOnInit(): void {
    this.loginForm = this.formbuilder.group({
      email: ['teste@teste.com', [Validators.required, Validators.email]],
      senha: ['12345T!', [Validators.required, Validators.nullValidator]]
    }) as FormGroup & ILogin;
    this.renderer.setAttribute(this.el.nativeElement, 'aria-hidden', 'false');
  }

  onLoginClick() {

    let login: ILogin = this.loginForm.getRawValue();

    this.acessoService.signIn(login).pipe(
      map((response: IAuth) => {
        if (response.authenticated) {
          return this.authProviderService.createAccessToken(response);
        }
        else {
          throw (response);
        }
      }),
      catchError((error) => {
        if (error && typeof error.message === 'string') {
          throw (error.message);
        }
        throw (error);
      })
    )
      .subscribe({
        next: (response: boolean) => {
          if (response)
            this.router.navigate(['/dashboard']).then(success => {
              if (!success) {
                console.error('Falha ao navegar para /dashboard');
              }
            });
        },
        error: (errorMessage: string) => {
          this.modalALert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  onGoogleLoginClick() {
    this.authProviderGoogleService.handleGoogleLogin().pipe(
      map((response: IAuth) => {
        if (response.authenticated) {
          return this.authProviderService.createAccessToken(response);
        } else {
          throw response;
        }
      }),
      catchError((error) => {
        if (error && typeof error.message === 'string') {
          throw error.message;
        }
        throw error;
      })
    ).subscribe({
      next: (response: boolean) => {
        if (response)
          this.router.navigate(['/dashboard']);
      },
      error: (errorMessage: string) => {
        this.modalALert.open(AlertComponent, errorMessage, AlertType.Warning);
      }
    });
  }

  onTooglePassword() {
    this.showPassword = !this.showPassword;
    this.eyeIconClass = (this.eyeIconClass === 'bi-eye') ? 'bi-eye-slash' : 'bi-eye';
  }

}
