import { Component, OnInit } from '@angular/core';
import { MessageService, PrimeNGConfig } from 'primeng/api';
import { RouterOutlet } from '@angular/router';
import { ToastModule } from 'primeng/toast';
import { RippleModule } from 'primeng/ripple';
import { Store } from '@ngrx/store';
import { loadUser } from './store/user/user.actions';

@Component({
    selector: 'app-root',
    imports: [RouterOutlet, ToastModule, RippleModule],
    standalone: true,
    templateUrl: './app.component.html',
    styleUrl: './app.component.scss',
    providers: [MessageService]
})
export class AppComponent implements OnInit {
    constructor(private primengConfig: PrimeNGConfig, private store: Store) {}

    ngOnInit() {
        this.store.dispatch(loadUser())
        document.body.style.height = `${window.innerHeight}px`;
        this.primengConfig.ripple = true;
    }
    title = 'ui';
}
