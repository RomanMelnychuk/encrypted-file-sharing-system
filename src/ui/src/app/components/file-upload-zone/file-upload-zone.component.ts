import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MessageService, PrimeNGConfig} from 'primeng/api';
import { FileSelectEvent, FileUploadEvent, FileUploadModule } from 'primeng/fileupload';
import { ButtonModule } from 'primeng/button';
import { CommonModule } from '@angular/common';
import { BadgeModule } from 'primeng/badge';
import { ProgressBarModule } from 'primeng/progressbar';
import { ToastModule } from 'primeng/toast';

interface UploadEvent {
    originalEvent: Event;
    files: File[];
}

@Component({
  selector: 'fg-file-upload-zone',
  standalone: true,
  imports: [FileUploadModule, ButtonModule, BadgeModule, ProgressBarModule, ToastModule, CommonModule],
  templateUrl: './file-upload-zone.component.html',
  styleUrl: './file-upload-zone.component.scss',
  providers: [MessageService]
})
export class FileUploadZoneComponent {
    uploadedFiles: any[] = [];

    @Input() files: File[] = []
    @Output() onFileAdd = new EventEmitter();


    constructor(private messageService: MessageService) {}

    onUpload(event: FileUploadEvent) {
        for(let file of event.files) {
            this.uploadedFiles.push(file);
        }

        this.messageService.add({severity: 'info', summary: 'File Uploaded', detail: ''});
    }

    onFileSelect(event: FileSelectEvent) {
        this.onFileAdd.emit(event.currentFiles);
    }
}
