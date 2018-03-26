using Xamarin.Forms;

namespace Labb
{
    internal class ArticleContentPage : ContentPage
    {
        private WebView browser;
        
        //Öppna länken ifrån vald artikel i en WebView
        public ArticleContentPage(string linkToOpen, string articleTitle)
        {
            this.Title = articleTitle;
            browser = new WebView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Source = linkToOpen
            };
            Content = browser;
        }
    }
}