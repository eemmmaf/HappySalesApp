using System.Linq;
using HappySalesApp.Data;
using HappySalesApp.Models.HappySales.Models;
using Microsoft.EntityFrameworkCore;

namespace HappySalesApp.Models
{
    public class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Kontrollera om kategoritabellen är tom
            if (!context.Categories.Any())
            {
                // Skapa några kategorier
                var categories = new Category[]
                {
                    new Category{CategoryName="Lampor", CategoryDescription="Lampor är en viktig del av inredningen och kan användas för att skapa en varm och inbjudande atmosfär i hemmet. Oavsett om du letar efter en lampa för att skapa en mysig stämning i vardagsrummet eller för att ge extra ljus till ditt arbetsrum, hoppas vi att vi har något för dig bland lamporna"},
                    new Category{CategoryName="Soffor", CategoryDescription="Soffan är utan tvekan en av de viktigaste möblerna i hemmet. Det är en plats där man kan koppla av och umgås med familj och vänner. På vår köp och sälj-sida har vi ett stort utbud av soffor i olika stilar, storlekar och färger för att passa alla behov och önskemål. Tveka inte att kolla in vårt utbud av soffor och hitta den perfekta möbeln för att skapa en bekväm och avslappnad atmosfär i ditt hem. Och om du har en soffa som du vill sälja, så kan du enkelt lägga upp den på vår sida och få den såld till rätt pris."},
                    new Category{CategoryName="Kök", CategoryDescription="Köket är hjärtat i hemmet där matlagning och samvaro ofta äger rum. På vår köp och sälj-sida kan du hitta allt du behöver för att skapa ditt drömkök. Vi har ett brett utbud av köksmöbler och köksredskap för att hjälpa dig att skapa en praktisk och stilren köksmiljö. "},
                    new Category{CategoryName="Utemöbler", CategoryDescription="Utemöbler är en viktig del av trädgårds- och uteplatsinredningen och kan hjälpa dig att skapa en bekväm och stilren utomhusmiljö. På vår köp och sälj-sida kan du hitta allt du behöver för att göra din trädgård eller uteplats till en avkopplande och trivsam plats att vara på. Vi har ett brett utbud av utemöbler, inklusive trädgårdsbord, stolar, solstolar och loungemöbler, för att passa alla smaker och önskemål."},
                    new Category{CategoryName="Textilier", CategoryDescription="Textilier är en viktig del av inredningen och kan hjälpa till att skapa en varm och inbjudande atmosfär i ditt hem. På vår köp och sälj-sida har vi ett brett utbud av textilier, inklusive kuddar, gardiner, mattor och överkast, för att hjälpa dig att förbättra ditt hem på ett snabbt och enkelt sätt. Vi har ett stort utbud av olika färger, mönster och texturer för att passa alla stilar och smaker. Oavsett om du söker efter en mjuk och mysig pläd för att krypa upp i soffan med eller en snygg gardin för att liva upp ditt vardagsrum, så hittar du det på vår sida."},
                    new Category{CategoryName="Handdukar", CategoryDescription="Handdukar är en nödvändig del av badrummet och köket. Vi har ett brett utbud av högkvalitativa handdukar tillgängliga på vår köp och sälj-sida. Låt oss hjälpa dig att hitta de bästa handdukarna för dina behov och göra ditt badrum till en bekväm och avslappnad plats."},
                    new Category{CategoryName="Ljus & Ljushållare", CategoryDescription="Ljus och ljushållare är en viktig del av inredningen och kan hjälpa till att skapa en varm och inbjudande atmosfär i ditt hem. Låt oss hjälpa dig att skapa en avslappnad och välkomnande atmosfär med vårt stora utbud av ljus och ljushållare."},
                    new Category{CategoryName="Tavlor & Posters", CategoryDescription="Tavlor och posters är ett fantastiskt sätt att förbättra inredningen i ditt hem. Låt oss hjälpa dig att hitta den perfekta tavlan eller postern för att förbättra inredningen i ditt hem och uttrycka din personliga stil."},
                    new Category{CategoryName="Krukor & Vaser", CategoryDescription="Låt oss hjälpa dig att hitta den perfekta tavlan eller postern för att förbättra inredningen i ditt hem och uttrycka din personliga stil. Låt oss hjälpa dig att skapa en frisk och avkopplande miljö med vårt stora utbud av växter"},
                    new Category{CategoryName="Sängar", CategoryDescription="En god natts sömn är avgörande för hälsan och välbefinnandet. Låt oss hjälpa dig att hitta den perfekta sängen för att förbättra din sömnkvalitet och ditt övergripande välbefinnande."},
                    new Category{CategoryName="Förvaring", CategoryDescription="Förvaring är en viktig faktor i att hålla ditt hem organiserat och rent. Låt oss hjälpa dig att hålla ditt hem snyggt och organiserat med vårt utbud av förvaringsmöbler och tillbehör."},
                    new Category{CategoryName="Bokhyllor", CategoryDescription="Bokhyllor är inte bara ett praktiskt sätt att lagra och organisera böcker, utan också en stilfull möbel som kan ge ditt hem en personlig touch. Låt oss hjälpa dig att visa upp dina böcker och dekorativa föremål på ett stilfullt och praktiskt sätt med vårt utbud av bokhyllor"},
                    new Category{CategoryName="Matsal", CategoryDescription="Matsalen är ett centralt rum i många hem, där familj och vänner kan samlas för att njuta av måltider tillsammans. Låt oss hjälpa dig att skapa en funktionell och vacker matsal där du kan samla dina nära och kära för minnesvärda måltider."},
                    new Category{CategoryName="Barn", CategoryDescription="Att inreda barnrum kan vara en rolig och kreativ uppgift, men det är också viktigt att hitta möbler och dekorationer som är både säkra och praktiska för ditt barns behov. Låt oss hjälpa dig att skapa en trygg, bekväm och rolig miljö för ditt barn med vårt utbud av barnsaker"},
                    new Category{CategoryName="Övrigt", CategoryDescription="Ibland kan det hända att du letar efter något som inte passar in i någon av våra andra kategorier. Då är kategorin Övrigt platsen för dig! Här hittar du ett brett utbud av produkter som inte passar in i någon annan kategori, men som fortfarande kan vara precis vad du söker. Så om du söker efter något speciellt eller unikt, ta en titt i vår kategori Övrigt - du vet aldrig vad du kan hitta där!"}
                };

                // Lägg till kategorierna i databasen
                foreach (Category c in categories)
                {
                    context.Categories.Add(c);
                }

                context.SaveChanges();
            }

        }


    }
}
