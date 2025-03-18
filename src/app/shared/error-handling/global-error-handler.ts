import { ErrorHandler, Inject, Injectable, Injector } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ErrorHandlerService } from './error-handler.service';

@Injectable({
  providedIn: 'root',
})
export class GlobalErrorHandler implements ErrorHandler {
  private get toastr() {
    return this.injector.get(ToastrService);
  }

  public constructor(
    private errorHandlerService: ErrorHandlerService,
    @Inject(Injector) private readonly injector: Injector,
  ) {}

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  public handleError(err: any): void {
    // eslint-disable-next-line @typescript-eslint/no-unsafe-argument
    this.errorHandlerService.handleError(err);
  }
}
