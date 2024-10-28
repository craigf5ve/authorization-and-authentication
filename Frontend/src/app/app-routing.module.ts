import { RouterModule } from '@angular/router';
import { NgModule } from '@angular/core';
import { NotfoundComponent } from './demo/components/notfound/notfound.component';
import { AppLayoutComponent } from "./layout/app.layout.component";
import { AuthGuardService } from 'src/proxy/services/auth-guard.service';

@NgModule({
    imports: [
        RouterModule.forRoot([
            {
                path: '', component: AppLayoutComponent,
                children: [
                    {
                        path: '',
                        loadChildren: () => import('./demo/components/dashboard/dashboard.module').then(m => m.DashboardModule),
                        canActivate: [AuthGuardService]
                    },
                    {
                        path: 'user-management',
                        loadChildren: () => import('./user-management/user-management.module').then(m => m.UserManagementModule),
                        canActivate: [AuthGuardService]
                    },
                ],
                canActivate: [AuthGuardService]
            },
            { path: 'auth', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule) },
            { path: 'landing', loadChildren: () => import('./demo/components/landing/landing.module').then(m => m.LandingModule) },
            { path: 'notfound', component: NotfoundComponent },
            { path: '**', redirectTo: '/notfound' },
        ], { scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled', onSameUrlNavigation: 'reload' })
    ],
    exports: [RouterModule]
})
export class AppRoutingModule {
}
