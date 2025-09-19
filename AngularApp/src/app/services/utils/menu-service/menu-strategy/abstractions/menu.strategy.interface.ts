import { Router } from "@angular/router";

export interface MenuStrategy {
    navigate(router: Router): void;
    setMenuName(): string;
}
