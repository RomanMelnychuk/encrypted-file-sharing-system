import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthModel, RegistrationModel } from '../models';
import { BaseApiService } from './base-api.service';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
    providedIn: 'root',
})
export class AuthService extends BaseApiService {
    apiUrl = `${this.baseUrl}/authenticate`
    constructor(http: HttpClient, cookies: CookieService) {
        super(http, cookies);
    }

    authenticate(request: AuthModel): Observable<any> {
        return this.http.post<any>(this.apiUrl, request, { withCredentials: true });
    }

    registration(request: RegistrationModel): Observable<any> {
        return this.http.post<any>(`${this.apiUrl}/register`, request);
    }

    logout(){
        this.cookies.delete(this.tokenKey, '/');
        window.location.reload();
    }
}
