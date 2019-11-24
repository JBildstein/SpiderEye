import { Injectable, NgZone } from '@angular/core';
import { Observable, from, Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { SpiderEye } from 'spidereye';

@Injectable({
    providedIn: 'root'
})
export class SpiderEyeService {

    constructor(private zone: NgZone) {
    }

    invokeApi<TResult = any, TParams = any>(id: string, params?: TParams): Observable<TResult> {
        return from(SpiderEye.invokeApiAsync(id, params))
            .pipe(map(t => {
                if (!t.success) {
                    throw new Error(t.error);
                }

                return t.value;
            }));
    }

    registerHandler<T>(name: string): Observable<T> {
        const subject = new Subject<T>();

        // note: need to call zone.run because callback is outside of angular zone
        SpiderEye.addEventHandler<void, T>(name, t => this.zone.run(() => subject.next(t)));

        return subject.asObservable();
    }

    removeHandler(name: string) {
        SpiderEye.removeEventHandler(name);
    }
}
