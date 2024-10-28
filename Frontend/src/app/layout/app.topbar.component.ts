import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { LayoutService } from "./service/app.layout.service";
import { AccountService } from 'src/proxy/services/account.service';
import { AuthService } from 'src/proxy/services/auth.service';
import { RevokeTokenRequestDto } from 'src/proxy/Interfaces/Authentication/revoke-token-request-dto';
import { AccountResponseDto } from 'src/proxy/Interfaces/Authentication/account-response-dto';
import { error } from 'console';

@Component({
    selector: 'app-topbar',
    templateUrl: './app.topbar.component.html'
})
export class AppTopBarComponent {

    menuItems: MenuItem[] = [];

    items!: MenuItem[];

    @ViewChild('menubutton') menuButton!: ElementRef;

    @ViewChild('topbarmenubutton') topbarMenuButton!: ElementRef;

    @ViewChild('topbarmenu') menu!: ElementRef;

    @HostListener('window:resize', ['$event'])
    onResize(event: any) {
        this.handleResize();
    }

    revokeRequest: RevokeTokenRequestDto = {} as RevokeTokenRequestDto;

    currentUser: AccountResponseDto = {} as AccountResponseDto;

    greeting: string;

    icon: string;

    notifications: string[] = [];

    value: number = 0;

    issuesValue: number = 0;

    message = '';
    isDesktop = true;
    constructor(
        public layoutService: LayoutService,
        private accountService: AccountService,
        private authService: AuthService,
    ) {
        const currentTime = new Date().getHours()

        if (currentTime < 12) {
            this.greeting = 'Good Morning!';
            this.icon = "pi pi-sun"
        } else if (currentTime < 18) {
            this.greeting = 'Good Afternoon!';
            this.icon = "pi pi-sun"

        } else {
            this.greeting = 'Good Evening!';
            this.icon = "pi pi-moon"

        }
    }

    ngOnInit() {
        this.handleResize();
        this.menuItems = [

            {
                label: 'Profile',
                icon: 'pi pi-fw pi-user',
                routerLink: ['user-management/profile']
            },
            {
                label: 'Settings',
                icon: 'pi pi-fw pi-cog'
            },
            {
                separator: true
            },
            {
                label: 'Log Out',
                icon: 'pi pi-fw pi-sign-out',
                routerLink: ['/auth/login'],
                command: (onclick) => { this.logout() }
            },
        ];

        this.currentUser = JSON.parse(localStorage.getItem('currentUser')!) || {} as AccountResponseDto;
    }

    logout() {
        const refreshToken = this.authService.getRefreshToken();
        this.revokeRequest.token = refreshToken != null ? refreshToken : '';
        this.authService.revokeToken(this.revokeRequest)
            .subscribe((res: any) => {
                if (res.isSuccess) {
                    //Clear localStorage
                    this.authService.clearLocalStorage();

                    //Clear refreshToken
                    // this.authService.clearRefreshToken();
                }

            },
                (error) => {
                    console.error(error)
                })

    }
    handleResize() {
        if (window.innerWidth > 991) {
            this.isDesktop = true
        } else {
            this.isDesktop = false
        }
    }
}
