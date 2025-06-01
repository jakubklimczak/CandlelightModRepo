export interface ModVersionUploadForm {
  modId: string;
  version: string;
  changelog?: string;
  file: File;
}
