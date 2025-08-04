
import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { UserEffects } from "./user/user.effects";
import * as user from "./user/user.reducer";
import * as fileSystem from "./file-management/file-management.reducer";
import { FileManagementEffects } from './file-management/file-management.effects';

export const appEffects = [UserEffects, FileManagementEffects]

export interface State {
    user: user.UserState;
    fileManagement: fileSystem.FileManagementState;
}

export const reducers: ActionReducerMap<State> = {
    user: user.userReducer,
    fileManagement: fileSystem.fileManagementReducers,
};

export const metaReducers: MetaReducer<State>[] = [];
