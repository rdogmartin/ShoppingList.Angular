import { Injectable } from '@angular/core';
import { NotificationService } from '../services/notification.service';

@Injectable({
  providedIn: 'root',
})
export class ErrorHandlerService {
  public constructor(private notificationService: NotificationService) {}

  /**
   * Show the user an error message and log the specified message or error.
   * @param error A string or Error object
   */
  public handleError(error: string | Error | object): void {
    this.notificationService.showError(this.getNotificationMessage(error));
  }

  private getNotificationMessage(error: string | Error | object) {
    let message = 'Oops, there was an error';

    if (typeof error === 'string') {
      message = error;
    } else if (Object.prototype.hasOwnProperty.call(error, 'message')) {
      message = (error as { message: string }).message;
    }

    return message;
  }
}
