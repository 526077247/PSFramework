import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import {
  MatAutocompleteModule,
  MatButtonModule, MatCardModule,
  MatCheckboxModule, MatDividerModule, MatExpansionModule,
  MatFormFieldModule,
  MatIconModule,
  MatInputModule, MatMenuModule, MatOptionModule,
  MatRippleModule, MatSelectModule, MatSliderModule,
  MatSnackBarModule, MatTabsModule,
  MatTreeModule
} from '@angular/material';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ForgetComponent } from './components/forget/forget.component';
import { ResetComponent } from './components/reset/reset.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ForgetComponent,
    ResetComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    MatAutocompleteModule,
    MatButtonModule, MatCardModule,
    MatCheckboxModule, MatDividerModule, MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule, MatMenuModule, MatOptionModule,
    MatRippleModule, MatSelectModule, MatSliderModule,
    MatSnackBarModule, MatTabsModule,
    DragDropModule,
  ],
  providers: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule {

}
