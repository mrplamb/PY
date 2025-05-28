using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HolidayManagerWeb.Models; // Make sure this path is correct for your models
using HolidayManagerWeb; // Assuming AppDbContext and other core classes are here
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; // Required for List<T>

namespace Final
{
    public partial class AddTrips : Window
    {
        private readonly AppDbContext _db;
        private User _currentUser;

        // NEW: Collection for suggested cities
        public ObservableCollection<CitySuggestion> SuggestedCities { get; set; }

        public AddTrips()
        {
            InitializeComponent();
            _db = new AppDbContext();
            SuggestedCities = new ObservableCollection<CitySuggestion>(); // Initialize

            // Bind the DestinationComboBox to the SuggestedCities collection
            DestinationComboBox.ItemsSource = SuggestedCities;

            LoadUserData();
            LoadSuggestedCities(); // Load suggested cities
        }

        private async void LoadUserData()
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User session not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            _currentUser = await _db.Users
                                    .FirstOrDefaultAsync(u => u.ID == AppState.CurrentUser.ID);

            if (_currentUser == null)
            {
                MessageBox.Show("User data could not be loaded from the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }
        }

        // NEW: Method to load suggested cities and their activities/restaurants
        private void LoadSuggestedCities()
        {
            SuggestedCities.Clear();

            // --- Madrid, Spain ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Madrid, Spain",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Prado Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187514-d190130-Reviews-Prado_National_Museum-Madrid.html" },
                    new PlaceDetail { Name = "Retiro Park", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187514-d190132-Reviews-Parque_del_Retiro-Madrid.html" },
                    new PlaceDetail { Name = "Royal Palace", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187514-d190133-Reviews-Royal_Palace_of_Madrid-Madrid.html" },
                    new PlaceDetail { Name = "Plaza Mayor", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187514-d190134-Reviews-Plaza_Mayor-Madrid.html" },
                    new PlaceDetail { Name = "Temple of Debod", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187514-d242784-Reviews-Temple_of_Debod-Madrid.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "San Miguel Market", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187514-d738096-Reviews-Mercado_de_San_Miguel-Madrid.html" },
                    new PlaceDetail { Name = "Sobrino de Botín", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187514-d738072-Reviews-Sobrino_de_Botin-Madrid.html" },
                    new PlaceDetail { Name = "Chocolateria San Ginés", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187514-d738071-Reviews-Chocolateria_San_Gines-Madrid.html" }
                }
            });

            // --- Paris, France ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Paris, France",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Eiffel Tower", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187147-d188151-Reviews-Eiffel_Tower-Paris_Ile_de_France.html" },
                    new PlaceDetail { Name = "Louvre Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187147-d188151-Reviews-Louvre_Museum-Paris_Ile_de_France.html" },
                    new PlaceDetail { Name = "Notre Dame Cathedral", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187147-d188151-Reviews-Cathedrale_Notre_Dame_de_Paris-Paris_Ile_de_France.html" },
                    new PlaceDetail { Name = "Arc de Triomphe", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187147-d188151-Reviews-Arc_de_Triomphe-Paris_Ile_de_France.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "Le Relais de l'Entrecôte", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187147-d719234-Reviews-Le_Relais_de_l_Entrecote-Paris_Ile_de_France.html" },
                    new PlaceDetail { Name = "Pierre Hermé (macarons)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187147-d1170138-Reviews-Pierre_Herme_Paris-Paris_Ile_de_France.html" }
                }
            });

            // --- Rome, Italy ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Rome, Italy",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Colosseum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187791-d192285-Reviews-Colosseum-Rome_Lazio.html" },
                    new PlaceDetail { Name = "Vatican City", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187791-d192285-Reviews-Vatican_City-Rome_Lazio.html" },
                    new PlaceDetail { Name = "Trevi Fountain", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187791-d192285-Reviews-Trevi_Fountain-Rome_Lazio.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "Trattoria Da Enzo al 29", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187791-d1089228-Reviews-Trattoria_Da_Enzo_al_29-Rome_Lazio.html" },
                    new PlaceDetail { Name = "Roscioli Salumeria", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187791-d1101905-Reviews-Salumeria_Roscioli-Rome_Lazio.html" }
                }
            });

            // --- London, UK ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "London, UK",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "British Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186338-d188092-Reviews-The_British_Museum-London_England.html" },
                    new PlaceDetail { Name = "Tower of London", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186338-d191295-Reviews-Tower_of_London-London_England.html" },
                    new PlaceDetail { Name = "Buckingham Palace", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186338-d191295-Reviews-Buckingham_Palace-London_England.html" },
                    new PlaceDetail { Name = "London Eye", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186338-d218206-Reviews-Coca_Cola_London_Eye-London_England.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "Borough Market", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186338-d218206-Reviews-Borough_Market-London_England.html" },
                    new PlaceDetail { Name = "Dishoom Covent Garden", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g186338-d218206-Reviews-Dishoom_Covent_Garden-London_England.html" }
                }
            });

            // --- Berlin, Germany ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Berlin, Germany",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Brandenburg Gate", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187323-d191295-Reviews-Brandenburg_Gate-Berlin.html" },
                    new PlaceDetail { Name = "Reichstag Building", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187323-d207436-Reviews-Reichstag_Building-Berlin.html" },
                    new PlaceDetail { Name = "East Side Gallery", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187323-d284797-Reviews-East_Side_Gallery-Berlin.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "Mustafas Gemüsekebap", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187323-d1000632-Reviews-Mustafas_Gemuesekebap-Berlin.html" },
                    new PlaceDetail { Name = "Curry 36", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187323-d716896-Reviews-Curry_36-Berlin.html" }
                }
            });

            // --- Amsterdam, Netherlands ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Amsterdam, Netherlands",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Anne Frank House", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g188590-d191295-Reviews-Anne_Frank_House-Amsterdam_North_Holland.html" },
                    new PlaceDetail { Name = "Rijksmuseum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g188590-d191295-Reviews-Rijksmuseum-Amsterdam_North_Holland.html" },
                    new PlaceDetail { Name = "Van Gogh Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g188590-d191295-Reviews-Van_Gogh_Museum-Amsterdam_North_Holland.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "Pancakes Amsterdam", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g188590-d1000632-Reviews-Pancakes_Amsterdam-Amsterdam_North_Holland.html" },
                    new PlaceDetail { Name = "Foodhallen", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g188590-d716896-Reviews-Foodhallen-Amsterdam_North_Holland.html" }
                }
            });

            // --- Barcelona, Spain ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Barcelona, Spain",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Sagrada Familia", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187497-d191295-Reviews-Basilica_of_the_Sagrada_Familia-Barcelona_Catalonia.html" },
                    new PlaceDetail { Name = "Park Güell", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187497-d191295-Reviews-Parc_Guell-Barcelona_Catalonia.html" },
                    new PlaceDetail { Name = "Gothic Quarter", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187497-d191295-Reviews-Barri_Gotic-Barcelona_Catalonia.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "La Boqueria Market", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187497-d191295-Reviews-Mercat_de_la_Boqueria-Barcelona_Catalonia.html" },
                    new PlaceDetail { Name = "Ciudad Condal (tapas)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187497-d1000632-Reviews-Ciudad_Condal-Barcelona_Catalonia.html" }
                }
            });

            // --- Lisbon, Portugal ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Lisbon, Portugal",
                PlacesToVisit =
                {
                    new PlaceDetail { Name = "Belém Tower", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189158-d191295-Reviews-Tower_of_Belem-Lisbon_Lisbon_District_Central_Portugal.html" },
                    new PlaceDetail { Name = "Jerónimos Monastery", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189158-d218206-Reviews-Mosteiro_dos_Jeronimos-Lisbon_Lisbon_District_Central_Portugal.html" },
                    new PlaceDetail { Name = "São Jorge Castle", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189158-d218206-Reviews-St_George_s_Castle-Lisbon_Lisbon_District_Central_Portugal.html" }
                },
                PlacesToEat =
                {
                    new PlaceDetail { Name = "Pastéis de Belém", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189158-d1000632-Reviews-Pasteis_de_Belem-Lisbon_Lisbon_District_Central_Portugal.html" },
                    new PlaceDetail { Name = "Time Out Market Lisboa", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189158-d716896-Reviews-Time_Out_Market_Lisboa-Lisbon_Lisbon_District_Central_Portugal.html" }
                }
            });

            // --- Prague, Czech Republic ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Prague, Czech Republic",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Charles Bridge", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274707-d191295-Reviews-Charles_Bridge-Prague_Bohemia.html" },
                    new PlaceDetail { Name = "Prague Castle", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274707-d207436-Reviews-Prague_Castle-Prague_Bohemia.html" },
                    new PlaceDetail { Name = "Old Town Square", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274707-d284797-Reviews-Old_Town_Square-Prague_Bohemia.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Lokál Dlouhááá", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g274707-d1000632-Reviews-Lokal_Dlouhaaa-Prague_Bohemia.html" },
                    new PlaceDetail { Name = "Trdelník (street food)", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274707-d716896-Reviews-Trdelnik-Prague_Bohemia.html" } // Note: this is a general search for Trdelnik
                }
            });

            // --- Vienna, Austria ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Vienna, Austria",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Schönbrunn Palace", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g190447-d191295-Reviews-Schonbrunn_Palace-Vienna.html" },
                    new PlaceDetail { Name = "St. Stephen's Cathedral", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g190447-d207436-Reviews-St_Stephen_s_Cathedral-Vienna.html" },
                    new PlaceDetail { Name = "Hofburg Palace", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g190447-d284797-Reviews-Hofburg_Palace-Vienna.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Figlmüller (Schnitzel)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g190447-d1000632-Reviews-Figlmuller-Vienna.html" },
                    new PlaceDetail { Name = "Demel (pastries)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g190447-d716896-Reviews-Demel-Vienna.html" }
                }
            });

            // --- Florence, Italy ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Florence, Italy",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Duomo", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187895-d191295-Reviews-Duomo_Cattedrale_di_Santa_Maria_del_Fiore-Florence_Tuscany.html" },
                    new PlaceDetail { Name = "Uffizi Gallery", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187895-d207436-Reviews-Galleria_degli_Uffizi-Florence_Tuscany.html" },
                    new PlaceDetail { Name = "Ponte Vecchio", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187895-d284797-Reviews-Ponte_Vecchio-Florence_Tuscany.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "All'Antico Vinaio", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187895-d1000632-Reviews-All_Antico_Vinaio-Florence_Tuscany.html" },
                    new PlaceDetail { Name = "Trattoria Mario", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187895-d716896-Reviews-Trattoria_Mario-Florence_Tuscany.html" }
                }
            });

            // --- Dublin, Ireland ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Dublin, Ireland",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Guinness Storehouse", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186609-d191295-Reviews-Guinness_Storehouse-Dublin_County_Dublin.html" },
                    new PlaceDetail { Name = "Trinity College (Book of Kells)", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186609-d207436-Reviews-Trinity_College_Dublin-Dublin_County_Dublin.html" },
                    new PlaceDetail { Name = "Temple Bar", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186609-d284797-Reviews-Temple_Bar-Dublin_County_Dublin.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "The Brazen Head (Pub)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g186609-d1000632-Reviews-The_Brazen_Head-Dublin_County_Dublin.html" },
                    new PlaceDetail { Name = "Leo Burdock (Fish & Chips)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g186609-d716896-Reviews-Leo_Burdock_Christchurch-Dublin_County_Dublin.html" }
                }
            });

            // --- Budapest, Hungary ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Budapest, Hungary",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Parliament Building", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274887-d191295-Reviews-Hungarian_Parliament_Building-Budapest_Central_Hungary.html" },
                    new PlaceDetail { Name = "Buda Castle", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274887-d207436-Reviews-Buda_Castle-Budapest_Central_Hungary.html" },
                    new PlaceDetail { Name = "Fisherman's Bastion", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274887-d284797-Reviews-Halaszbastya-Budapest_Central_Hungary.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Central Market Hall", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274887-d1000632-Reviews-Central_Market_Hall-Budapest_Central_Hungary.html" },
                    new PlaceDetail { Name = "New York Café", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g274887-d716896-Reviews-New_York_Cafe-Budapest_Central_Hungary.html" }
                }
            });

