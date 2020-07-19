using System.Net;   // webrequests
using System.IO;
using Newtonsoft.Json;



namespace TelegramWeatherBot.Weather
{
    class Weather
    {
        public string SetCity(string city)
        {
            string url = "http://api.openweathermap.org/data/2.5/weather?q=" + city + "&units=metric&appid=" + Program.openWeatherAppId;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string response;

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            var obj = JsonConvert.DeserializeObject<WeatherInfo.Root>(response);

            string gorod = obj.name.ToString();
            string weather = obj.weather[0].description.ToString();
            string temp = obj.main.temp.ToString();
            double pressure = obj.main.pressure * 0.75;
            string weathermessage = "Погода в " + gorod + "\nСейчас: " + temp + "°C\n" + weather + "\nДавление : " + pressure.ToString() + "мм";

            return weathermessage;
        }

        public string GetForecast(string lat, string lon, string when)
        {
            string url = "http://api.openweathermap.org/data/2.5/forecast?lat=" + lat + "&lon=" + lon + "&units=metric&appid=" + Program.openWeatherAppId;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            string response;

            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            var obj = JsonConvert.DeserializeObject<WeatherInfoCoord.RootObject>(response);

            string todaydate = obj.list[0].dt_txt.ToString().Split('-')[2].Split(' ')[0]; //Возращает число месяца

            if (when == "3 days")
            {
                int tomorrowid = 0, tomorrowid2 = 0, tomorrowid3 = 0;
                for (int i = 0; i < 9; i++)
                {
                    if (todaydate != obj.list[i].dt_txt.ToString().Split('-')[2].Split(' ')[0])
                    {
                        tomorrowid = i + 3;             // i = завтрашний день в 00:00:00 //i+3 = завтрашний день в 09:00:00
                        tomorrowid2 = i + 11;           //i+11 = послезавтра в 09:00:00
                        tomorrowid3 = i + 19;          //i+19 = послепослезавтра в 09:00:00
                        break;
                    }
                }

                string tomorrowweather = obj.list[tomorrowid].dt_txt.Split(' ')[0] +
                    "\nУтро : " + obj.list[tomorrowid].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid].weather[0].main + " (" + obj.list[tomorrowid].weather[0].description + ")" +
                    "\nДень : " + obj.list[tomorrowid + 2].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid + 2].weather[0].main + " (" + obj.list[tomorrowid + 2].weather[0].description + ")" +
                    "\nВечер : " + obj.list[tomorrowid + 3].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid + 3].weather[0].main + " (" + obj.list[tomorrowid + 3].weather[0].description + ")";

                string tomorrowweather2 = obj.list[tomorrowid2].dt_txt.Split(' ')[0] +
                    "\nУтро : " + obj.list[tomorrowid2].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid2 + 2].weather[0].main + " (" + obj.list[tomorrowid2].weather[0].description + ")" +
                    "\nДень : " + obj.list[tomorrowid2 + 2].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid2 + 2].weather[0].main + " (" + obj.list[tomorrowid2 + 2].weather[0].description + ")" +
                    "\nВечер : " + obj.list[tomorrowid2 + 3].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid2 + 3].weather[0].main + " (" + obj.list[tomorrowid2 + 3].weather[0].description + ")";

                string tomorrowweather3 = obj.list[tomorrowid3].dt_txt.Split(' ')[0] +
                    "\nУтро : " + obj.list[tomorrowid3].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid3].weather[0].main + " (" + obj.list[tomorrowid3].weather[0].description + ")" +
                    "\nДень : " + obj.list[tomorrowid3 + 2].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid3 + 2].weather[0].main + " (" + obj.list[tomorrowid3 + 2].weather[0].description + ")" +
                    "\nВечер : " + obj.list[tomorrowid3 + 3].main.temp_min.ToString() + "°C\n" + obj.list[tomorrowid3 + 3].weather[0].main + " (" + obj.list[tomorrowid3 + 3].weather[0].description + ")";

                if (tomorrowweather.Contains("Clouds")) { tomorrowweather += "\n \U00002601"; }
                if (tomorrowweather.Contains("Rain")) { tomorrowweather += "\n \U00002614"; }

                if (tomorrowweather2.Contains("Clouds")) { tomorrowweather2 += "\n \U00002601"; }
                if (tomorrowweather2.Contains("Rain")) { tomorrowweather2 += "\n \U00002614"; }

                if (tomorrowweather3.Contains("Clouds")) { tomorrowweather3 += "\n \U00002601"; }
                if (tomorrowweather3.Contains("Rain")) { tomorrowweather3 += "\n \U00002614"; }

                return tomorrowweather + "\n - - - -\n" + tomorrowweather2 + "\n - - - -\n" + tomorrowweather3;
            }

            if (when == "today")
            {
                int j = 0;
                string weathertoday = obj.list[j].dt_txt.ToString().Split(' ')[0] + "\n- - - -";     //Пример :  obj.list[0].dt_txt == 2017-09-26  09:00:00
                do
                {
                    weathertoday += "\n" + obj.list[j].dt_txt.Split(' ')[1] + ": " + obj.list[j].main.temp_min.ToString() + "°C\n" + obj.list[j].weather[0].main + " (" + obj.list[j].weather[0].description + ")" + "\n- - - -";
                    j++;
                }
                while (obj.list[j].dt_txt.ToString().Split('-')[2].Split(' ')[0] == todaydate);
                if (weathertoday.Contains("Clouds")) { weathertoday += "\n \U00002601"; }
                if (weathertoday.Contains("Rain")) { weathertoday += "\n \U00002614"; }
                return weathertoday;
            }
            else
            {
                return "error";
            }
        }
    }
}
