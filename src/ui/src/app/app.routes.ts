import { Routes } from '@angular/router';
import { AuthenticateComponent } from './pages/authenticate/authenticate.component';
import { RegistrationComponent } from './pages/registration/registration.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { MyFilesComponent } from './pages/my-files/my-files.component';
import { SharedWithMeComponent } from './pages/shared-with-me/shared-with-me.component';
import { SharedByMeComponent } from './pages/shared-by-me/shared-by-me.component';
import { canActivateDashboard } from './core/guards/auth.guard';

const dashboardRoutes: Routes = [
    {
        path: '',
        component: MyFilesComponent,
    },
    {
        path: 'shared-with-me',
        component: SharedWithMeComponent,
    },
    {
        path: 'shared-by-me',
        component: SharedByMeComponent,
    }
]

export const routes: Routes = [
    {
        path: 'authenticate',
        component: AuthenticateComponent,
    },
    {
        path: 'registration',
        component: RegistrationComponent,
    },
    {
        path: 'dashboard',
        canActivate: [canActivateDashboard],
        children: dashboardRoutes,
        component: DashboardComponent,
    },
    {
        path: '',
        redirectTo: '/dashboard',
        pathMatch: 'full'
    },
];

