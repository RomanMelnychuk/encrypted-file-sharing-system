import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { BreadcrumbModule } from 'primeng/breadcrumb';

@Component({
  selector: 'fg-folder-breadcrumb',
  standalone: true,
  imports: [CommonModule, BreadcrumbModule],
  templateUrl: './folder-breadcrumb.component.html',
})
export class FolderBreadcrumbComponent implements OnInit {
    @Input() topMenu: MenuItem | undefined = undefined;

    menuItems: MenuItem[] = []

    constructor(private route: ActivatedRoute, private router: Router) {}

    ngOnInit(): void {
        this.route.queryParams.subscribe(params => {
            const folderName = params['folderName'] || null;
            const folderId = params['folderId'] || null;

            if (folderName == null || folderId == null) {
                this.router.navigate([this.topMenu?.routerLink]);
                this.menuItems = [];
                return;
            }

            const existingIndex = this.menuItems.findIndex((item) => {
                if (item?.queryParams){
                    const folderIdParam = item.queryParams['folderId'];

                    return folderIdParam === folderId
                }

                return false;
            });

            if (existingIndex !== -1) {
                this.menuItems = this.menuItems.slice(0, existingIndex + 1);
            } else {
                const newItem: MenuItem = {
                    label: folderName,
                    icon: 'pi pi-folder',
                    routerLink: [this.topMenu?.routerLink],
                    queryParams: { folderId, folderName },
                };

                this.menuItems = [...this.menuItems, newItem];
            }
        });
    }
}
