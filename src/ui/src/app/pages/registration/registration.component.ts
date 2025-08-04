import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../core/services/auth.service';
import { catchError, EMPTY, first, tap } from 'rxjs';

@Component({
    selector: 'fg-registration',
    imports: [ReactiveFormsModule, InputTextModule, ButtonModule, CommonModule, RouterModule, PasswordModule],
    standalone: true,
    templateUrl: './registration.component.html',
    styleUrl: './registration.component.scss',
})
export class RegistrationComponent {
    registerForm!: FormGroup;

    constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {}

    ngOnInit(): void {
        this.registerForm = this.fb.group({
            username: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            password: ['', Validators.required],
        });
    }

    onRegister(): void {
        if (!this.registerForm.valid) {
            console.log('Register Data:', this.registerForm.value);
            return;
        }

        this.authService
            .registration(this.registerForm.value)
            .pipe(
                first(),
                catchError((err) => {
                    this.registerForm.reset();
                    return EMPTY;
                }),
                tap(() => this.router.navigate(['/dashboard']))
            )
            .subscribe();
    }
}
