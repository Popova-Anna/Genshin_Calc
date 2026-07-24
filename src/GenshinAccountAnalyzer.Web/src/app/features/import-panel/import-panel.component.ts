import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AccountStoreService } from '../../core/services/account-store.service';
import { LanguageService } from '../../core/services/language.service';

/** Lets the user pick (or drag-and-drop) an Enka.Network export JSON file and analyze it. */
@Component({
  selector: 'app-import-panel',
  imports: [],
  templateUrl: './import-panel.component.html',
  styleUrl: './import-panel.component.scss',
})
export class ImportPanelComponent {
  private readonly store = inject(AccountStoreService);
  private readonly router = inject(Router);
  protected readonly language = inject(LanguageService);

  protected readonly dragOver = signal(false);
  protected readonly loading = this.store.loading;
  protected readonly error = this.store.error;

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.dragOver.set(true);
  }

  onDragLeave(): void {
    this.dragOver.set(false);
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.dragOver.set(false);
    const file = event.dataTransfer?.files?.[0];
    if (file) {
      void this.importFile(file);
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (file) {
      void this.importFile(file);
    }
    input.value = '';
  }

  private async importFile(file: File): Promise<void> {
    try {
      await this.store.load(file);
      await this.router.navigate(['/characters']);
    } catch {
      // The error is already surfaced via store.error(); nothing further to do here.
    }
  }
}
