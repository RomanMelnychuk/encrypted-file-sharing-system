import { HttpClient, HttpHeaders, HttpParamsOptions } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
    providedIn: 'root',
})
export class BaseApiService {
    protected tokenKey = 'fg_access_token';
    protected baseUrl = 'https://localhost:7282/api';

    constructor(protected http: HttpClient, protected cookies: CookieService) {}

    protected options = { withCredentials: true, headers: this.getAuthHeaders() };

    protected getAuthHeaders(): HttpHeaders {
        const token = this.cookies.get(this.tokenKey);

        return new HttpHeaders({
            Authorization: `Bearer ${token}`,
        });
    }
}
