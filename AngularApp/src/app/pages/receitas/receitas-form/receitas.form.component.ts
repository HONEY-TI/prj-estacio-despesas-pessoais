import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import dayjs from 'dayjs';
import { AlertComponent, AlertType } from '../../../components';
import { ICategoria, IReceita, Action } from '../../../models';
import { ReceitaService } from '../../../services/api';
import { CustomValidators } from '../../validators';

@Component({
  selector: 'app-receitas-form',
  templateUrl: './receitas.form.component.html',
  styleUrls: ['./receitas.form.component.scss'],
  standalone: false
})

export class ReceitasFormComponent {
  categorias: ICategoria[] = [];
  receitaForm: FormGroup & IReceita;
  action: Action = Action.Create;
  refresh: Function = () => { };  
  setRefresh(_refresh: Function): void {
    this.refresh = _refresh;
  }

  constructor(
    public formbuilder: FormBuilder,
    public modalAlert: AlertComponent,
    public activeModal: NgbActiveModal,
    public receitaService: ReceitaService ) { }

  public ngOnInit(): void {
    this.getCatgeoriasFromReceitas();
    this.receitaForm = this.formbuilder.group({
      id: [null],
      categoria: [null, Validators.required],
      data: [dayjs().format('YYYY-MM-DD'), Validators.required],
      descricao: ['', Validators.required],
      valor: [0, [Validators.required, CustomValidators.isGreaterThanZero]],
    }) as FormGroup & IReceita;
  }

  public onSaveClick = () => {
    switch (this.action) {
      case Action.Create:
        this.saveCreateReceita();
        break;
      case Action.Edit:
        this.saveEditReceita();
        break;
      default:
        this.modalAlert.open(AlertComponent, 'Ação não pode ser realizada.', AlertType.Warning);
    }
  }

  private getCatgeoriasFromReceitas = () => {
    this.receitaService.getReceitasCategorias()
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

  private saveCreateReceita = () => {
    this.receitaService.postReceita(this.receitaForm.getRawValue() as IReceita)
      .subscribe({
        next: (result: IReceita) => {
          if (result) {
            this.activeModal.close();
            this.refresh();
            this.modalAlert.open(AlertComponent, 'Receita cadastrada com Sucesso.', AlertType.Success);
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  private saveEditReceita = () => {
    this.receitaService.putReceita(this.receitaForm.getRawValue() as IReceita)
      .subscribe({
        next: (response: IReceita) => {
          if (response !== undefined && response !== null && response) {
            this.activeModal.close();
            this.refresh();
            this.modalAlert.open(AlertComponent, 'Receita alterada com Sucesso.', AlertType.Success);
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  public editReceita = (idReceita: number) => {
    this.receitaService.getReceitaById(idReceita)
      .subscribe({
        next: (response: IReceita) => {
          if (response && response !== undefined && response !== null){
            const receitaData = response
            this.receitaForm.patchValue(receitaData);
            const categoriaSelecionada = this.categorias.find(c => c.id === receitaData.categoria.id);
            if (categoriaSelecionada) {
              this.receitaForm.get('categoria')?.setValue(categoriaSelecionada);
            }
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }

  public deleteReceita = (idReceita: number, callBack: Function) => {
    this.receitaService.deleteReceita(idReceita)
      .subscribe({
        next: (response: boolean) => {
          if (response) {
            callBack();
            this.modalAlert.open(AlertComponent, 'Receita excluída com sucesso', AlertType.Success);
          }
          else {
            this.modalAlert.open(AlertComponent, 'Erro ao excluír receita', AlertType.Warning);
          }
        },
        error: (errorMessage: string) => {
          this.modalAlert.open(AlertComponent, errorMessage, AlertType.Warning);
        }
      });
  }
}
