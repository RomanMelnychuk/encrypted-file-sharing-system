import { createSelector, createFeatureSelector } from '@ngrx/store';
import { FileManagementState } from './file-management.reducer';

export const selectFileManagementState = createFeatureSelector<FileManagementState>('fileManagement');

export const selectFileSystem = createSelector(selectFileManagementState, (state) => state.fileSystem);

export const selectIsLoading = createSelector(selectFileManagementState, (state) => state.loading);

export const selectError = createSelector(selectFileManagementState, (state) => state.error);
