using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Labb.Model;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Windows.Input;

namespace Labb
{
    public partial class MainPage : ContentPage
    {
        List<News> newsList = new List<News>();

        public MainPage()
        {
            InitializeComponent();
            
            var goToTopButton = new ToolbarItem
            {
                Icon = "toTop.png",
                Text = "Topp",
                Command = new Command(this.GoToTop),
            };
            this.ToolbarItems.Add(goToTopButton);

            var updateButton = new ToolbarItem
            {
                Icon = "refresh.png",
                Text = "Hämta",
                Command = new Command(this.UpdateFeed),
            };
            this.ToolbarItems.Add(updateButton);

            CallRSSFeeds();
        }

        //Metoder för toppmenyns knappar
        private void GoToTop(object obj)
        {
            listNews.ItemsSource = newsList.OrderByDescending(x => x.Date);
            listNews.ScrollTo(newsList.OrderByDescending(x => x.Date).First(), ScrollToPosition.Start, true);
        }
        private void UpdateFeed(object obj)
        {
            newsList.Clear();
            slider.Value = 10;
            CallRSSFeeds();
            listNews.ItemsSource = newsList.OrderByDescending(x => x.Date);
        }

        //Anrop till tidningarnas RSS-strömmar
        public async void CallRSSFeeds()
        {
            loadingTextSpinner.IsVisible = true;
            loadingTextSpinner.IsRunning = true;
            List<string> errors = new List<string>();
            HttpClient httpClient = new HttpClient();
            try
            {
                await GetCopyriot(httpClient);
            }
            catch
            {
                errors.Add("Copyriot");
            }
            try
            {
                await GetRiktpunkt(httpClient);
            }
            catch
            {
                errors.Add("Riktpunkt");
            }
            try
            {
                await GetRevolution(httpClient);
            }
            catch
            {
                errors.Add("Revolution");
            }
            try
            {
                await GetVansterbloggar(httpClient);
            }
            catch
            {
                errors.Add("Vänsterbloggar.se");
            }
            try
            {
                await GetDagensArbete(httpClient);
            }
            catch
            {
                errors.Add("Dagens Arbete");
            }
            try
            {
                await GetPolitism(httpClient);
            }
            catch
            {
                errors.Add("Politism");
            }
            try
            {
                await GetAftonbladet(httpClient);
            }
            catch
            {
                errors.Add("Aftonbladet");
            }
            try
            {
                await GetDagensArena(httpClient);
            }
            catch
            {
                errors.Add("Dagens Arena");
            }
            try
            {
                await GetDagensSamhalle(httpClient);
            }
            catch
            {
                errors.Add("Dagens Samhälle");
            }
            try
            {
                await GetSVT(httpClient);
            }
            catch
            {
                errors.Add("SVT");
            }
            try
            {
                await GetExpressen(httpClient);
            }
            catch
            {
                errors.Add("Expressen");
            }
            try
            {
                await GetUpsalaNyaTidning(httpClient);
            }
            catch
            {
                errors.Add("Upsala Nya Tidning");
            }
            try
            {
                await GetDagensNyheter(httpClient);
            }
            catch
            {
                errors.Add("Dagens Nyheter");
            }
            try
            {
                await GetRadikaltForum(httpClient);
            }
            catch
            {
                errors.Add("Radikalt Forum");
            }
            try
            {
                await GetSvenskaDagbladet(httpClient);
            }
            catch
            {
                errors.Add("Svenska Dagbladet");
            }
            try
            {
                await GetBarometern(httpClient);
            }
            catch
            {
                errors.Add("Barometern");
            }
            try
            {
                await GetCornucopia(httpClient);
            }
            catch
            {
                errors.Add("Cornucopia");
            }
            try
            {
                await GetGenusdebatten(httpClient);
            }
            catch
            {
                errors.Add("Genusdebatten");
            }
            try
            {
                await GetSamtiden(httpClient);
            }
            catch
            {
                errors.Add("Samtiden");
            }
            try
            {
                await GetFriaTider(httpClient);
            }
            catch
            {
                errors.Add("Fria Tider");
            }
            if (errors.Count > 0)
            {
                string[] error = errors.ToArray();
                await DisplayAlert("Hoppsan", "Följande nyhetskällor gick inte att nå:\n" + string.Join(Environment.NewLine, error) + "\nProva igen om en stund!", "OK");
            }
            loadingTextSpinner.IsVisible = false;
            loadingTextSpinner.IsRunning = false;
            listNews.ItemsSource = newsList.OrderByDescending(x => x.Date);

        }

