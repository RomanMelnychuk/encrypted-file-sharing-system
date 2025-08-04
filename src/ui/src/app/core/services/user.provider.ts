import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { jwtDecode } from 'jwt-decode';
import { TokenUser } from '../models';

@Injectable({ providedIn: 'root' })
export class UserProvider {
    private cookieKey = 'fg_access_token';

    constructor(private cookieService: CookieService) {}

    getCurrentUser(): TokenUser | null {
        const token = this.cookieService.get(this.cookieKey);
        if (!token) return null;

        try {
            const decodedToken: any = jwtDecode(token);
            const user: TokenUser = {
                userId: decodedToken.userId,
                email: decodedToken.email,
                username: decodedToken.sub,
            };
            return user;
        } catch (error) {
            console.error('Invalid token:', error);
            return null;
        }
    }
}
