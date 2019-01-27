
export class FileFilter {
    public readonly name: string;
    public readonly filters: string[];

    constructor(name: string, ...filters: string[]) {
        if (name == null) {
            throw new Error("No name provided");
        }

        this.name = name;
        this.filters = filters || [];
    }
}
