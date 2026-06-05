import { Router } from '@angular/router';
import { MenuStrategy } from '../abstractions/menu.strategy.interface';

export class ReceitaStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/receita']);
    }

    setMenuName(): string {
        return 'Receitas';
    }
}