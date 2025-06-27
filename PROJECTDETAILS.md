
In home page:
- Generic Search will search movie title, summaries and actors.
o Add a drop-down search like above to be specific on the search.
o On first three letters, show maximum of 3 top items that satisfy the search text
- Show a list of at least 10 movies in the opening page (with a slider) with IMDB scores,
image and a trailer
- If not logged in, login will be visible at the header.
- From top right, user can change language to English/Turkish. Only Home page should be
in 2 languages EN/TR. Default should be browser language

In login page:
- Login with email/password or Google will be supported.
- User can register with just email, password (at least 8 characters, 1 number and 1 none
alphanumeric character) and country, city. Photo upload needs to be supported but is
optional for registration. User passwords will be stored with one way hashing
-
- If google authenticated or after manual registration, user will be directed to HOME
PAGE.
o Header will have the user name
o Users will be able to add movies to watch list and rate/comment on movies
o When NOT logged in, users clicking on Add to Watchlist, Rate will be directed to
login page

- Search results will be displayed in different Titles and People sections (if there are any hits) 


In movie/show detail page:
- One image and one clip of the movie
- Rating of the movie
o When clicked on rating, it will show a graphs that the rating distribution by
country and the comments/ratings that calculated the rating of the movie
- Popularity of the movie. This is a business logic you will need to create based on research
and some assumptions. Generally, popularity is a mix of ratings, comments, page views
etc. Also, you need to show ranking in popularity