            // --- Copenhagen, Denmark ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Copenhagen, Denmark",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Nyhavn", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189541-d191295-Reviews-Nyhavn-Copenhagen_Capital_Region.html" },
                    new PlaceDetail { Name = "Little Mermaid statue", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189541-d207436-Reviews-The_Little_Mermaid_Statue-Copenhagen_Capital_Region.html" },
                    new PlaceDetail { Name = "Tivoli Gardens", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189541-d284797-Reviews-Tivoli_Gardens-Copenhagen_Capital_Region.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Pølsevogn (hot dog stand)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189541-d1000632-Reviews-Copenhagen_Street_Food-Copenhagen_Capital_Region.html" }, // General food market example
                    new PlaceDetail { Name = "Torvehallerne Market", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189541-d716896-Reviews-Torvehallerne-Copenhagen_Capital_Region.html" }
                }
            });

            // --- Stockholm, Sweden ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Stockholm, Sweden",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Vasa Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189852-d191295-Reviews-Vasa_Museum-Stockholm.html" },
                    new PlaceDetail { Name = "Gamla Stan (Old Town)", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189852-d207436-Reviews-Gamla_Stan-Stockholm.html" },
                    new PlaceDetail { Name = "ABBA The Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189852-d284797-Reviews-ABBA_The_Museum-Stockholm.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Meatballs for the People", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189852-d1000632-Reviews-Meatballs_for_the_People-Stockholm.html" },
                    new PlaceDetail { Name = "Östermalms Saluhall", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189852-d716896-Reviews-Ostermalms_Saluhall-Stockholm.html" }
                }
            });

