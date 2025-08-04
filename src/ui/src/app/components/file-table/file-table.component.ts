import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { PanelMenuModule } from 'primeng/panelmenu';
import { SplitButtonModule } from 'primeng/splitbutton';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToolbarModule } from 'primeng/toolbar';
import { catchError, EMPTY, first, iif, map, Observable, of, tap } from 'rxjs';
import { UserService } from '../../core/services/user.service';
import { UserProvider } from '../../core/services/user.provider';
import { FileSection } from '../../core/models/api-models';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DeleteItemsComponent } from '../delete-items/delete-items.component';
import { convertToLocalDateTime } from '../../utils/date';
import { FileManagementService } from '../../core/services/file-management.service';
import { MenuItem, MessageService } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';
import { FolderBreadcrumbComponent } from "../folder-breadcrumb/folder-breadcrumb.component";
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { loadFileSystem } from '../../store/file-management/file-management.actions';
import { formatSize, getFileIcon } from '../../utils/trash';

export interface TableItem {
    id: string;
    name: string;
    owner?: string;
    createdAt?: string;
    size?: string;
    type: 'folder' | 'file';
}

interface SelectUser {
    name: string;
    code: string; // UserId
}

@Component({
    selector: 'fg-file-table',
    standalone: true,
    imports: [
    TableModule,
    OverlayPanelModule,
    ButtonModule,
    SplitButtonModule,
    CommonModule,
    CheckboxModule,
    FormsModule,
    ToolbarModule,
    PanelMenuModule,
    TagModule,
    MultiSelectModule,
    BreadcrumbModule,
    FolderBreadcrumbComponent
],
    providers: [DialogService],
    templateUrl: './file-table.component.html',
    styleUrls: ['./file-table.component.scss'],
})
export class FileTableComponent implements OnInit, OnChanges {
    @Input() pageIcon: string | null = null;
    @Input() pageName: string | null = null;
    @Input() pageRouterLink: string = '';
    @Input() isDeleteVisible = true;
    @Input() isShareAccessVisible = true;

    @Input() files: FileSection | null = null;

    localFiles: FileSection | null = null;

    ref: DynamicDialogRef | undefined;

    topMenu: MenuItem | undefined;

    tableData: TableItem[] = [];

    users$: Observable<SelectUser[]> = of([]);

    selectedItem: TableItem | null = null;
    selectedUsers: SelectUser[] = [];
    downloading = false;

    constructor(
        private userService: UserService,
        private userProvider: UserProvider,
        private dialogService: DialogService,
        private fileManagementService: FileManagementService,
        private toastr: MessageService,
        private router: Router,
        private route: ActivatedRoute,
        private store: Store,
    ) {}

    ngOnChanges(changes: SimpleChanges): void {
        console.log(changes)
        this.processFiles();
    }

    ngOnInit(): void {
        this.processFiles();

        this.users$ = this.userService.getUsers().pipe(
            map((users) => {
                const filteredUsers = users.filter((user) => user.id !== this.userProvider.getCurrentUser()?.userId);
                return filteredUsers.map((user) => ({ name: user.email, code: user.id }));
            })
        );

        this.route.queryParams.subscribe(params => {
            const folderId = params['folderId'];
            if (folderId)
                this.fetchFolderItems(folderId);
            else {
                this.localFiles = this.files;
                this.processFiles();
            }

        });

        this.topMenu = { icon: this.pageIcon ?? '', routerLink: this.pageRouterLink, label: this.pageName ?? '' };


        this.topMenu = { icon: this.pageIcon ?? '', routerLink: this.pageRouterLink, label: this.pageName ?? ''  };
    }

    navigateToFolder(folder: TableItem): void {
        const currentParams = { ...this.route.snapshot.queryParams };
        const newPath = folder.name;

        this.router.navigate([], {
            relativeTo: this.route,
            queryParams: {
                folderId: folder.id,
                folderName: newPath
            },
            queryParamsHandling: 'replace'
        });
    }

    deleteItems() {
        this.ref = this.dialogService.open<DeleteItemsComponent>(DeleteItemsComponent, {
            header: 'Підтвердження видалення',
            data: {
                name: this.selectedItem?.name,
                type: this.selectedItem?.type,
                id: this.selectedItem?.id,
            },
        });

        this.ref.onClose.pipe(first()).subscribe(({ confirmed }) => {
            console.log(confirmed);
            if (confirmed)
                window.location.reload();
        });
    }

    download() {
        if (!this.selectedItem) return;

        this.downloading = true;

        this.fileManagementService
            .downloadFile(this.selectedItem.id)
            .pipe(first())
            .subscribe({
                next: (blob: Blob) => {
                    const url = window.URL.createObjectURL(blob);
                    const a = document.createElement('a');
                    a.href = url;
                    a.download = this.selectedItem?.name || 'download';
                    a.click();
                    window.URL.revokeObjectURL(url);
                    this.toastr.add({
                        severity: 'success',
                        summary: 'Success',
                        detail: 'Файл успішно розшифровано та завантажено.',
                    });
                },
                error: (error) => {
                    console.error('Download Error:', error);
                    this.toastr.add({
                        severity: 'error',
                        summary: 'Помилка',
                        detail: 'Неможливо розшифрувати та завантажити файл.',
                    });
                },
                complete: () => (this.downloading = false),
            });
    }

    shareItem() {
        if (!this.selectedItem) return;

        iif(
            () => this.selectedItem?.type === 'file',
            this.fileManagementService.shareFile(this.selectedItem.id, this.selectedUsers[0].code),
            this.fileManagementService.shareFolder(this.selectedItem.id, this.selectedUsers[0].code)
        )
            .pipe(
                first(),
                tap(() => {
                    this.toastr.add({
                        severity: 'success',
                        detail: 'Ви успішно поділились своїм файлом',
                        summary: 'Успіх',
                    });

                    this.store.dispatch(loadFileSystem());
                    this.selectedUsers = []
                }),
                catchError((err) => {

                    this.toastr.add({
                        severity: 'error',
                        summary: 'Помилка',
                        detail: 'Не вдалося поділитись файлом.',
                    });

                    return EMPTY;
                })
            )
            .subscribe();
    }

    convertToLocalDateTime(dateTime: string) {
        return convertToLocalDateTime(dateTime);
    }

    getFileIcon(file: TableItem): string {
        return getFileIcon(file.name, file.type);
    }

    private processFiles(): void {
        if (!this.localFiles) {
            this.tableData = [];
            return;
        }

        const folders: TableItem[] = this.localFiles.folders.map((folder) => ({
            id: folder.id,
            name: folder.name,
            type: 'folder' as const,
            createdAt: folder.createdAt,
            size: formatSize(folder.sizeInBytes ?? 0),
        }));

        const files: TableItem[] = this.localFiles.files.map((file) => ({
            id: file.id,
            name: file.fileName,
            type: 'file' as const,
            owner: 'Unknown',
            createdAt: file.createdAt,
            size: formatSize(file.sizeInBytes),
        }));

        this.tableData = [...folders, ...files];
    }

    private fetchFolderItems(folderId: string): void {
        this.fileManagementService.getFolder(folderId).pipe(
            first(),
            catchError(err => {
                console.error('Error fetching folder items:', err);
                this.toastr.add({
                    severity: 'error',
                    summary: 'Помилка',
                    detail: 'Не вдалося завантажити вміст папки.'
                });
                return EMPTY;
            })
        ).subscribe({
            next: (fileSection: FileSection) => {
                this.localFiles = fileSection;
                this.processFiles();
            }
        });
    }
}
