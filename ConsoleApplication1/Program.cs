using ConsoleApplication1.API;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Vimeo;

namespace ConsoleApplication1
{
    class Program
    {


        static void Main(string[] args)
        {
            Program p = new Program();
            //p.POSTUploadFileFromPath( "C://SASFiles/video.mp4" );
            //var a = Auth.GetClientCredentials(accessToken, secret);
            //var aa = Auth.GetAuthURL(cid, null, "", "https://api.vimeo.com" );

            //var b = VimeoClient.Authorize(accessToken, cid, secret, "");
            //var a = Auth.GetAccessToken(accessToken, cid, secret, "", "https://api.vimeo.com");

            //VimeoContext.GetInstance();
            var inst = VimeoContext.GetInstance("1319131e75b3c70af11abb02cbfa554d", "6ea556d826e6539b10f9ace9504a3d03da98b1b2", "uLrPw2idERc11Qv1K2SCNcL0oU+uOTx1biLq5adCxNP0iwbhfUujZTO2ZdZZyIfSwJc6aNs8pvMuEW9LfKhi6cKFvDQoZ1QAYaGETl37ZOrbx7/A204ZlFwaHFHQX5W5");
            var file = inst.PullUploadFileFromPath("http://brainoteka.com/" + "/Content/uploads/users/4/2015612111446_1-введение в html(2).mp4");

            //var video = p.PullUploadFileFromPath("C://SAS/video.mp4");
            //p.SetPrivateStatus(video.VideoId.ToString(), "brainoteka.com");

            //p.SetPrivateStatus("329154891", "brainoteka.com");

            //Console.ReadKey();
        }


    }
}
