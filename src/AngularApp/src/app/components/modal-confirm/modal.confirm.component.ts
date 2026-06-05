import { Component } from '@angular/core';
import { NgbModalConfig, NgbModal, NgbActiveModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-modal-confirm',
  templateUrl: './modal.confirm.component.html'
})

export class ModalConfirmComponent {
  header:string = 'Mensagem';
  message:string ='';
  onClickConfirm: Function = () => {};

  constructor(config: NgbModalConfig, public modalService: NgbModal, public activeModal: NgbActiveModal) {
    config.backdrop = 'static';
    config.keyboard = false;
  }

  public open(content: any, _message: string): NgbModalRef {
    let modalRef = this.modalService.open(content);
    modalRef.componentInstance.message = _message;
    return modalRef;
  }

  public close(): void {
    this.modalService.dismissAll();
  }

  public setConfirmButton( _confirm: Function): void {
    this.onClickConfirm = () => {
      _confirm();
      this.activeModal.close();
    };
  }
}
