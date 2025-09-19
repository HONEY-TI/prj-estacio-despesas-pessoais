import { Router } from '@angular/router';
import { MenuStrategy } from '..';

export class ConfiguracoesStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/configuracoes']);
    }

    setMenuName(): string {
        return 'Configurações';
    }
}