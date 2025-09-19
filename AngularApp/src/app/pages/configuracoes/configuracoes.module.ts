import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfiguracoesRoutingModule } from './configuracoes.routing.module';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../app.shared.module';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { ChangeAvatarComponent } from './change-avatar/change-avatar.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ConfiguracoesComponent } from './configuracoes.component';
import { DeleteDPComponent } from './delete-dados-pessoais/deleteDP.component';
import { MatCheckboxModule } from '@angular/material/checkbox';

@NgModule({
  declarations : [ConfiguracoesComponent, ChangeAvatarComponent, ChangePasswordComponent, DeleteDPComponent],
  imports: [CommonModule, ReactiveFormsModule, ConfiguracoesRoutingModule, MatIconModule, MatFormFieldModule, MatInputModule, MatCheckboxModule, SharedModule]
})

export class ConfiguracoesModule {}

