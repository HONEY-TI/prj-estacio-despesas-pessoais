import { Router } from '@angular/router';
import { MenuStrategy } from '../abstractions/menu.strategy.interface';

export class DespesaStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/despesa']);
    }

    setMenuName(): string {
        return 'Despesas';
    }
}