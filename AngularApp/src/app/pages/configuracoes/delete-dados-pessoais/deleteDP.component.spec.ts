/* tslint:disable:no-unused-variable */
import {  ComponentFixture, TestBed } from '@angular/core/testing';
import { DeleteDPComponent } from './exclusao-dados-pessoais.component';

describe('ExclusaoDadosPessoaisComponent', () => {
  let component: DeleteDPComponent;
  let fixture: ComponentFixture<DeleteDPComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ DeleteDPComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteDPComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
