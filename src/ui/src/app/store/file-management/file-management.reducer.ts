import { createReducer, on } from '@ngrx/store';
import { UserFileSystemDto } from '../../core/models/api-models';
import {
    addNewFilesSuccess,
    createFolderFailure,
    createFolderSuccess,
    loadFileSystem,
    loadFileSystemFailure,
    loadFileSystemSuccess,
} from './file-management.actions';

export interface FileManagementState {
    fileSystem: UserFileSystemDto | null;
    error: string | null;
    loading: boolean;
}

export const initialState: FileManagementState = {
    fileSystem: null,
    error: null,
    loading: false,
};

export const fileManagementReducers = createReducer(
    initialState,
    on(loadFileSystem, (state) => ({ ...state, loading: true })),
    on(loadFileSystemSuccess, (state, { fileSystem }) => ({ ...state, loading: false, fileSystem })),
    on(loadFileSystemFailure, (state, { error }) => ({ ...state, loading: false, error })),
    // Folder reducers
    on(createFolderSuccess, (state, { folder }) => ({
        ...state,
        fileSystem: state.fileSystem
            ? {
                  ...state.fileSystem,
                  myFiles: {
                      ...state.fileSystem.myFiles,
                      folders: [...state.fileSystem.myFiles.folders, folder],
                  },
              }
            : state.fileSystem,
    })),
    on(createFolderFailure, (state, { error }) => ({
        ...state,
        error: error.errorCode,
    })),
    on(addNewFilesSuccess, (state, { files }) => ({
        ...state,
        fileSystem: state.fileSystem
            ? {
                  ...state.fileSystem,
                  myFiles: {
                      ...state.fileSystem.myFiles,
                      files: [...state.fileSystem.myFiles.files, ...files],
                  },
              }
            : state.fileSystem,
    }))
);
