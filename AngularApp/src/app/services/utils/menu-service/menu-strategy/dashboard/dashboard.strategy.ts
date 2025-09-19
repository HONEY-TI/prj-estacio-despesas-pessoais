import { Router } from '@angular/router';
import { MenuStrategy } from '..';

export class DashboardStrategy implements MenuStrategy {
    navigate(router: Router): void {
        router.navigate(['/dashboard']);
    }

    setMenuName(): string {
        return 'Dashboard';
    }
}
