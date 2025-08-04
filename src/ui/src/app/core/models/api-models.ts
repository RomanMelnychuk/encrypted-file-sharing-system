// ..\services\FileGuard.Identity.Application\DTOs\FileDto.cs
export interface FileDto {
    id: string;
    fileName: string;
    path: string;
    createdAt: string;
    sizeInBytes: number;
    folderId?: string;
}

// ..\services\FileGuard.Identity.Application\DTOs\FolderDto.cs
export interface FolderDto {
    id: string;
    name: string;
    parentFolderId?: string;
    createdAt: string;
    sizeInBytes?: number;
    subFolders: FolderDto[];
    files: FileDto[];
}

// ..\services\FileGuard.Identity.Application\DTOs\UserDto.cs
export interface UserDto {
    id: string;
    userName: string;
    email: string;
}

// ..\services\FileGuard.Identity.Application\DTOs\UserFileSystemDto.cs
export interface FileSection {
    folders: FolderDto[];
    files: FileDto[];
}

// ..\services\FileGuard.Identity.Application\DTOs\UserFileSystemDto.cs
export interface UserFileSystemDto {
    myFiles: FileSection;
    sharedWithMe: FileSection;
    sharedByMe: FileSection;
}

// ..\services\FileGuard.API\Models\ErrorModel.cs
export interface ErrorModel {
    status: number;
    message: string;
    errorCode: string;
}

// ..\services\FileGuard.API\Models\Records.cs
export interface CreateFolderRequest {
    folderName: string;
    parentFolderId?: string;
}
