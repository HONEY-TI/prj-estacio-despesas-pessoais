import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { MatInputModule } from "@angular/material/input";
import { CommonModule } from "@angular/common";
import { MatIconModule } from "@angular/material/icon";
import { LoginRoutingModule } from "./login.routing.module";
import { FooterComponent } from "../../components/footer/footer.component";
import { LoginComponent } from "./login.component";

@NgModule({
  declarations: [LoginComponent],
  imports: [CommonModule, LoginRoutingModule, MatInputModule, MatIconModule, ReactiveFormsModule, FooterComponent],
  exports: [LoginComponent]
})

export class LoginModule { }
