using System;
using System.IO;





namespace YouBrowse
{
    public class Favorite : History
    {

        public string Name;
        public Favorite(string url, string name)
            : base(url)
        {
            this.Name = name;
        }

        public string getName() { return this.Name; }
        public void setName(string name) { this.Name = name; }


        public void appendFavorite(string url, string name, string pathl, string pathn)
        {
            using (System.IO.StreamWriter file = File.AppendText(pathl))
            {
                file.WriteLine(url);
            }
            using (System.IO.StreamWriter file = File.AppendText(pathn))
            {
                file.WriteLine(name);
            }

        }

        
    }
}