import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { BaseApiService } from './base-api.service';
import { Observable, of } from 'rxjs';
import { CreateFolderRequest, FileDto, FileSection, FolderDto, UserFileSystemDto } from '../models/api-models';

@Injectable({
    providedIn: 'root',
})
export class FileManagementService extends BaseApiService {
    fileManagementUrl = `${this.baseUrl}/FileManagement`;

    constructor(http: HttpClient, cookies: CookieService) {
        super(http, cookies);
    }

    getFolder(folderId: string): Observable<FileSection>{
        return this.http.get<FileSection>(`${this.fileManagementUrl}/folder/${folderId}`, this.options);
    }

    getUserTopLevelFolder(): Observable<UserFileSystemDto> {
        return this.http.get<UserFileSystemDto>(this.fileManagementUrl, this.options);
    }

    createFolder(folder: CreateFolderRequest): Observable<FolderDto> {
        return this.http.post<FolderDto>(`${this.fileManagementUrl}/folder`, folder, this.options);
    }

    deleteFolder(folderId: string): Observable<void> {
        return this.http.delete<void>(`${this.fileManagementUrl}/folder/${folderId}`, this.options);
    }

    deleteFile(fileId: string): Observable<void> {
        return this.http.delete<void>(`${this.fileManagementUrl}/file/${fileId}`, this.options);
    }

    downloadFile(fileId: string): Observable<Blob> {
        return this.http.get(`${this.fileManagementUrl}/file/${fileId}`, {
            ...this.options,
            responseType: 'blob',
        });
    }

    uploadFiles(folderId: string | null, files: File[]): Observable<FileDto[]> {
        const formData: FormData = new FormData();

        files.forEach((file) => {
            formData.append('files', file, file.name);
        });

        const url = folderId ? `${this.fileManagementUrl}/files?folderId=${folderId}` : `${this.fileManagementUrl}/files`

        return this.http.post<FileDto[]>(url, formData, {
            ...this.options,
        });
    }

    shareFile(fileId: string, userId: string): Observable<void> {
        return this.http.post<void>(`${this.fileManagementUrl}/file/${fileId}/share/${userId}`, {}, this.options);
    }

    shareFolder(folderId: string, userId: string): Observable<void> {
        return this.http.post<void>(`${this.fileManagementUrl}/folder/${folderId}/share/${userId}`, {}, this.options);
    }

    searchFilesAndFolders(searchTerm: string): Observable<FileSection> {
        return this.http.get<FileSection>(`${this.fileManagementUrl}/search?searchTerm=${searchTerm}`, this.options);
    }
}
