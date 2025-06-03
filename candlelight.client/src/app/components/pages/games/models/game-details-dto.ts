export interface Genre {
  id: string;
  name: string;
}

export interface Category {
  id: string;
  name: string;
}

export interface Platform {
  id: string;
  name: string;
}

export interface GameDetailsDto {
  id: string;
  appId: number | null;
  name: string;
  descriptionSnippet?: string;
  description?: string;
  headerImage?: string;
  developer?: string;
  publisher?: string;
  isFree?: boolean;
  price?: number;
  currency?: string;
  metacriticScore?: number;
  website?: string;
  releaseDate?: string;
  modCount: number;
  favouriteCount: number;
  isCustom: boolean;
  genres: Genre[];
  categories: Category[];
  platforms: Platform[];
}
