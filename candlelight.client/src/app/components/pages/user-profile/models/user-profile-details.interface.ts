export interface UserProfileDetails {
    id: string;
    displayName: string;
    avatarFilename: string;
    bio: string | null;
    backgroundColour: string | null;
    steamId: string | null;
    createdAt: Date;
    email: string;
}