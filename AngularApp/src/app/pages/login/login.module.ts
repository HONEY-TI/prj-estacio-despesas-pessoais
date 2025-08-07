import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { LoginComponent } from "./login.component";
import { MatInputModule } from "@angular/material/input";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { LoginRoutingModule } from "./login.routing.module";

@NgModule({
  declarations: [LoginComponent],
  imports: [CommonModule, LoginRoutingModule, MatInputModule, MatIconModule, ReactiveFormsModule],
  exports: [LoginComponent]
})

export class LoginModule { }
