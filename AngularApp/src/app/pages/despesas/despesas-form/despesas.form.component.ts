import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import dayjs from 'dayjs';
import { AlertComponent, AlertType } from '../../../components';
import { ICategoria, IDespesa, Action } from '../../../models';
import { DespesaService } from '../../../services/api';
import { CustomValidators } from '../../validators';

@Component({
  selector: 'app-despesas-form',
  templateUrl: './despesas.form.component.html',
  styleUrls: ['./despesas.form.component.scss'],
  standalone: false
})

export class DespesasFormComponent {
  categorias: ICategoria[] = [];
  despesaForm: FormGroup & IDespesa;
  action: Action = Action.Create;
  refresh: Function = () => { };
  setRefresh(_refresh: Function): void {
    this.refresh = _refresh;
  }

  constructor(
    public formbuilder: FormBuilder,
    public modalAlert: AlertComponent,
    public activeModal: NgbActiveModal,
    public despesaService: DespesaService
  ) { }

  ngOnInit(): void {
    this.getCatgeoriasFromDespesas();
    this.despesaForm = this.formbuilder.group({
      id: [null],
      categoria: [null, Validators.required],
      data: [dayjs().format('YYYY-MM-DD'), Validators.required],
      descricao: ['', Validators.required],
      valor: [0, [Validators.required, CustomValidators.isGreaterThanZero]],
      dataVencimento: null
    }) as FormGroup & IDespesa;
  }

  getCatgeoriasFromDespesas = () => {
    this.despesaService.getDespesasCategorias()
      .subscribe({
        next: (result: ICategoria[]) => {
          if (result)
            this.categorias = result;
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  onSaveClick = () => {
    switch (this.action) {
      case Action.Create:
        this.saveCreateDespesa();
        break;
      case Action.Edit:
        this.saveEditDespesa();
        break;
      default:
        this.modalAlert.open(AlertComponent, 'Ação não pode ser realizada.', AlertType.Warning);
    }
  }

  saveCreateDespesa = () => {
    this.despesaService.postDespesa(this.despesaForm.getRawValue() as IDespesa)
      .subscribe({
        next: (result: boolean) => {
          if (result) {
            this.activeModal.close();
            this.refresh();
            this.modalAlert.open(AlertComponent, 'Despesa cadastrada com Sucesso.', AlertType.Success);
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  saveEditDespesa = () => {
    const despesa = this.despesaForm.getRawValue() as IDespesa;
    this.despesaService.putDespesa(despesa)
      .subscribe({
        next: (response: IDespesa) => {
          if (response !== undefined && response !== null) {
            this.activeModal.close();
            this.refresh();
            this.modalAlert.open(AlertComponent, 'Despesa alterada com Sucesso.', AlertType.Success);
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });

  }

  editDespesa = (idDespesa: number) => {
    this.despesaService.getDespesaById(idDespesa)
      .subscribe({
        next: (response: IDespesa) => {
          if (response !== undefined && response !== null){
            const despesaData = response;
            this.despesaForm.patchValue(despesaData);
            const categoriaSelecionada = this.categorias.find(c => c.id === despesaData.categoria.id);
            if (categoriaSelecionada) {
              this.despesaForm.get('categoria')?.setValue(categoriaSelecionada);
            }
          }

        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  deleteDespesa = (idDespesa: number, callBack: Function) => {
    this.despesaService.deleteDespesa(idDespesa)
      .subscribe({
        next: (response: any) => {
          if (response) {
            callBack();
            this.modalAlert.open(AlertComponent, 'Despesa excluída com sucesso', AlertType.Success);
          }
          else {
            this.modalAlert.open(AlertComponent, 'Erro ao excluír despesa', AlertType.Warning);
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }
}
