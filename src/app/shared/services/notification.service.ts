import { Inject, Injectable, Injector } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  public constructor(@Inject(Injector) private readonly injector: Injector) {}

  private get toastr() {
    return this.injector.get(ToastrService);
  }

  public showSuccess(message: string, title?: string) {
    this.toastr.success(message, title, {
      onActivateTick: true,
      enableHtml: true,
      positionClass: 'toast-top-full-width',
      closeButton: true,
    });
  }

  public showError(message: string, title?: string) {
    this.toastr.error(message, title, {
      onActivateTick: true,
      enableHtml: true,
      positionClass: 'toast-top-full-width',
      closeButton: true,
      disableTimeOut: true,
    });
  }

  public showInfo(message: string, title?: string) {
    this.toastr.info(message, title, {
      onActivateTick: true,
      enableHtml: true,
      positionClass: 'toast-top-full-width',
      closeButton: true,
    });
  }

  public showWarning(message: string, title?: string) {
    this.toastr.warning(message, title, {
      onActivateTick: true,
      enableHtml: true,
      positionClass: 'toast-top-full-width',
      closeButton: true,
    });
  }
}
