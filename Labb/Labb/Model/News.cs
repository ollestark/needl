using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb.Model
{
    public class News
    {
        public int showValue;
        public string source;
        public DateTime date;
        public string title;
        public string description;
        public string link;

        public News(int showValue, string source, DateTime date, string title, string description, string link)
        {
            this.showValue = showValue;
            this.source = source;
            this.date = date;
            this.title = title;
            this.description = description;
            this.link = link;
        }
        public int ShowValue
        {
            get { return showValue; }
            set { showValue = value; }
        }
        public string OriginalSource
        {
            get { return source; }
            set { source = value; }
        }
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public string Link
        {
            get { return link; }
            set { link = value; }
        }
    }
}
