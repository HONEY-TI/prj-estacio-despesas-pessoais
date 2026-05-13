import { Component } from '@angular/core';
import { NgbModalConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-modal-form',
  template: ''
})
export class ModalFormComponent {

  constructor(public config: NgbModalConfig, public modalService: NgbModal) {
		config.backdrop = 'static';
    config.keyboard = false;
	}

	public open(content: any): void  {
		this.modalService.open(content, { centered: true });
	}

  public close(): void {
    this.modalService.dismissAll();
  }
}
