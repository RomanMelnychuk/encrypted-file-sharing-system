import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { SidebarComponent } from '../../components/sidebar/sidebar.component';
import { RouterModule } from '@angular/router';
import { Store } from '@ngrx/store';
import { loadFileSystem } from '../../store/file-management/file-management.actions';

@Component({
    selector: 'fg-dashboard',
    imports: [HeaderComponent, SidebarComponent, RouterModule],
    standalone: true,
    templateUrl: './dashboard.component.html',
    styleUrl: './dashboard.component.scss',
})
export class DashboardComponent  implements OnInit {
    constructor(private store: Store) {}

    ngOnInit(): void {
        this.store.dispatch(loadFileSystem());
    }
}
