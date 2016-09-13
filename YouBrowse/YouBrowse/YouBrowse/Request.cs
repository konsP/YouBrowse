using System;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace YouBrowse
{
    public class Request
    {
        public string url;
        public Request(string url)
        {
            this.url = url;
        }
        public string getUrl() { return this.url; }
        public void setUrl(string url) { this.url = url; }
        //long ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
        public string getPage(string url, BackgroundWorker worker, DoWorkEventArgs e)
        {

            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            else
            {

                try
                {//check the string format
                    if (url.StartsWith("http")) /*also includes https://www., http://www., https://")*/
                    {
                        //do nothing
                    }
                    else if (url.StartsWith("www."))
                    {
                        url = "http://" + url;
                    }
                    else /*http://www. omitted*/
                    {
                        url = "http://www." + url;
                    }



                    //Initialization
                    HttpWebRequest webrq = (HttpWebRequest)WebRequest.Create(url);      

                    //Method = "GET";

                    HttpWebResponse webrs = (HttpWebResponse)webrq.GetResponse();

                    if (webrs.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //read the response
                        Stream answer = webrs.GetResponseStream();
                        StreamReader theAnswer = new StreamReader(answer);

                        //return the response
                        string result = theAnswer.ReadToEnd();
                        return result;
                    }


                    webrs.Close();
                    return "";
                }


                catch (WebException we)
                {


                    if (we.Status == WebExceptionStatus.ProtocolError)
                    {
                        int code = (Int32)((HttpWebResponse)we.Response).StatusCode;
                      
                        // 404 NotFound or 403 Forbidden
                        return "Status Code :  " + code + ((HttpWebResponse)we.Response).StatusCode;

                    }
                    else
                    {
                        //400 BadRequest	
                        return "Status Code : 400 Bad Request " + we.ToString();

                    }


                }
                catch (Exception ex)
                {
                    return "An unknown exception occured : " + ex.Message;
                }

            }
            return "";

        }

    }
}





