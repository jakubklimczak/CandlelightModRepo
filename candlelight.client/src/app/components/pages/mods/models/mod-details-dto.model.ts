import { ModVersion } from "./mod-version.model";

export interface ModDetailsDto {
    id: string;
    gameId: string;
    authorUsername: string;
    name: string;
    thumbnailUrl: string;
    description: string;
    gameName: string;
    versions: ModVersion[];
    createdBy: string;
    lastUpdatedAt: Date;
    createdAt: Date;
}