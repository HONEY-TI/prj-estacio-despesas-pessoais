import { NgModule } from '@angular/core';
import { NgbModalConfig, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalConfirmComponent } from '..';
@NgModule({
  declarations: [ModalConfirmComponent],
  providers: [NgbModalConfig, NgbModal],
})

export class ModalConfirmModule {}
