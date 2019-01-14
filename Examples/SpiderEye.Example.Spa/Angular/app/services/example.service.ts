import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ExampleService {

    private serverUrl = 'api/v1/Example/';

    constructor(private http: HttpClient) {
    }

    getExample(name: string): Observable<string> {
        return this.http.get<string>(this.serverUrl + 'GetExample', {
            params: { name: name }
        });
    }

    getSlowExample(name: string): Observable<string> {
        return this.http.get<string>(this.serverUrl + 'GetSlowExample', {
            params: { name: name }
        });
    }
}
