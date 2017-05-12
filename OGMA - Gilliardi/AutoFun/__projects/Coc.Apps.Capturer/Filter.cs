using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Coc.Apps.Capturer
{
   public class Filter
    {
       public static String TDactionValue { get; private set; }

        public static bool FitrarPictures(String url)
        {
            return (
                url.EndsWith("gif") ||
                url.EndsWith("jpg") ||
                url.EndsWith("png") ||
                url.EndsWith("css") ||
                url.EndsWith("js")  ||
                url.EndsWith("pbg") ||
                url.Contains(".gif")||
                url.EndsWith("swf"));

        }

        public static string iShttps(String url)
        {
            if (!url.Contains("http://") && !url.Contains("https://"))
            {

                url = "http://" + url;
            
            }

            if (url.EndsWith("443") == true)
            {
                url = url.Replace(":443", "");
                if (url.Contains("http") == true)
                {
                    url = url.Replace("http", "https");

                }
            }
            return url;
        }


        public static String filtrarNome(String url)
        {
            string urlAux = url;
            if (url.Contains(".")) {
                string[] lines = urlAux.Split('.');
                return lines[1];
            }
          
            return urlAux;

        }

       public static String contadorUrl(List<String> lista,String url){
          
           int count = -1;
           for (int i = 0; i < lista.Count; i++) {
               String urlAux = lista[i];
               if (urlAux.Equals(url)) {
                   count++;               
               }
           }
             return count+""; 
      }

      public static String filtraParam(String url) {
            if (!url.Contains("?")) {
                TDactionValue = url;
                return "";
            }
            string[] str = url.Split('?');
            TDactionValue = str[0].Replace("?","");

            if (str.Length > 1)
            {
                url = str[1].Replace("=", "@@").Replace("&", "|");
            }
            return url;
        
        }

      public static bool isGet(String url) {
          return url.Contains("?");
      }
    }
}
