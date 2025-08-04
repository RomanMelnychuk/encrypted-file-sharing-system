import { Component } from '@angular/core';
import { FileTableComponent } from '../../components/file-table/file-table.component';
import { map, Observable, of } from 'rxjs';
import { FileSection } from '../../core/models/api-models';
import { Store } from '@ngrx/store';
import { selectFileSystem } from '../../store/file-management/file-management.selectors';
import { CommonModule } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';

@Component({
    selector: 'fg-shared-with-me',
    imports: [FileTableComponent, CommonModule, ProgressSpinnerModule],
    standalone: true,
    templateUrl: './shared-with-me.component.html',
    styleUrl: './shared-with-me.component.scss',
})
export class SharedWithMeComponent {
    sharedWithMe$: Observable<FileSection | null> = of(null);

    constructor(private store: Store) {
        this.sharedWithMe$ = this.store.select(selectFileSystem).pipe(map((res) => res?.sharedWithMe ?? null));
    }
}
