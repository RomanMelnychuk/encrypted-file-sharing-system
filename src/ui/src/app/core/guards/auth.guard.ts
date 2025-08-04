import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot } from "@angular/router";
import { UserProvider } from "../services/user.provider";

export const canActivateDashboard: CanActivateFn = (
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot,
) => {
    const service = inject(UserProvider);
    const router = inject(Router);

    if (!!service.getCurrentUser()) {
        return true;
    } else {
        router.navigate(['/authenticate']);
        return false;
    }
};
