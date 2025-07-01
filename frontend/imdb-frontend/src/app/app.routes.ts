import { Routes } from '@angular/router';
import { HomepageComponent } from './components/homepage/homepage.component';
import { LoginComponent } from './components/auth/login/login.component';
import { SignupComponent } from './components/auth/signup/signup.component';
import { MovieDetailComponent } from './components/movie-detail/movie-detail.component';
import { SearchResultPageComponent } from './components/search-result-page/search-result-page.component';
import { RatingDetailPageComponent } from './components/rating-detail-page/rating-detail-page.component';

export const routes: Routes = [
  { path: '', component: HomepageComponent },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'movie/:id', component: MovieDetailComponent },
  { path: 'search', component: SearchResultPageComponent },
  { path: 'movie/:id/ratings', component: RatingDetailPageComponent },
  { path: '**', redirectTo: '' }
];
