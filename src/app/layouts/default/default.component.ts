import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';

@Component({
    selector: 'app-default',
    imports: [CommonModule, RouterOutlet, HeaderComponent, FooterComponent],
    templateUrl: './default.component.html',
    styleUrl: './default.component.scss'
})
export class DefaultComponent {}
