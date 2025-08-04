import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { MenuItem, Message, MessageService } from 'primeng/api';
import { MenuModule } from 'primeng/menu';
import { RippleModule } from 'primeng/ripple';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { CardModule } from 'primeng/card';
import { DialogModule } from 'primeng/dialog';
import { FloatLabelModule } from 'primeng/floatlabel';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';
import { FileUploadZoneComponent } from '../file-upload-zone/file-upload-zone.component';
import { FileManagementService } from '../../core/services/file-management.service';
import { Store } from '@ngrx/store';
import {
    addNewFilesSuccess,
    createFolder,
    createFolderFailure,
    createFolderSuccess,
    loadFileSystemFailure,
} from '../../store/file-management/file-management.actions';
import { catchError, EMPTY, finalize, first, map, of } from 'rxjs';
import { ErrorModel, FileDto, FolderDto } from '../../core/models/api-models';
import { DividerModule } from 'primeng/divider';
import { MessagesModule } from 'primeng/messages';
import { ActivatedRoute, IsActiveMatchOptions } from '@angular/router';

@Component({
    selector: 'fg-sidebar',
    imports: [
        MenuModule,
        ButtonModule,
        RippleModule,
        OverlayPanelModule,
        CardModule,
        DialogModule,
        FloatLabelModule,
        FormsModule,
        InputTextModule,
        FileUploadZoneComponent,
        DividerModule,
        MessagesModule,
    ],
    standalone: true,
    templateUrl: './sidebar.component.html',
    styleUrl: './sidebar.component.scss',
})
export class SidebarComponent implements OnInit {
    readonly routeMatchOptions: IsActiveMatchOptions = {
        queryParams: 'ignored',
        matrixParams: 'exact',
        paths: 'exact',
        fragment: 'exact',
    };

    constructor(
        private store: Store,
        private fileManagementService: FileManagementService,
        private toast: MessageService,
        private route: ActivatedRoute,
    ) {}

    folderDialog = false;
    fileUploadDialog = false;
    buttonLoading = false;
    uploadFilesLoading = false;

    filesToAdd: File[] = [];

    folderName: string = '';

    menuItems: MenuItem[] = [
        {
            label: 'Мої файли',
            icon: 'pi pi-home',
            routerLink: ['/dashboard'],
            routerLinkActiveOptions: { exact: true },
        },
        {
            label: 'Поділилися зі мною',
            icon: 'pi pi-user',
            routerLink: ['/dashboard/shared-with-me'],
            routerLinkActiveOptions: { exact: true },
        },
        {
            label: 'Я поділився',
            icon: 'pi pi-users',
            routerLink: ['/dashboard/shared-by-me'],
            routerLinkActiveOptions: { exact: true },
        },
    ];

    ngOnInit(): void {}

    createFolder() {
        this.buttonLoading = true;

        const parentFolderId = this.route.snapshot.queryParams['folderId'] || null;

        this.fileManagementService
            .createFolder({ folderName: this.folderName, parentFolderId })
            .pipe(
                first(),
                map((folderDto: FolderDto) => {
                    this.store.dispatch(createFolderSuccess({ folder: folderDto }));
                    this.toast.add({
                        severity: 'success',
                        summary: 'Успіх',
                        detail: `Папка "${folderDto.name}" успішно створена!`,
                    });
                }),
                catchError((error) => {
                    this.toast.add({
                        severity: 'error',
                        summary: 'Помилка',
                        detail: 'Виникла помилка при створенні папки!',
                    });

                    this.store.dispatch(createFolderFailure({ error: error }));
                    return of(error);
                }),
                finalize(() => {
                    this.folderDialog = false;
                    this.buttonLoading = false;
                })
            )
            .subscribe();
    }

    cancelFolderCreation() {
        this.folderDialog = false;
        this.buttonLoading = false;
        this.folderName = '';
    }

    onFileAdd(currentFiles: File[]) {
        this.filesToAdd = currentFiles;
    }

    cancelFileUploading() {
        this.filesToAdd = [];
        this.fileUploadDialog = false;
    }

    uploadFiles() {
        this.uploadFilesLoading = true;

        const parentFolderId = this.route.snapshot.queryParams['folderId'] || null;

        this.fileManagementService
            .uploadFiles(parentFolderId, this.filesToAdd)
            .pipe(
                first(),
                map((files: FileDto[]) => {
                    this.store.dispatch(addNewFilesSuccess({ files }));

                    this.toast.add({
                        severity: 'success',
                        summary: 'Успіх',
                        detail: `Успішно було додано нові файли!`,
                    });
                }),
                catchError((err: ErrorModel) => {
                    console.error('Upload error', err);

                    this.toast.add({
                        severity: 'error',
                        summary: 'Помилка',
                        detail: 'Під час імпорту файлів виникла помилка!',
                    });
                    return EMPTY;
                })
            )
            .subscribe();

        this.fileUploadDialog = false;
        this.filesToAdd = [];
    }
}
