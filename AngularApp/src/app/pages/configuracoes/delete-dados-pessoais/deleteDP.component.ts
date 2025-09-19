import { Component, OnInit } from '@angular/core';
import { AlertComponent, AlertType, ModalConfirmComponent } from '../../../components';

@Component({
  selector: 'app-deleteDP',
  templateUrl: './deleteDP.component.html',
  styleUrls: ['./deleteDP.component.scss'],
  standalone: false
})

export class DeleteDPComponent implements OnInit {
  aceiteDados: boolean = false;
  constructor(
    public modalAlert: AlertComponent,    
    public modalConfirm: ModalConfirmComponent) { }

  ngOnInit() {}

  public handledeleteDadosPessoais(): void {
    if (!this.aceiteDados) return;
    const modalRef = this.modalConfirm.open(
      ModalConfirmComponent,
      `Deseja aceitar a exclusão dos dados pessoais ?`);
    modalRef.componentInstance.setConfirmButton(
      () => this.deleteDadosPessoais());
  }

  private deleteDadosPessoais(): void {
    this.modalAlert.open(
      AlertComponent, 
      'Dados pessoais excluídos com sucesso!', 
      AlertType.Success);    
  }
}

