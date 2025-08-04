import { Component, Input, OnInit } from '@angular/core';
import { TableItem } from '../file-table/file-table.component';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { FileManagementService } from '../../core/services/file-management.service';
import { catchError, EMPTY, finalize, first, iif, tap } from 'rxjs';
import { MessageService } from 'primeng/api';
import { Store } from '@ngrx/store';
import { loadFileSystem } from '../../store/file-management/file-management.actions';

@Component({
    selector: 'fg-delete-items',
    standalone: true,
    imports: [CommonModule, ButtonModule],
    templateUrl: './delete-items.component.html',
})
export class DeleteItemsComponent implements OnInit {
    constructor(
        private ref: DynamicDialogRef,
        private config: DynamicDialogConfig,
        private fileManagementService: FileManagementService,
        private toastr: MessageService,
        private store: Store
    ) {}

    itemName: string = '';
    itemType: string = '';
    itemId: string = '';

    isLoading = false;

    ngOnInit(): void {
        this.itemName = this.config.data.name;
        this.itemType = this.config.data.type;
        this.itemId = this.config.data.id;
    }

    cancel() {
        this.ref.close({ confirmed: false });
    }

    submit() {
        this.isLoading = true;
        iif(
            () => this.itemType === 'file',
            this.fileManagementService.deleteFile(this.itemId),
            this.fileManagementService.deleteFolder(this.itemId)
        )
            .pipe(
                first(),
                tap(() => {
                    this.toastr.add({ severity: 'success', detail: 'Елемент успішно видалено', summary: 'Успіх' });
                    this.store.dispatch(loadFileSystem());
                }),
                catchError(() => {
                    this.toastr.add({
                        severity: 'error',
                        detail: 'Виникла помилка при видаленні елементу',
                        summary: 'Помилка',
                    });

                    return EMPTY;
                }),
                finalize(() => {
                    this.isLoading = false;
                    this.ref.close({ confirmed: true });
                })
            )
            .subscribe();
    }
}
