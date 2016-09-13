using System;
using System.IO;




namespace YouBrowse
{
    public class History
    {

        public string Url;

        public History(string url)
        {
            this.Url = url;
        }

        public string getUrl() { return this.Url; }
        public void setUrl(string url) { this.Url = url; }

        public void appendHistory(string url, string path)
        {
            using (System.IO.StreamWriter file = File.AppendText(path))
            {
                file.WriteLine(url);
            }


        }


        /* public void save()
        {
            History[] array1 ;//= new string[listBox1.Items.Count];
            

            for (int i = 0; i < array1.Length; i++)
            {
                object s = listBox1.Items[i];
                if (s != null)
                {
                    urls[i] = s.ToString();
                    names[i] = s.ToString();
                }
                else
                {
                    i--;
                    break;
                }
            }*/


    }
}