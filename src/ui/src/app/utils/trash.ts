export const getFileIcon = (fileName: string, fileType: 'file' | 'folder'): string => {
    if (fileType === 'folder') return 'pi pi-folder';

    if (fileType === 'file') {
        const extension = fileName.split('.').pop()?.toLowerCase();
        switch (extension) {
            case 'jpg':
            case 'jpeg':
            case 'png':
            case 'gif':
            case 'bmp':
                return 'pi pi-image';
            case 'pdf':
                return 'pi pi-file-pdf';
            case 'doc':
            case 'docx':
                return 'pi pi-file-word';
            case 'xls':
            case 'xlsx':
                return 'pi pi-file-excel';
            case 'ppt':
            case 'pptx':
                return 'pi pi-file-powerpoint';
            case 'txt':
                return 'pi pi-file-text';
            case 'zip':
            case 'rar':
                return 'pi pi-file-archive';
            default:
                return 'pi pi-file';
        }
    }

    return 'pi pi-file';
};

export const formatSize = (sizeInBytes: number): string => {
    if (sizeInBytes >= 1e9) return (sizeInBytes / 1e9).toFixed(2) + ' GB';
    if (sizeInBytes >= 1e6) return (sizeInBytes / 1e6).toFixed(2) + ' MB';
    if (sizeInBytes >= 1e3) return (sizeInBytes / 1e3).toFixed(2) + ' KB';
    return sizeInBytes + ' B';
};
