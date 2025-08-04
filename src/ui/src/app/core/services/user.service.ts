import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseApiService {
    url = `${this.baseUrl}/User`;

    constructor(http: HttpClient, cookies: CookieService) {
        super(http, cookies);
    }

    get(): Observable<User> {
        return this.http.get<User>(this.url, this.options);
    }

    getUsers(): Observable<User[]>  {
        return this.http.get<User[]>(`${this.url}/all`, this.options);
    }
}
