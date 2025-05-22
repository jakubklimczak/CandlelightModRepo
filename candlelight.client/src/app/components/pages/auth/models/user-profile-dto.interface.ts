export interface UserProfileDto {
    profileId: string;
    userId: string;
    displayName: string;
    bio?: string;
    avatarFilename?: string;
    createdAt: Date;
    lastUpdatedAt: Date;
    backgroundColour?: string;
}