import { createSelector, createFeatureSelector } from '@ngrx/store';
import { UserState } from './user.reducer';

export const selectUserState = createFeatureSelector<UserState>('user');

export const selectCurrentUser = createSelector(selectUserState, (state) => state.user);

export const selectIsLoading = createSelector(selectUserState, (state) => state.loading);

export const selectError = createSelector(selectUserState, (state) => state.error);
