using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace DiscordSCLDbot215
{
    class SCLD_tools
    {
        String br = Convert.ToChar(13).ToString() + Convert.ToChar(10).ToString(); //打/n也可以

        public void write_log_file(string content)
        {
            string path = "log/";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string file_name = DateTime.Now.ToString("yyyyMMdd");

            System.IO.File.AppendAllText(path + file_name + ".txt", content + "\n");
        }



        public String post_line_notify(String the_token, String the_message)
        {
            //line 通知 API 的網址
            string url = "https://notify-api.line.me/api/notify";


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = System.Web.HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("message", br + the_message);



            //Console.WriteLine(postParams.ToString());// 將取得"version=1.0&action=preserveCodeCheck&pCode=pCode&TxID=guid&appId=appId", key和value會自動UrlEncode
            //要發送的字串轉為byte[] 
            byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }//end using

            //API回傳的字串
            string responseStr = "";
            //發出Request

            request.Headers["Authorization"] = "Bearer " + the_token;
            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    responseStr = sr.ReadToEnd();
                }//end using  
            }




            return responseStr;

        }


        public String post_line_notify(String the_message)
        {
            String[] AccessToken = System.IO.File.ReadAllLines("SCLD_Token.txt");
            return post_line_notify(AccessToken[0], the_message);
        }

    }
}
