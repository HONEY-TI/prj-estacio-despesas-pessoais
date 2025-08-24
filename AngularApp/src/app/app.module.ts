import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgbActiveModal, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app.routing.module';
import { AcessoComponent } from './pages/acesso/acesso.component';
import { AlertComponent, ModalFormComponent, ModalConfirmComponent, LoadingComponent } from './components';
import { AlertModule } from './components/alert-component/alert.component.module';
import { AuthService, MenuService, CustomInterceptor } from './services';
import { AcessoService } from './services/api';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE, MatNativeDateModule } from '@angular/material/core';
import { MAT_MOMENT_DATE_FORMATS, MomentDateAdapter, MomentDateModule, MAT_MOMENT_DATE_ADAPTER_OPTIONS, } from '@angular/material-moment-adapter';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { DashboardModule } from './pages/dashboard/dashboard.module';
import { CategoriasModule } from './pages/categorias/categorias.module';
import { DespesasModule } from './pages/despesas/despesas.module';
import { ReceitasModule } from './pages/receitas/receitas.module';
import { LancamentosModule } from './pages/lancamentos/lancamentos.module';

@NgModule({
  declarations: [AppComponent],
  bootstrap: [AppComponent],
  imports: [BrowserModule, AppRoutingModule, CommonModule, ReactiveFormsModule, AcessoComponent, AlertModule, DashboardModule, CategoriasModule, DespesasModule, ReceitasModule, LancamentosModule, 
    MatFormFieldModule, MatInputModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule, BrowserAnimationsModule, MomentDateModule, NgbDropdownModule,
    NgxMaskDirective, NgxMaskPipe],
  providers: [AuthService, AcessoService, MenuService, AlertComponent, ModalFormComponent, ModalConfirmComponent, NgbActiveModal,
    { provide: HTTP_INTERCEPTORS, useClass: CustomInterceptor, multi: true, },
    { provide: MAT_DATE_LOCALE, useValue: 'pt-br' },
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
    provideNgxMask(), provideHttpClient(withInterceptorsFromDi()), provideAnimationsAsync()]
})

export class AppModule { }
