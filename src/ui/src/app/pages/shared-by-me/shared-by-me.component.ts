import { Component } from '@angular/core';
import { FileTableComponent } from "../../components/file-table/file-table.component";
import { CommonModule } from '@angular/common';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { Store } from '@ngrx/store';
import { Observable, of, map } from 'rxjs';
import { FileSection } from '../../core/models/api-models';
import { selectFileSystem } from '../../store/file-management/file-management.selectors';

@Component({
    selector: 'fg-shared-by-me',
    imports: [FileTableComponent, CommonModule, ProgressSpinnerModule],
    standalone: true,
    templateUrl: './shared-by-me.component.html',
    styleUrl: './shared-by-me.component.scss',
})
export class SharedByMeComponent {
    sharedByMe$: Observable<FileSection | null> = of(null);

    constructor(private store: Store) {
        this.sharedByMe$ = this.store.select(selectFileSystem).pipe(map((res) => res?.sharedByMe ?? null));
    }
}
