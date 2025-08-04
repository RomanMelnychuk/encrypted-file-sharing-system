import { createReducer, on } from '@ngrx/store';
import { loadUser, loadUserSuccess, loadUserFailure } from './user.actions';
import { User } from '../../core/models';

export interface UserState {
    user: User | null;
    error: string | null;
    loading: boolean;
}

export const initialState: UserState = {
    user: null,
    error: null,
    loading: false,
};

export const userReducer = createReducer(
    initialState,
    on(loadUser, (state) => ({ ...state, loading: true })),
    on(loadUserSuccess, (state, { user }) => ({ ...state, loading: false, user })),
    on(loadUserFailure, (state, { error }) => ({ ...state, loading: false, error }))
);