        #region RSS Feeds

        private async Task GetCopyriot(HttpClient httpClient)
        {
            string copyriotURL = "https://copyriot.se/feed/";
            HttpResponseMessage copyriotResponseXML = await httpClient.GetAsync(new Uri(copyriotURL));
            if (copyriotResponseXML.IsSuccessStatusCode)
            {
                var copyriotContent = await copyriotResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(copyriotContent));
                var copyriotResults = from m in dataDoc.Descendants("item")
                                       select new NewsXML
                                       {
                                           showValue = 0,
                                           source = "Copyriot",
                                           pubDate = (string)m.Element("pubDate") ?? "",
                                           title = (string)m.Element("title") ?? "",
                                           description = (string)m.Element("description") ?? "",
                                           link = (string)m.Element("link") ?? "",
                                       };
                foreach (var s in copyriotResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();
                    strippedDescription = strippedDescription.Substring(0, 200) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetRiktpunkt(HttpClient httpClient)
        {
            string riktpunktURL = "http://riktpunkt.nu/feed/";
            HttpResponseMessage riktpunktResponseXML = await httpClient.GetAsync(new Uri(riktpunktURL));
            if (riktpunktResponseXML.IsSuccessStatusCode)
            {
                var riktpunktContent = await riktpunktResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(riktpunktContent));
                var riktpunktResults = from m in dataDoc.Descendants("item")
                                       select new NewsXML
                                       {
                                           showValue = 1,
                                           source = "Riktpunkt",
                                           pubDate = (string)m.Element("pubDate") ?? "",
                                           title = (string)m.Element("title") ?? "",
                                           description = (string)m.Element("description") ?? "",
                                           link = (string)m.Element("link") ?? "",
                                       };
                foreach (var s in riktpunktResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp|&#.+?;)", " ").Trim();
                    strippedDescription = strippedDescription.Substring(0, strippedDescription.Length - 50) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetRevolution(HttpClient httpClient)
        {
            string revolutionURL = "http://www.marxist.se/rss.xml";
            HttpResponseMessage revolutionResponseXML = await httpClient.GetAsync(new Uri(revolutionURL));
            if (revolutionResponseXML.IsSuccessStatusCode)
            {
                var revolutionContent = await revolutionResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(revolutionContent));
                var revolutionNewsResults = from m in dataDoc.Descendants("item")
                                            select new NewsXML
                                            {
                                                showValue = 2,
                                                source = "Revolution",
                                                pubDate = (string)m.Element("pubDate") ?? "",
                                                title = (string)m.Element("title") ?? "",
                                                description = (string)m.Element("description") ?? "",
                                                link = (string)m.Element("link") ?? "",
                                            };
                foreach (var s in revolutionNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", "").Trim();
                    strippedDescription = strippedDescription.Substring(0, 200) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetVansterbloggar(HttpClient httpClient)
        {
            string vansterURL = "http://www.vansterbloggar.se/portal/feed/";
            HttpResponseMessage vansterResponseXML = await httpClient.GetAsync(new Uri(vansterURL));
            if (vansterResponseXML.IsSuccessStatusCode)
            {
                var vansterContent = await vansterResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(vansterContent));
                var vansterNewsResults = from m in dataDoc.Descendants("item")
                                         select new NewsXML
                                         {
                                             showValue = 3,
                                             source = "Vänsterbloggar.se",
                                             pubDate = (string)m.Element("pubDate") ?? "",
                                             title = (string)m.Element("title") ?? "",
                                             description = (string)m.Element("description") ?? "",
                                             link = (string)m.Element("link") ?? "",
                                         };
                foreach (var s in vansterNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&#.+?;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetDagensArbete(HttpClient httpClient)
        {
            string dagensArbeteURL = "http://da.se/feed/";
            HttpResponseMessage dagensArbeteResponseXML = await httpClient.GetAsync(new Uri(dagensArbeteURL));
            if (dagensArbeteResponseXML.IsSuccessStatusCode)
            {
                var dagensArbeteContent = await dagensArbeteResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(dagensArbeteContent));
                var dagensArbeteResults = from m in dataDoc.Descendants("item")
                                       select new NewsXML
                                       {
                                           showValue = 4,
                                           source = "Dagens Arbete",
                                           pubDate = (string)m.Element("pubDate") ?? "",
                                           title = (string)m.Element("title") ?? "",
                                           description = (string)m.Element("description") ?? "",
                                           link = (string)m.Element("link") ?? "",
                                       };
                foreach (var s in dagensArbeteResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetPolitism(HttpClient httpClient)
        {
            string politismURL = "http://www.politism.se/feed/";
            HttpResponseMessage politismResponseXML = await httpClient.GetAsync(new Uri(politismURL));
            if (politismResponseXML.IsSuccessStatusCode)
            {
                var politismContent = await politismResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(politismContent));
                var politismResults = from m in dataDoc.Descendants("item")
                                          select new NewsXML
                                          {
                                              showValue = 5,
                                              source = "Politism",
                                              pubDate = (string)m.Element("pubDate") ?? "",
                                              title = (string)m.Element("title") ?? "",
                                              description = (string)m.Element("description") ?? "",
                                              link = (string)m.Element("link") ?? "",
                                          };
                foreach (var s in politismResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&#.+?;)", "").Trim();
                    strippedDescription = strippedDescription.Substring(0, strippedDescription.Length - 20) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetSydostran(HttpClient httpClient)
        {
            string sydostranURL = "http://www.sydostran.se/feed/";
            HttpResponseMessage sydostranResponseXML = await httpClient.GetAsync(new Uri(sydostranURL));
            if (sydostranResponseXML.IsSuccessStatusCode)
            {
                var sydostranContent = await sydostranResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(sydostranContent));
                var sydostranResults = from m in dataDoc.Descendants("item")
                                       select new NewsXML
                                       {
                                           showValue = 5,
                                           source = "Sydöstran",
                                           pubDate = (string)m.Element("pubDate") ?? "",
                                           title = (string)m.Element("title") ?? "",
                                           description = (string)m.Element("description") ?? "",
                                           link = (string)m.Element("link") ?? "",
                                       };
                foreach (var s in sydostranResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetAftonbladet(HttpClient httpClient)
        {
            string aftonbladetURL = "http://www.aftonbladet.se/rss.xml";
            HttpResponseMessage aftonbladetResponseXML = await httpClient.GetAsync(new Uri(aftonbladetURL));
            if (aftonbladetResponseXML.IsSuccessStatusCode)
            {
                var aftonbladetContent = await aftonbladetResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(aftonbladetContent));
                var aftonbladetNewsResults = from m in dataDoc.Descendants("item")
                                             select new NewsXML
                                             {
                                                 showValue = 6,
                                                 source = "Aftonbladet",
                                                 pubDate = (string)m.Element("pubDate") ?? "",
                                                 title = (string)m.Element("title") ?? "",
                                                 description = (string)m.Element("description") ?? "",
                                                 link = (string)m.Element("link") ?? "",
                                             };
                foreach (var s in aftonbladetNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, d MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);
                    
                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }

            }
        }

        private async Task GetDagensArena(HttpClient httpClient)
        {
            string arenaURL = "http://www.dagensarena.se/feed/da";
            HttpResponseMessage arenaResponseXML = await httpClient.GetAsync(new Uri(arenaURL));
            if (arenaResponseXML.IsSuccessStatusCode)
            {
                var arenaContent = await arenaResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(arenaContent));
                var arenaResults = from m in dataDoc.Descendants("item")
                                   select new NewsXML
                                   {
                                       showValue = 8,
                                       source = "Dagens Arena",
                                       pubDate = (string)m.Element("pubDate") ?? "",
                                       title = (string)m.Element("title") ?? "",
                                       description = (string)m.Element("description") ?? "",
                                       link = (string)m.Element("link") ?? "",
                                   };
                foreach (var s in arenaResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetDagensSamhalle(HttpClient httpClient)
        {
            string dsURL = "https://www.dagenssamhalle.se/feed/all";
            HttpResponseMessage dsResponseXML = await httpClient.GetAsync(new Uri(dsURL));
            if (dsResponseXML.IsSuccessStatusCode)
            {
                var dsContent = await dsResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(dsContent));
                var dsResults = from m in dataDoc.Descendants("item")
                                select new NewsXML
                                {
                                    showValue = 9,
                                    source = "Dagens Samhälle",
                                    pubDate = (string)m.Element("pubDate") ?? "",
                                    title = (string)m.Element("title") ?? "",
                                    description = (string)m.Element("description") ?? "",
                                    link = (string)m.Element("link") ?? "",
                                };
                foreach (var s in dsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, s.description, s.link));
                }
            }
        }

        private async Task GetSVT(HttpClient httpClient)
        {
            string svtURL = "https://www.svt.se/nyheter/rss.xml";
            HttpResponseMessage svtResponseXML = await httpClient.GetAsync(new Uri(svtURL));
            if (svtResponseXML.IsSuccessStatusCode)
            {
                var svtContent = await svtResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(svtContent));
                var svtNewsResults = from m in dataDoc.Descendants("item")
                                     select new NewsXML
                                     {
                                         showValue = 10,
                                         source = "SVT Nyheter",
                                         pubDate = (string)m.Element("pubDate") ?? "",
                                         title = (string)m.Element("title") ?? "",
                                         description = (string)m.Element("description") ?? "",
                                         link = (string)m.Element("link") ?? "",
                                     };
                foreach (var s in svtNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, s.description, s.link));
                }
            }
        }

        private async Task GetExpressen(HttpClient httpClient)
        {
            string expressenURL = "http://expressen.se/rss/nyheter";
            HttpResponseMessage expressenResponseXML = await httpClient.GetAsync(new Uri(expressenURL));
            if (expressenResponseXML.IsSuccessStatusCode)
            {
                var expressenContent = await expressenResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(expressenContent));
                var expressenNewsResults = from m in dataDoc.Descendants("item")
                                           select new NewsXML
                                           {
                                               showValue = 11,
                                               source = "Expressen",
                                               pubDate = (string)m.Element("pubDate") ?? "",
                                               title = (string)m.Element("title") ?? "",
                                               description = (string)m.Element("description") ?? "",
                                               link = (string)m.Element("link") ?? "",
                                           };

                foreach (var s in expressenNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetUpsalaNyaTidningInrikes(HttpClient httpClient)
        {
            string untNewsURL = "http://www.unt.se/rss/sverige/";
            HttpResponseMessage untNewsResponseXML = await httpClient.GetAsync(new Uri(untNewsURL));
            if (untNewsResponseXML.IsSuccessStatusCode)
            {
                var untNewsContent = await untNewsResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(untNewsContent));
                var untNewsResults = from m in dataDoc.Descendants("item")
                                     select new NewsXML
                                     {
                                         showValue = 12,
                                         source = "Upsala Nya Tidning",
                                         pubDate = (string)m.Element("pubDate") ?? "",
                                         title = (string)m.Element("title") ?? "",
                                         description = (string)m.Element("description") ?? "",
                                         link = (string)m.Element("link") ?? "",
                                     };
                foreach (var s in untNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetUpsalaNyaTidning(HttpClient httpClient)
        {
            string untLeadURL = "http://www.unt.se/rss/ledare/";
            HttpResponseMessage untLeadResponseXML = await httpClient.GetAsync(new Uri(untLeadURL));
            if (untLeadResponseXML.IsSuccessStatusCode)
            {
                var untLeadContent = await untLeadResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(untLeadContent));
                var untLeadResults = from m in dataDoc.Descendants("item")
                                     select new NewsXML
                                     {
                                         showValue = 12,
                                         source = "Upsala Nya Tidning, ledare",
                                         pubDate = (string)m.Element("pubDate") ?? "",
                                         title = (string)m.Element("title") ?? "",
                                         description = (string)m.Element("description") ?? "",
                                         link = (string)m.Element("link") ?? "",
                                     };
                foreach (var s in untLeadResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);
                    //DateTime parsedAndCorrectedDate = parsedDate.AddHours(2);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetDagensNyheter(HttpClient httpClient)
        {
            string dnURL = "http://www.dn.se/nyheter/m/rss/";
            HttpResponseMessage dnResponseXML = await httpClient.GetAsync(new Uri(dnURL));
            if (dnResponseXML.IsSuccessStatusCode)
            {
                var dnContent = await dnResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(dnContent));
                var dnNewsResults = from m in dataDoc.Descendants("item")
                                    select new NewsXML
                                    {
                                        showValue = 13,
                                        source = "Dagens Nyheter",
                                        pubDate = (string)m.Element("pubDate") ?? "",
                                        title = (string)m.Element("title") ?? "",
                                        description = (string)m.Element("description") ?? "",
                                        link = (string)m.Element("link") ?? "",
                                    };
                foreach (var s in dnNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", System.Globalization.CultureInfo.InvariantCulture);
                    DateTime parsedAndCorrectedDate = parsedDate.AddHours(1);

                    newsList.Add(new News(s.showValue, s.source, parsedAndCorrectedDate, s.title, s.description, s.link));
                }
            }
        }

        private async Task GetRadikaltForum(HttpClient httpClient)
        {
            string radikalURL = "http://www.radikaltforum.se/feed/";
            HttpResponseMessage radikalResponseXML = await httpClient.GetAsync(new Uri(radikalURL));
            if (radikalResponseXML.IsSuccessStatusCode)
            {
                var radikalContent = await radikalResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(radikalContent));
                var radikalNewsResults = from m in dataDoc.Descendants("item")
                                         select new NewsXML
                                         {
                                             showValue = 14,
                                             source = "Radikalt Forum",
                                             pubDate = (string)m.Element("pubDate") ?? "",
                                             title = (string)m.Element("title") ?? "",
                                             description = (string)m.Element("description") ?? "",
                                             link = (string)m.Element("link") ?? "",
                                         };
                foreach (var s in radikalNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>)", "").Trim();
                    strippedDescription = strippedDescription.Substring(0, 200) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetSvenskaDagbladet(HttpClient httpClient)
        {
            string svdURL = "https://www.svd.se/feed/articles.rss";
            HttpResponseMessage svdResponseXML = await httpClient.GetAsync(new Uri(svdURL));
            if (svdResponseXML.IsSuccessStatusCode)
            {
                var svdContent = await svdResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(svdContent));
                var svdResults = from m in dataDoc.Descendants("item")
                                 select new NewsXML
                                 {
                                     showValue = 15,
                                     source = "Svenska Dagbladet",
                                     pubDate = (string)m.Element("pubDate") ?? "",
                                     title = (string)m.Element("title") ?? "",
                                     description = (string)m.Element("description") ?? "",
                                     link = (string)m.Element("link") ?? "",
                                 };
                foreach (var s in svdResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-entiteter i title-taggen
                    string strippedTitle = Regex.Replace(s.title, @"(&quot;)", " ").Trim();

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, strippedTitle, strippedDescription, s.link));
                }
            }
        }

        private async Task GetBarometern(HttpClient httpClient)
        {
            string barometernURL = "http://www.barometern.se/feed/";
            HttpResponseMessage barometernResponseXML = await httpClient.GetAsync(new Uri(barometernURL));
            if (barometernResponseXML.IsSuccessStatusCode)
            {
                var barometernContent = await barometernResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(barometernContent));
                var barometernResults = from m in dataDoc.Descendants("item")
                                        select new NewsXML
                                        {
                                            showValue = 16,
                                            source = "Barometern",
                                            pubDate = (string)m.Element("pubDate") ?? "",
                                            title = (string)m.Element("title") ?? "",
                                            description = (string)m.Element("description") ?? "",
                                            link = (string)m.Element("link") ?? "",
                                        };
                foreach (var s in barometernResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);
                   
                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetCornucopia(HttpClient httpClient)
        {
            string cornuURL = "http://cornucopia.cornubot.se/feeds/posts/default?alt=rss";
            HttpResponseMessage cornuResponseXML = await httpClient.GetAsync(new Uri(cornuURL));
            if (cornuResponseXML.IsSuccessStatusCode)
            {
                var cornuContent = await cornuResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(cornuContent));
                var cornuNewsResults = from m in dataDoc.Descendants("item")
                                       select new NewsXML
                                       {
                                           showValue = 17,
                                           source = "Cornucopia",
                                           pubDate = (string)m.Element("pubDate") ?? "",
                                           title = (string)m.Element("title") ?? "",
                                           description = (string)m.Element("description") ?? "",
                                           link = (string)m.Element("link") ?? "",
                                       };
                foreach (var s in cornuNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);
                    parsedDate = parsedDate.AddHours(1);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>)", "").Trim();
                    strippedDescription = strippedDescription.Substring(0, 200) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetGenusdebatten(HttpClient httpClient)
        {
            string genusURL = "http://genusdebatten.se/feed/";
            HttpResponseMessage genusResponseXML = await httpClient.GetAsync(new Uri(genusURL));
            if (genusResponseXML.IsSuccessStatusCode)
            {
                var genusContent = await genusResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(genusContent));
                var genusResults = from m in dataDoc.Descendants("item")
                                        select new NewsXML
                                        {
                                            showValue = 18,
                                            source = "Genusdebatten",
                                            pubDate = (string)m.Element("pubDate") ?? "",
                                            title = (string)m.Element("title") ?? "",
                                            description = (string)m.Element("description") ?? "",
                                            link = (string)m.Element("link") ?? "",
                                        };
                foreach (var s in genusResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp|&#.+?;)", " ").Trim();
                    strippedDescription = strippedDescription.Substring(0, 200) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetSamtiden(HttpClient httpClient)
        {
            string samtidenURL = "https://samtiden.nu/feed/";
            HttpResponseMessage samtidenResponseXML = await httpClient.GetAsync(new Uri(samtidenURL));
            if (samtidenResponseXML.IsSuccessStatusCode)
            {
                var samtidenContent = await samtidenResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(samtidenContent));
                var samtidenResults = from m in dataDoc.Descendants("item")
                                        select new NewsXML
                                        {
                                            showValue = 19,
                                            source = "Samtiden",
                                            pubDate = (string)m.Element("pubDate") ?? "",
                                            title = (string)m.Element("title") ?? "",
                                            description = (string)m.Element("description") ?? "",
                                            link = (string)m.Element("link") ?? "",
                                        };
                foreach (var s in samtidenResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&#.+?)", " ").Trim();
                    strippedDescription = strippedDescription.Substring(0, 200) + "...";

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        private async Task GetFriaTider(HttpClient httpClient)
        {
            string friatiderURL = "http://www.friatider.se/rss.xml";
            HttpResponseMessage friatiderResponseXML = await httpClient.GetAsync(new Uri(friatiderURL));
            if (friatiderResponseXML.IsSuccessStatusCode)
            {
                var friatiderContent = await friatiderResponseXML.Content.ReadAsStringAsync();
                XDocument dataDoc = XDocument.Load(new StringReader(friatiderContent));
                var friatiderNewsResults = from m in dataDoc.Descendants("item")
                                           select new NewsXML
                                           {
                                               showValue = 20,
                                               source = "Fria Tider",
                                               pubDate = (string)m.Element("pubDate") ?? "",
                                               title = (string)m.Element("title") ?? "",
                                               description = (string)m.Element("description") ?? "",
                                               link = (string)m.Element("link") ?? "",
                                           };
                foreach (var s in friatiderNewsResults)
                {
                    //Parse-funktionalitet för att konvertera datumangivelsen
                    var parsedDate = DateTime.ParseExact(s.pubDate, "ddd, dd MMM yyyy HH:mm:ss K", System.Globalization.CultureInfo.InvariantCulture);

                    //Regex-funktionalitet för att radera HTML-element i description-taggen
                    string strippedDescription = Regex.Replace(s.description, @"(<.+?>|&nbsp;)", " ").Trim();

                    newsList.Add(new News(s.showValue, s.source, parsedDate, s.title, strippedDescription, s.link));
                }
            }
        }

        #endregion

        //Öppna vald artikel i en ny sida
        public void OpenArticle(object sender, SelectedItemChangedEventArgs e)
        {
            News selectedArticle = (News)e.SelectedItem;
            string linkToOpen = (string)selectedArticle.Link;
            string articleTitle = (string)selectedArticle.Title;
            Navigation.PushAsync(new ArticleContentPage(linkToOpen, articleTitle));
        }
        
        //Filtrera listvyn utefter förändrat värde av slidern
        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if ((int)e.NewValue >= 10)
            {
                var filteredList = newsList.Where(s => s.ShowValue >= (int)e.NewValue);
                listNews.ItemsSource = filteredList.OrderByDescending(x => x.Date);
                
                return;
            }
            else if ((int) e.NewValue <= 10)
            {
                var filteredList = newsList.Where(s => s.ShowValue <= (int)e.NewValue);

                listNews.ItemsSource = filteredList.OrderByDescending(x => x.Date);
                return;
            }
        }
    }
}
