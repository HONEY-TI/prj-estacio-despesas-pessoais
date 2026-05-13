import { Router } from '@angular/router';
import { MenuStrategy } from '..';

export class CategoriaStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/categoria']);
    }

    setMenuName(): string {
        return 'Categorias';
    }
}