            // --- Brussels, Belgium ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Brussels, Belgium",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Grand Place", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g188644-d191295-Reviews-Grand_Place-Brussels.html" },
                    new PlaceDetail { Name = "Manneken Pis", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g188644-d207436-Reviews-Manneken_Pis-Brussels.html" },
                    new PlaceDetail { Name = "Atomium", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g188644-d284797-Reviews-Atomium-Brussels.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Maison Antoine (Fries)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g188644-d1000632-Reviews-Maison_Antoine-Brussels.html" },
                    new PlaceDetail { Name = "Chez Léon (Mussels)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g188644-d716896-Reviews-Chez_Leon-Brussels.html" }
                }
            });

            // --- Warsaw, Poland ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Warsaw, Poland",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Old Town Market Place", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274856-d191295-Reviews-Old_Town_Market_Place-Warsaw_Mazovia_Province_Central_Poland.html" },
                    new PlaceDetail { Name = "Royal Castle", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274856-d207436-Reviews-Royal_Castle-Warsaw_Mazovia_Province_Central_Poland.html" },
                    new PlaceDetail { Name = "Warsaw Uprising Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g274856-d284797-Reviews-Warsaw_Uprising_Museum-Warsaw_Mazovia_Province_Central_Poland.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Zapiecek (Pierogi)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g274856-d1000632-Reviews-Zapiecek_Polskie_Pierogarnie-Warsaw_Mazovia_Province_Central_Poland.html" },
                    new PlaceDetail { Name = "Browarmia Krolewska", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g274856-d716896-Reviews-Browarmia_Krolewska-Warsaw_Mazovia_Province_Central_Poland.html" }
                }
            });

            // --- Helsinki, Finland ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Helsinki, Finland",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Suomenlinna Fortress", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189934-d191295-Reviews-Suomenlinna_Fortress-Helsinki_Uusimaa.html" },
                    new PlaceDetail { Name = "Temppeliaukio Church", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189934-d207436-Reviews-Temppeliaukio_Church-Helsinki_Uusimaa.html" },
                    new PlaceDetail { Name = "Market Square", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189934-d284797-Reviews-Market_Square-Helsinki_Uusimaa.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Old Market Hall", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189934-d1000632-Reviews-Old_Market_Hall-Helsinki_Uusimaa.html" },
                    new PlaceDetail { Name = "Ravintola Savoy", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189934-d716896-Reviews-Ravintola_Savoy-Helsinki_Uusimaa.html" }
                }
            });

            // --- Oslo, Norway ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Oslo, Norway",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Vigeland Sculpture Park", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g190447-d191295-Reviews-Vigeland_Museum-Oslo_Eastern_Norway.html" },
                    new PlaceDetail { Name = "Viking Ship Museum", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g190447-d207436-Reviews-Viking_Ship_Museum-Oslo_Eastern_Norway.html" },
                    new PlaceDetail { Name = "Akershus Fortress", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g190447-d284797-Reviews-Akershus_Fortress-Oslo_Eastern_Norway.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Mathallen Oslo (Food Hall)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g190447-d1000632-Reviews-Mathallen_Oslo-Oslo_Eastern_Norway.html" },
                    new PlaceDetail { Name = "Fiskeriet Youngstorget", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g190447-d716896-Reviews-Fiskeriet_Youngstorget-Oslo_Eastern_Norway.html" }
                }
            });

            // --- Athens, Greece ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Athens, Greece",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Acropolis (Parthenon)", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189400-d191295-Reviews-Acropolis-Athens_Attica.html" },
                    new PlaceDetail { Name = "Plaka", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189400-d207436-Reviews-Plaka-Athens_Attica.html" },
                    new PlaceDetail { Name = "Syntagma Square", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g189400-d284797-Reviews-Syntagma_Square-Athens_Attica.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Tzitzikas kai o Mermigas", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189400-d1000632-Reviews-Tzitzikas_kai_o_Mermigas-Athens_Attica.html" },
                    new PlaceDetail { Name = "Avli", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g189400-d716896-Reviews-Avli-Athens_Attica.html" }
                }
            });

            // --- Edinburgh, Scotland ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Edinburgh, Scotland",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Edinburgh Castle", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186525-d191295-Reviews-Edinburgh_Castle-Edinburgh_Scotland.html" },
                    new PlaceDetail { Name = "Royal Mile", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186525-d207436-Reviews-The_Royal_Mile-Edinburgh_Scotland.html" },
                    new PlaceDetail { Name = "Arthur's Seat", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g186525-d284797-Reviews-Arthur_s_Seat-Edinburgh_Scotland.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Oink (Hog Roast)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g186525-d1000632-Reviews-Oink-Edinburgh_Scotland.html" },
                    new PlaceDetail { Name = "The Witchery by the Castle", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g186525-d716896-Reviews-The_Witchery_by_the_Castle-Edinburgh_Scotland.html" }
                }
            });

            // --- Seville, Spain ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Seville, Spain",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Seville Cathedral", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187443-d191295-Reviews-Seville_Cathedral-Seville_Province_of_Seville_Andalucia.html" },
                    new PlaceDetail { Name = "Alcazar", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187443-d207436-Reviews-Real_Alcazar_de_Sevilla-Seville_Province_of_Seville_Andalucia.html" },
                    new PlaceDetail { Name = "Plaza de España", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187443-d284797-Reviews-Plaza_de_Espana-Seville_Province_of_Seville_Andalucia.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "El Rinconcillo", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187443-d1000632-Reviews-El_Rinconcillo-Seville_Province_of_Seville_Andalucia.html" },
                    new PlaceDetail { Name = "Bar La Azotea", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187443-d716896-Reviews-La_Azotea-Seville_Province_of_Seville_Andalucia.html" }
                }
            });

            // --- Venice, Italy ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Venice, Italy",
                PlacesToVisit = {
                    new PlaceDetail { Name = "St. Mark's Square", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187870-d191295-Reviews-Piazza_San_Marco-Venice_Veneto.html" },
                    new PlaceDetail { Name = "Doge's Palace", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187870-d207436-Reviews-Doge_s_Palace-Venice_Veneto.html" },
                    new PlaceDetail { Name = "Rialto Bridge", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g187870-d284797-Reviews-Ponte_di_Rialto-Venice_Veneto.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Trattoria Al Gazzettino", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187870-d1000632-Reviews-Trattoria_Al_Gazzettino-Venice_Veneto.html" },
                    new PlaceDetail { Name = "Ristorante Da Ivo", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g187870-d716896-Reviews-Ristorante_Da_Ivo-Venice_Veneto.html" }
                }
            });

            // --- Istanbul, Turkey ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Istanbul, Turkey",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Hagia Sophia", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g293974-d191295-Reviews-Hagia_Sophia_Mosque-Istanbul.html" },
                    new PlaceDetail { Name = "Blue Mosque", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g293974-d207436-Reviews-Blue_Mosque-Istanbul.html" },
                    new PlaceDetail { Name = "Grand Bazaar", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g293974-d284797-Reviews-Grand_Bazaar-Istanbul.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Balikci Sabahattin (Seafood)", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g293974-d1000632-Reviews-Balikci_Sabahattin-Istanbul.html" },
                    new PlaceDetail { Name = "Neolokal", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g293974-d716896-Reviews-Neolokal-Istanbul.html" }
                }
            });

            // --- Dubai, UAE ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Dubai, UAE",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Burj Khalifa", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g295424-d191295-Reviews-Burj_Khalifa-Dubai_United_Arab_Emirates.html" },
                    new PlaceDetail { Name = "The Dubai Mall", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g295424-d207436-Reviews-The_Dubai_Mall-Dubai_United_Arab_Emirates.html" },
                    new PlaceDetail { Name = "The Dubai Fountain", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g295424-d284797-Reviews-The_Dubai_Fountain-Dubai_United_Arab_Emirates.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Al Fanar Restaurant & Cafe", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g295424-d1000632-Reviews-Al_Fanar_Restaurant_Cafe-Dubai_United_Arab_Emirates.html" },
                    new PlaceDetail { Name = "Pierchic", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g295424-d716896-Reviews-Pierchic-Dubai_United_Arab_Emirates.html" }
                }
            });

            // --- Tokyo, Japan ---
            SuggestedCities.Add(new CitySuggestion
            {
                CityName = "Tokyo, Japan",
                PlacesToVisit = {
                    new PlaceDetail { Name = "Senso-ji Temple", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g298184-d191295-Reviews-Senso_ji_Temple-Taito_Tokyo_Tokyo_Prefecture_Kanto.html" },
                    new PlaceDetail { Name = "Shibuya Crossing", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g298184-d207436-Reviews-Shibuya_Crossing-Shibuya_Tokyo_Tokyo_Prefecture_Kanto.html" },
                    new PlaceDetail { Name = "Tokyo Skytree", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g298184-d284797-Reviews-Tokyo_Skytree-Sumida_Tokyo_Tokyo_Prefecture_Kanto.html" }
                },
                PlacesToEat = {
                    new PlaceDetail { Name = "Tsukiji Outer Market", WebsiteLink = "https://www.tripadvisor.com/Attraction_Review-g298184-d1000632-Reviews-Tsukiji_Outer_Market-Chuo_Tokyo_Tokyo_Prefecture_Kanto.html" },
                    new PlaceDetail { Name = "Sushi Saito", WebsiteLink = "https://www.tripadvisor.com/Restaurant_Review-g298184-d716896-Reviews-Sushi_Saito-Minato_Tokyo_Tokyo_Prefecture_Kanto.html" }
                }
            });
        }

        private void DestinationComboBox_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Optional: Implement auto-suggestion/filtering if needed
            // For now, this is just a placeholder, as the ComboBox is bound to SuggestedCities
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Ensure a destination is selected or entered
            string destination = "";
            if (DestinationComboBox.SelectedItem is CitySuggestion selectedCity)
            {
                destination = selectedCity.CityName;
            }
            else if (!string.IsNullOrWhiteSpace(DestinationComboBox.Text))
            {
                destination = DestinationComboBox.Text;
            }
            else
            {
                MessageBox.Show("Please select or enter a destination.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            if (!StartDatePicker.SelectedDate.HasValue || !EndDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select both a start and an end date.", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (StartDatePicker.SelectedDate.Value > EndDatePicker.SelectedDate.Value)
            {
                MessageBox.Show("Start date cannot be after end date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(BudgetTextBox.Text, out decimal budget) || budget < 0)
            {
                MessageBox.Show("Please enter a valid positive number for the budget.", "Invalid Budget", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var newTrip = new Trip
                {
                    UserId = _currentUser.ID,
                    Destination = destination,
                    StartDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    EndDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value),
                    Budget = budget,
                    ItemsToTake = ItemsTextBox.Text,
                    Activities = ActivitiesTextBox.Text
                };

                _db.Trips.Add(newTrip);
                await _db.SaveChangesAsync();

                MessageBox.Show("Trip added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Optionally clear form or close window
                this.Close();
                // You might want to open the TripsPage here after saving
                // TripsPage tripsPage = new TripsPage();
                // tripsPage.Show();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the trip: {ex.Message}\nInner: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // NEW: Event handler for the "Activities suggestions" button
        private void ActivitiesSuggestionsButton_Click(object sender, RoutedEventArgs e)
        {
            // If the user has typed text, try to find a matching city
            CitySuggestion selectedCity = null;
            if (DestinationComboBox.SelectedItem is CitySuggestion comboBoxSelected)
            {
                selectedCity = comboBoxSelected;
            }
            else if (!string.IsNullOrWhiteSpace(DestinationComboBox.Text))
            {
                string searchText = DestinationComboBox.Text.Trim();
                // Find the first city that starts with or contains the typed text (case-insensitive)
                selectedCity = SuggestedCities.FirstOrDefault(c =>
                    c.CityName.StartsWith(searchText, StringComparison.OrdinalIgnoreCase) ||
                    c.CityName.Contains(searchText, StringComparison.OrdinalIgnoreCase));
            }


            if (selectedCity != null)
            {
                CityActivitiesWindow activitiesWindow = new CityActivitiesWindow(selectedCity);
                activitiesWindow.Owner = this; // Set owner to keep it on top of the main window
                activitiesWindow.ShowDialog(); // Show as modal dialog
            }
            else
            {
                MessageBox.Show("Please select a destination city from the dropdown or type a valid city name.", "No City Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Sidebar Navigation Methods (ensure these open the correct windows)
        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            Personal_info personalInfoPage = new Personal_info();
            personalInfoPage.Show();
            this.Close();
        }

        private void Trips_Click(object sender, RoutedEventArgs e)
        {
            // Assuming TripsPage is your main Trips window
            // TripsPage tripsPage = new TripsPage();
            // tripsPage.Show();
            // this.Close();
            MessageBox.Show("Trips page navigation placeholder.");
        }

        private void Finance_Click(object sender, RoutedEventArgs e)
        {
            // Assuming BankInfo is your main Finance window
            // BankInfo bankInfo = new BankInfo();
            // bankInfo.Show();
            // this.Close();
            MessageBox.Show("Finance page navigation placeholder.");
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            // Assuming UploadDocumentPage is your main Documents window
            UploadDocumentPage uploadDocumentPage = new UploadDocumentPage();
            uploadDocumentPage.Show();
            this.Close();
        }

        private void Dashboarding_Click(object sender, RoutedEventArgs e)
        {
            // Assuming DashboardingPage is your main Dashboarding window
            // DashboardingPage dashboardingPage = new DashboardingPage();
            // dashboardingPage.Show();
            // this.Close();
            MessageBox.Show("Dashboarding page navigation placeholder.");
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            // Assuming Settings is your settings page
            Settings settingsPage = new Settings();
            settingsPage.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                AppState.CurrentUser = null;
                // Assuming LoginPage is your main login window
                MainWindow loginPage = new MainWindow();
                loginPage.Show();
                this.Close();
            }
        }

    }
}