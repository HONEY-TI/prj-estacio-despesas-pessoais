import { Router } from '@angular/router';
import { MenuStrategy } from '../abstractions/menu.strategy.interface';

export class LancamentoStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/lancamento']);
    }

    setMenuName(): string {
        return 'Lançamentos';
    }
}