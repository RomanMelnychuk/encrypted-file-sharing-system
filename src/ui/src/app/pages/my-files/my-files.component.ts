import { Component, OnInit } from '@angular/core';
import { FileTableComponent } from '../../components/file-table/file-table.component';
import { Store } from '@ngrx/store';
import { FileSection } from '../../core/models/api-models';
import { map, Observable, of } from 'rxjs';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { selectFileSystem } from '../../store/file-management/file-management.selectors';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'fg-my-files',
    imports: [FileTableComponent, ProgressSpinnerModule, CommonModule],
    standalone: true,
    templateUrl: './my-files.component.html',
    styleUrl: './my-files.component.scss',
})
export class MyFilesComponent implements OnInit {
    myFiles$: Observable<FileSection | null> = of(null);

    constructor(private store: Store) {
        this.myFiles$ = this.store.select(selectFileSystem).pipe(
            map((res) => {
                return res?.myFiles ?? null;
            })
        );
    }

    ngOnInit(): void { }
}
