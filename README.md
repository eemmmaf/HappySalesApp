# HappySalesApp
Detta projekt har skapats i kursen DT191G Datateknik GR (B), Webbutveckling med .NET. Det är en ASP.NET Core Web App med Model, Views och Controllers.

## Om Happy Sales
Happy Sales är en webbapplikation som används för att användare ska kunna lägga upp annonser med sin använda inredning. Andra användare kan lägga bud på annonserna och skaparen kan godkänna bud. 

## Struktur
Detta är en MVC-applikation och innehåller Models, Controllers och Views. Det finns en del JavaScript-kod för den responsiva menyn. Controllers och Views har scaffoldats fram med EF Core. Användarhantering har scaffoldats fram med ramverket Identity.

### Models
Projektet innehåller tre modeller. En för buden, en för annonser och en för kategorier. Modellerna går att hitta i mappen Models.cs och i filen Products.cs.

#### SeedData
SeedData är en klass som innehåller seedning av kategorier. Om tabellen för kategorier är tom lagras 15st kategorier med namn och beskrivning.

### Controllers
Controllers för bud och annonser har scaffoldats fram utifrån modellerna Bids och Products. De controllers som finns är:
* BidsController - Logiken bakom buden
* ProductsController - Logiken bakom annonserna
* HomeController - Returnerar about-sidan, startsidan och logiken bakom sök-funktionaliteten. 

### Views
Views scaffoldades fram i samband med att Controllers scaffoldades fram. De kataloger med olika views som finns är:
* Bids
* Home
* Products

## Databas-anslutning
Detta projekt använder en MySQL-databasanslutning. En databas skapades för detta projekt och en anslutningssträng och anslutning skapades.

Mvh
Emma Forslund
emfo2102@student.miun.se
