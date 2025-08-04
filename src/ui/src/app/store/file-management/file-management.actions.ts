import { createAction, props } from '@ngrx/store';
import { CreateFolderRequest, ErrorModel, FileDto, FolderDto, UserFileSystemDto } from '../../core/models/api-models';

// File System actions
export const loadFileSystem = createAction('[FileManagement] Load Top Level files and folders for User');
export const loadFileSystemSuccess = createAction(
    '[FileManagement] Load Top Level files and folders for User Success',
    props<{ fileSystem: UserFileSystemDto }>()
);
export const loadFileSystemFailure = createAction(
    '[FileManagement] Load Top Level files and folders for User Failure',
    props<{ error: any }>()
);

// Folder Actions
export const createFolder = createAction('[FileManagement] CreateFolder', props<{ folder: CreateFolderRequest }>());
export const createFolderSuccess = createAction(
    '[FileManagement] CreateFolder Success',
    props<{ folder: FolderDto }>()
);
export const createFolderFailure = createAction('[FileManagement] CreateFolder Failed', props<{ error: ErrorModel }>());

// Files Actions
export const addNewFilesSuccess = createAction('[FileManagement] Add new files success', props<{ files: FileDto[] }>());
