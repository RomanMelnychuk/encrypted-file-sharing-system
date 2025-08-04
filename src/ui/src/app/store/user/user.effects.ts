import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, map, of, switchMap } from 'rxjs';
import { loadUser, loadUserSuccess, loadUserFailure } from './user.actions';
import { User } from '../../core/models';
import { UserService } from '../../core/services/user.service';

@Injectable({
    providedIn: 'root',
})
export class UserEffects {
    constructor(private actions$: Actions, private userService: UserService) {}

    loadUser$ = createEffect(() =>
        this.actions$.pipe(
            ofType(loadUser),
            switchMap(() =>
                this.userService.get().pipe(
                    map((user: User) => loadUserSuccess({ user })),
                    catchError((error) => of(loadUserFailure({ error })))
                )
            )
        )
    );
}
