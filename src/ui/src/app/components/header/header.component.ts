import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { InputIconModule } from 'primeng/inputicon';
import { IconFieldModule } from 'primeng/iconfield';
import { InputTextModule } from 'primeng/inputtext';
import { OverlayPanel, OverlayPanelModule } from 'primeng/overlaypanel';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { SkeletonModule } from 'primeng/skeleton';
import { DividerModule } from 'primeng/divider';
import { ButtonModule } from 'primeng/button';
import { ChipsModule } from 'primeng/chips';
import { CommonModule } from '@angular/common';
import { Store } from '@ngrx/store';
import { catchError, debounceTime, distinctUntilChanged, finalize, map, Observable, of, switchMap } from 'rxjs';
import { User } from '../../core/models';
import { selectCurrentUser } from '../../store/user/user.selector';
import { AuthService } from '../../core/services/auth.service';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { FileDto, FileSection, FolderDto } from '../../core/models/api-models';
import { FileManagementService } from '../../core/services/file-management.service';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { getFileIcon } from '../../utils/trash';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';

@Component({
    selector: 'fg-header',
    imports: [
        OverlayPanelModule,
        DividerModule,
        SkeletonModule,
        InputGroupModule,
        InputGroupAddonModule,
        ButtonModule,
        InputTextModule,
        ChipsModule,
        CommonModule,
        InputIconModule,
        IconFieldModule,
        ReactiveFormsModule,
        TagModule,
        TooltipModule,
    ],
    standalone: true,
    templateUrl: './header.component.html',
    styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit {
    user$: Observable<User | null> = of(null);

    searchControl: FormControl = new FormControl('');
    searchResults: FileSection = { files: [], folders: [] };
    isLoading = false;

    constructor(
        private store: Store,
        private auth: AuthService,
        private fileManagementService: FileManagementService,
        private toast: MessageService,
        private router: Router,
    ) {}

    ngOnInit(): void {
        this.user$ = this.store.select(selectCurrentUser);

        this.searchControl.valueChanges
            .pipe(
                debounceTime(300),
                distinctUntilChanged(),
                switchMap((query) => {
                    if (query && query.trim().length > 0) {
                        this.isLoading = true;

                        console.log(query);
                        return this.fileManagementService.searchFilesAndFolders(query).pipe(
                            map((results: FileSection) => {
                                this.searchResults = results;
                                console.log(results);
                            }),
                            catchError((error) => {
                                this.toast.add({
                                    severity: 'error',
                                    summary: 'Помилка',
                                    detail: 'Не вдалося виконати пошук.',
                                });
                                return of(null);
                            }),
                            finalize(() => (this.isLoading = false))
                        );
                    } else {
                        return of(null);
                    }
                }),
            )
            .subscribe();
    }

    logout() {
        this.auth.logout();
    }

    getFileIcon = (file: FileDto) => {
        return getFileIcon(file.fileName, 'file');
    }

    navigateToFolder(folder: FolderDto) {
        this.router.navigate(['/dashboard'], {
            queryParams: {
                folderName: folder.name,
                folderId: folder.id,
            },
            queryParamsHandling: 'merge',
        });
    }

    downloadFile(file: FileDto) {
        this.fileManagementService.downloadFile(file.id).subscribe(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = file.fileName;
            a.click();
            window.URL.revokeObjectURL(url);
        });
    }
}
