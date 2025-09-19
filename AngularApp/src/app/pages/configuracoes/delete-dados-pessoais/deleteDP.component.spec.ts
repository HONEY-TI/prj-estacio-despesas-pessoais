/* tslint:disable:no-unused-variable */
import {  ComponentFixture, TestBed } from '@angular/core/testing';
import { DeleteDPComponent } from './deleteDP.component';
import { AlertComponent, ModalConfirmComponent } from '../../../components';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

describe('DeleteDPComponent', () => {
  let component: DeleteDPComponent;
  let fixture: ComponentFixture<DeleteDPComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ DeleteDPComponent],
      providers: [AlertComponent, ModalConfirmComponent,  NgbActiveModal]
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
