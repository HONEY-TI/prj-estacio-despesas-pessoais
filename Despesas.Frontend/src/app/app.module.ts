import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app.routing.module';
import { AppComponent } from './app.component';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { LoginRoutingModule } from './pages/login/login.routing.module';
import { AlertComponent, ModalFormComponent, ModalConfirmComponent } from './shared/components';
import { AlertModule } from './shared/components/alert-component/alert.component.module';
import { AuthService, CustomInterceptor, MenuService } from './shared/services';
import { ControleAcessoService } from './shared/services/api';
import { MatNativeDateModule, MAT_DATE_LOCALE, DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { NgbActiveModal, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { MAT_MOMENT_DATE_FORMATS, MomentDateAdapter, MomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from '@angular/material-moment-adapter';
import { AcessoComponent } from './pages/acesso/acesso.component';


@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, AppRoutingModule, CommonModule, ReactiveFormsModule, AlertModule, AcessoComponent, LoginRoutingModule,
    MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule, BrowserAnimationsModule, MomentDateModule,
    NgxMaskDirective, NgxMaskPipe],
  providers: [AuthService, ControleAcessoService, MenuService, AlertComponent, ModalFormComponent, ModalConfirmComponent, NgbActiveModal, NgbDropdownModule,
    ReactiveFormsModule,
    CommonModule,
    { provide: HTTP_INTERCEPTORS, useClass: CustomInterceptor, multi: true, },
    { provide: MAT_DATE_LOCALE, useValue: 'pt-br' },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    provideNgxMask(), provideHttpClient(withInterceptorsFromDi()), provideAnimationsAsync()],
  bootstrap: [AppComponent]
})
export class AppModule { }
