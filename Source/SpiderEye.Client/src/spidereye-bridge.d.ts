
declare global {
    interface Window {
        _spidereye: SpiderEyeBridge;
    }
}

export type ApiCallback<T> = (result: ApiResult<T>) => void;
export type EventCallback<T = any, U = void> = (value?: T) => U;

export interface SpiderEyeBridge {
    updateTitle(title: string): void;
    invokeApi<T = any, U = any>(id: string, parameters: T, callback: ApiCallback<U>): void;
    addEventHandler<T = any, U = void>(name: string, callback: EventCallback<T, U>): void;
    removeEventHandler(name: string): void;

    _endApiCall<T>(callbackId: number, result: ApiResult<T>): void;
    _sendEvent<T>(name: string, value: T): string;
}

export interface ApiResult<T> {
    value: T;
    success: boolean;
    error: string;
}
