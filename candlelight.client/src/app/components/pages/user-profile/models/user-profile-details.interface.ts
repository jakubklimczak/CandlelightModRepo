export interface UserProfileDetails {
    id: string;
    displayName: string;
    avatarFilename: string | null;
    bio: string | null;
    backgroundColour: string | null;
    steamId: string | null;
    createdAt: Date;
    email: string;
    favouritesVisible: boolean;
}