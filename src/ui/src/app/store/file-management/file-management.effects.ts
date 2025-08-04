import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { loadFileSystem, loadFileSystemFailure, loadFileSystemSuccess } from './file-management.actions';
import { FileManagementService } from '../../core/services/file-management.service';
import { UserFileSystemDto } from '../../core/models/api-models';
import { MessageService } from 'primeng/api';

@Injectable({
    providedIn: 'root',
})
export class FileManagementEffects {
    constructor(private actions$: Actions, private fileManagement: FileManagementService, private toast: MessageService) {}

    loadFileSystem$ = createEffect(() =>
        this.actions$.pipe(
            ofType(loadFileSystem),
            switchMap(() =>
                this.fileManagement.getUserTopLevelFolder().pipe(
                    map((fileSystem: UserFileSystemDto) => {
                        return loadFileSystemSuccess({ fileSystem });
                    }),
                    catchError((error) => of(loadFileSystemFailure({ error })))
                )
            )
        )
    );
}
