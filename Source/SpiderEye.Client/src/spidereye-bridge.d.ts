
declare global {
    interface Window {
        _spidereye: SpiderEyeBridge;
    }
}

export type ApiCallback<T> = (result: ApiResult<T>) => void;
export type EventCallback<TResult = any, TParam = any> = (value?: TParam) => TResult;

export interface SpiderEyeBridge {
    updateTitle(title: string): void;
    invokeApi<TResult = any, TParam = any>(id: string, parameters: TParam, callback: ApiCallback<TResult>): void;
    addEventHandler<TResult = any, TParam = any>(name: string, callback: EventCallback<TResult, TParam>): void;
    removeEventHandler(name: string): void;

    _endApiCall<T>(callbackId: number, result: ApiResult<T>): void;
    _sendEvent<T>(name: string, value: T): string;
}

export interface ApiResult<T> {
    value: T;
    success: boolean;
    error: string;
}
