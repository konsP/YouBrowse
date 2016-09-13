using System;
using System.IO;




namespace YouBrowse
{
    public class Home : History
    {
        public Home(string url)
            : base(url)
        {
            //this.Url = url; //inherited from History.cs
        }
        //getUrl and setUrl inherited from History.cs

        public void saveHome(string pathHome, string homeurl)
        {
            System.IO.File.WriteAllText(pathHome, homeurl);
        }
    }
}