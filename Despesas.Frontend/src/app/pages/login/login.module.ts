import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { LoginComponent } from "./login.component";
import { MatInputModule } from "@angular/material/input";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { LoginRoutingModule } from "./login.routing.module";
import { SharedModule } from "../../shared/shared.module";

@NgModule({
  declarations: [LoginComponent],
  imports: [CommonModule, ReactiveFormsModule, LoginRoutingModule, SharedModule, MatInputModule, MatIconModule],
  exports: [LoginComponent]
})

export class LoginModule { }
