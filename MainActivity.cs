using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System;

namespace Movie_Search
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        /*
         * Declaring variables
         */
        public string JSONresponse;
        public static string MovieTitle;
        public static string MovieBio;
        public static string MovieRating;
        public static string MovieReleased;
        public static Button SearchButton;
        private EditText movieName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            SearchButton = FindViewById<Button>(Resource.Id.Button_Search);
            movieName = (EditText)FindViewById(Resource.Id.moviename);

            /*
            * Implementing search function
            * (Triggers method: MovieSearch)
            * Starts Activity: MovieActivity
            */

            SearchButton.Click += delegate {
                SearchButton.Text = "Please Wait. Fetching info.";
                MovieSearch(movieName.Text.Replace(" ", "+"));
                StartActivity(new Intent(this, typeof(MovieActivity)));
            };

            /*
            * Implementing exit function
            * (Exits app)
            */
            var buttonExit = FindViewById<Button>(Resource.Id.Button_EXIT);
            buttonExit.Click += (sender, e) => {
                this.Finish();
            };
        }

        private void MovieSearch(string moviename)
        {
            /*
            * New HTTP Web Request
            * Sends get request to API
            */
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.themoviedb.org/3/search/movie?api_key=a3bdaae66f8cf705750820e17c0e9471&query="+ moviename);
            try
            {
                /*
                * Response from the request
                * Using StreamReader class to read the response
                */
                WebResponse webResponse = httpWebRequest.GetResponse();
                using (Stream responseStream = webResponse.GetResponseStream())
                {
                    StreamReader streamReader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JSONresponse = streamReader.ReadToEnd();

                    /*
                    * JSON Parsed and stored in a dynamic variable
                    */
                    dynamic data = JObject.Parse(JSONresponse);

                    try
                    {
                        data = data.results[0];
                        MovieTitle = data.title;
                        MovieBio = data.overview;
                        MovieRating = data.vote_average;
                        MovieReleased = data.release_date;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
            }

            /*
            * Process is finished upon exceptions
            */
            catch (WebException ex)
            {
                SearchButton.Text = "Failed";
                this.Finish();
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}