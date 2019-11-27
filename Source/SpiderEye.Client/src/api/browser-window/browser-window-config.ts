
export interface BrowserWindowConfig {
    title?: string;
    width?: number;
    height?: number;
    minWidth?: number;
    minHeight?: number;
    maxWidth?: number;
    maxHeight?: number;
    backgroundColor?: string;
    canResize?: boolean;
    useBrowserTitle?: boolean;
    enableScriptInterface?: boolean;
    enableDevTools?: boolean;
    url: string;
}
