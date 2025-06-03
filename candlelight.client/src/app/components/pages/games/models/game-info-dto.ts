export interface GameInfoDto {
    id: string;
    appId?: number;
    name: string;
    headerImage?: string;
    developer?: string;
    publisher?: string;
    description?: string;
    isCustom: boolean;
    createdBy?: string;
}
