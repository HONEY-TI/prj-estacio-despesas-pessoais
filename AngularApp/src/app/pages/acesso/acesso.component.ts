import { Component, OnInit } from "@angular/core";
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, FormsModule } from "@angular/forms";
import { Router } from "@angular/router";
import { map, catchError } from "rxjs";
import { AlertComponent, AlertType } from "../../components";
import { IAcesso } from "../../models";
import { AcessoService } from "../../services/api";
import { CustomValidators } from "../validators";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { FooterComponent } from "../../components/footer/footer.component";

@Component({
  selector: 'app-acesso',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,  FormsModule, MatIconModule, MatFormFieldModule, MatInputModule, FooterComponent ],
  templateUrl: './acesso.component.html',
  styleUrls: ['./acesso.component.scss'],

})

export class AcessoComponent  implements OnInit {
  createAccountFrom : FormGroup & IAcesso;
  eyeIconClass: string = 'bi-eye';
  eyeIconClassConfirmaSenha: string = 'bi-eye';
  showSenha = false;
  showConfirmaSenha = false;

  constructor(
    public formbuilder: FormBuilder,
    public router: Router,
    public acessoService: AcessoService,
    public modalALert: AlertComponent) {}

  public ngOnInit(): void{
    this.createAccountFrom = this.formbuilder.group({
      email: ['', [Validators.required, Validators.email]],
      nome: ['', [Validators.required]],
      sobreNome: '',
      telefone: ['', [Validators.required]],
      senha: ['', [Validators.required]],
      confirmaSenha: ['', [Validators.required]]
    }, {
      validator: CustomValidators.isValidPassword
    })as FormGroup & IAcesso;
  }

  public onSaveClick() {
    let acesso: IAcesso = this.createAccountFrom.getRawValue();
    this.acessoService.createUsuario(acesso).pipe(
      map((response: boolean) => {
        if (response) {
          return response;
        } else {
          throw Error("Erro ao realizar cadastro.");
        }
      }),
      catchError((error) => {
        if (error.status === 400) {
          const validationErrors = error.errors;
          if (validationErrors) {
            Object.keys(validationErrors).forEach(field => {
              throw validationErrors[field][0];
            });
          }
        }
        throw (error);
      })
    )
    .subscribe({
      next: (result: boolean) => {
        if (result){
          this.modalALert.open(AlertComponent, "Cadastro realizado com sucesso!", AlertType.Success);
          this.router.navigate(['/']);
        }
      },
      error: (errorMessage: string) =>  this.modalALert.open(AlertComponent, errorMessage, AlertType.Warning)
    });
  }

  public onToogleSenha() {
    this.showSenha = !this.showSenha;
    this.eyeIconClass = (this.eyeIconClass === 'bi-eye') ? 'bi-eye-slash' : 'bi-eye';
  }

  public onToogleConfirmaSenha(){
    this.showConfirmaSenha = !this.showConfirmaSenha;
    this.eyeIconClassConfirmaSenha = (this.eyeIconClassConfirmaSenha === 'bi-eye') ? 'bi-eye-slash' : 'bi-eye';
  }
}
