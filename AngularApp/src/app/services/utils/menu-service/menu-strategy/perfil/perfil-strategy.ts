import { Router } from '@angular/router';
import { MenuStrategy } from '../abstractions/menu.strategy.interface';

export class PerfilStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/perfil']);
    }

    setMenuName(): string {
        return 'Perfil';
    }
}