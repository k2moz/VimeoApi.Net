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

namespace ConsoleApplication1
{
	class Program
	{
		static void Main( string[] args )
		{
			Program p = new Program();
			//p.POSTUploadFileFromPath( "C://SASFiles/video.mp4" );
			
			p.PullUploadFileFromPath( "C://SASFiles/video.mp4" );
			Console.ReadKey();
		}

		/// <summary>
		/// 1. Simple HTTP POST uploading
		/// </summary>
		/// <param name="path">Local path from file</param>
		public void POSTUploadFileFromPath( string path )
		{

			var vc = Vimeo.VimeoClient.ReAuthorize(
			accessToken: "accessToken",
			cid: "cid",
			secret: "secret"
			 );

			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add( "type", "POST" );
			dic.Add( "redirect_url", "http://google.com/" );

			var my2 = vc.Request( "/me/videos", dic, "POST" );
			var linkSequere = my2["uri"].ToString();

			WebClient myWebClient = new WebClient();

			string fileName = path;
			byte[] responseArray = myWebClient.UploadFile( my2["upload_link_secure"].ToString(), "POST", fileName );

			var finaddr = myWebClient.BaseAddress;
			/*Console.WriteLine(System.Text.Encoding.ASCII.GetString( responseArray ) );*/
		}

		/// <summary>
		/// Resumable (Но это не точно) HTTP PUT uploads
		/// https://developer.vimeo.com/api/upload/videos
		/// </summary>
		/// <param name="videoFilePath">Local path to file</param>
		/// <param name="authToken">user oAuth token</param>
		/// <param name="tikket">Upload accessible token</param>
		/// <param name="uploadLinkSecure">secure link for upload action</param>
		/// <param name="completeUrl">link finished upload transaction</param>
		/// <returns>Exeption text or result video link </returns>
		public VimeoVideoWrapper PullUploadFileFromPath( string path )
		{
			
			var vc = Vimeo.VimeoClient.ReAuthorize(
						accessToken: "accessToken",
						cid: "cid",
						secret: "secret"
						 );
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add( "type", "streaming" );

			#region Test
			var test_var=GetThumb( "221619251", vc.AccessToken );
			#endregion
			VimeoVideoWrapper video = new VimeoVideoWrapper();
			var my2 = vc.Request( "/me/videos", dic, "POST" );
			var tikket = my2["uri"].ToString();

			try
			{
				Uri uri = new Uri( my2["upload_link_secure"].ToString() );
				
				video.VideoId = long.Parse(uri.Query.Split( '&' ).FirstOrDefault( x => x.StartsWith( "video_file_id=" ) ).Replace( "video_file_id=", "" )) ;
				video.VideoLink= SendFileToServer( path, vc.AccessToken, tikket, my2["upload_link_secure"].ToString(), my2["complete_uri"].ToString() );
				video.Thumb = GetThumb( video.VideoLink.Split( '/' ).Last(), vc.AccessToken ); 
			}
			catch(Exception e)
			{
				video.Error = true;
				video.ErrorText = e.Message;
			}
			return video;
		}

		/// <summary>
		/// Тут ещё нужно будет дописать логику на проверку "загрузился ли файл полностью" и вообще огонь будет
		/// </summary>
		/// <param name="videoFilePath"></param>
		/// <param name="authToken"></param>
		/// <param name="tikket"></param>
		/// <param name="uploadLinkSecure"></param>
		/// <param name="completeUrl"></param>
		/// <returns></returns>
		private string SendFileToServer( string videoFilePath, string authToken, string tikket, string uploadLinkSecure, string completeUrl )
		{

			StringBuilder sendData = new StringBuilder();

			using ( FileStream img = new FileStream( videoFilePath, FileMode.Open ) )
			{
				sendData.Append( "file_data=" + img.ToString() );
			}
			byte[] jsonBytes = Encoding.GetEncoding( 1251 ).GetBytes( sendData.ToString() );

			using ( var fileStream = new FileStream( videoFilePath, FileMode.Open ) )
			{

				HttpWebRequest request = ( HttpWebRequest )WebRequest.Create( uploadLinkSecure );

				request.Method = "PUT";
				request.Headers.Add( HttpRequestHeader.Authorization, "Bearer " + authToken );
				request.ContentLength = fileStream.Length;
				request.ContentType = "video/mp4";
				using ( Stream dataStream = request.GetRequestStream() )
				{
					byte[] buffer = new byte[fileStream.Length];
					var data = fileStream.Read( buffer, 0, buffer.Length );
					dataStream.Write( buffer, 0, data );
				}
				try
				{
					using ( HttpWebResponse response = ( HttpWebResponse )request.GetResponse() )
					{
						request = ( HttpWebRequest )WebRequest.Create( "https://api.vimeo.com"+completeUrl );

						request.Method = "DELETE";
						request.Headers.Add( HttpRequestHeader.Authorization, "Bearer " + authToken );

						using ( HttpWebResponse finishResponse = ( HttpWebResponse )request.GetResponse() )
						{
							return finishResponse.Headers["Location"];
						}

					}
				}
				catch ( WebException ex )
				{
					return ex.ToString() ;
				}
			}
		}

		private string GetThumb(string videoId,string authToken)
		{
			string result = "";
			HttpWebRequest request = ( HttpWebRequest )WebRequest.Create( "https://vimeo.com/api/oembed.json?url=https%3A//vimeo.com/"+videoId );

			request.Method = "GET";
			request.Headers.Add( HttpRequestHeader.Authorization, "Bearer " + authToken );

			using ( HttpWebResponse response = ( HttpWebResponse )request.GetResponse() )
			{
				var strreader = new StreamReader( response.GetResponseStream(), Encoding.UTF8 );
				var responseToString = strreader.ReadToEnd();
				var a = JsonConvert.DeserializeObject<JObject> (responseToString);
				return a["thumbnail_url"].ToString();
			}
			return result;
		}
	}
}
