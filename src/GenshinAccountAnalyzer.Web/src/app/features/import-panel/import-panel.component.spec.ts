import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { AccountStoreService } from '../../core/services/account-store.service';
import { ImportPanelComponent } from './import-panel.component';

describe('ImportPanelComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ImportPanelComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
  });

  it('shows the drop-zone prompt when idle', () => {
    const fixture = TestBed.createComponent(ImportPanelComponent);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Drop your Enka.Network export here');
  });

  it('calls the store to load the account when a file is selected', () => {
    const store = TestBed.inject(AccountStoreService);
    spyOn(store, 'load').and.returnValue(Promise.resolve());
    const fixture = TestBed.createComponent(ImportPanelComponent);
    fixture.detectChanges();

    const file = new File(['{}'], 'sample.json', { type: 'application/json' });
    const input = document.createElement('input');
    Object.defineProperty(input, 'files', { value: [file] });
    fixture.componentInstance.onFileSelected({ target: input } as unknown as Event);

    expect(store.load).toHaveBeenCalledWith(file);
  });

  it('toggles the drag-over state on dragover/dragleave', () => {
    const fixture = TestBed.createComponent(ImportPanelComponent);
    fixture.detectChanges();
    const component = fixture.componentInstance;

    component.onDragOver({ preventDefault: () => undefined } as DragEvent);
    expect((component as unknown as { dragOver: () => boolean }).dragOver()).toBeTrue();

    component.onDragLeave();
    expect((component as unknown as { dragOver: () => boolean }).dragOver()).toBeFalse();
  });
});
