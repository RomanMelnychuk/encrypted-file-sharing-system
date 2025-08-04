import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../core/services/auth.service';
import { catchError, EMPTY, first, tap, finalize, delay } from 'rxjs';
import { MessageService } from 'primeng/api';
import { Store } from '@ngrx/store';
import { loadUser } from '../../store/user/user.actions';

@Component({
    selector: 'fg-authenticate',
    imports: [ReactiveFormsModule, CommonModule, RouterModule, InputTextModule, ButtonModule, PasswordModule],
    standalone: true,
    templateUrl: './authenticate.component.html',
    styleUrl: './authenticate.component.scss',
})
export class AuthenticateComponent {
    isLoading = false;
    loginForm!: FormGroup;

    constructor(
        private fb: FormBuilder,
        private authService: AuthService,
        private store: Store,
        private router: Router,
        private toast: MessageService
    ) {}

    ngOnInit(): void {
        this.loginForm = this.fb.group({
            username: ['', [Validators.required]],
            password: ['', Validators.required],
        });
    }

    onLogin(): void {
        if (!this.loginForm.valid) {
            console.log('Login Data:', this.loginForm.value);
            return;
        }
        this.isLoading = true;
        this.authService
            .authenticate(this.loginForm.value)
            .pipe(
                first(),
                tap(() => {
                    setTimeout(() => {
                        this.store.dispatch(loadUser());
                    }, 1000);
                    this.router.navigate(['/']);
                        this.toast.add({
                            severity: 'success',
                            summary: 'Success',
                            detail: 'You are successfully logged in!',
                        });
                }),
                catchError((err) => {
                    this.loginForm.reset();
                    console.log(err);
                    this.toast.add({ severity: 'error', summary: 'Error', detail: 'Incorrect login or password!' });
                    return EMPTY;
                }),
                finalize(() => (this.isLoading = false))
            )
            .subscribe();
    }
}
