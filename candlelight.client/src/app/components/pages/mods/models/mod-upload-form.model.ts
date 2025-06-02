export interface ModUploadForm {
    name: string;
    descriptionSnippet?: string;
    description?: string;
    version: string;
    changelog?: string;
    gameId: string;
    supportedVersions?: string[];
    dependencies?: string[];
    file: File;
    images?: File[];
    selectedThumbnail?: string;
}
