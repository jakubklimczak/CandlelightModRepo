export interface ModsListItemDto {
  id: string;
  name: string;
  descriptionSnippet: string;
  thumbnailUrl: string;
  authorId: string;
  author: string;
  lastUpdatedDate: Date;
  totalDownloads: number;
  averageRating: number;
  totalFavourited: number;
  totalReviews: number;
}
