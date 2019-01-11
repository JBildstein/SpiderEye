
declare global {
    interface Window {
        _spidereye: SpiderEyeInterface;
    }
}

export interface SpiderEyeInterface {
    updateTitle(title: string): void;
    invokeApi<T = any, U = any>(id: string, parameters: T, callback: (result: ApiResult<U>) => void): void;
    addEventHandler<T = any, U = void>(name: string, callback: (value?: T) => U): void;
    removeEventHandler(name: string): void;

    _endApiCall<T>(callbackId: number, result: ApiResult<T>): void;
    _sendEvent<T>(name: string, value: T): string;
}

export interface ApiResult<T> {
    value: T;
    success: boolean;
    error: string;
}
