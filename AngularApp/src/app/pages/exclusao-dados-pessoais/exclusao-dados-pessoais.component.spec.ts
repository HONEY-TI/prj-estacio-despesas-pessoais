/* tslint:disable:no-unused-variable */
import {  ComponentFixture, TestBed } from '@angular/core/testing';
import { ExclusaoDadosPessoaisComponent } from './exclusao-dados-pessoais.component';

describe('ExclusaoDadosPessoaisComponent', () => {
  let component: ExclusaoDadosPessoaisComponent;
  let fixture: ComponentFixture<ExclusaoDadosPessoaisComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ ExclusaoDadosPessoaisComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExclusaoDadosPessoaisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
