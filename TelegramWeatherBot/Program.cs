using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramWeatherBot
{
    static class Program
    {
        public static string token = "YOUR_TELEGRAM_TOKEN_HERE";
        public static string openWeatherAppId = "YOUR_OPENWEATHER_APPID_HERE";
        public static BOT bot;
        static void Main()
        {
            bot = new BOT();
            if (token.Length == 45)
            {
                bot.SetToken(token);
                bot.Start(bot.GetBot());
                Console.WriteLine("Name : " + bot.GetName(bot.GetBot()));
                Console.WriteLine("BOT is Online");
            }
            else
            {
                Console.WriteLine("Error wrong token");
            }
        }
    }

    public class BOT
    {
        Weather.Weather weather = new Weather.Weather();
        Weather.SaveCoord coord = new Weather.SaveCoord();


        TelegramBotClient bot;
        public void SetToken(string TOKEN)
        {
            bot = new TelegramBotClient(TOKEN);
        }

        public TelegramBotClient GetBot()         // Возвращает текущий токен
        {
            return bot;
        }

        public string GetName(TelegramBotClient bot)        // Полчуаем имя бота
        {
            var name = bot.GetMeAsync().Result;
            return name.Username.ToString();
        }


        public void Start(TelegramBotClient bot)
        {
            bot.OnMessage += Bot_OnMessage;

            bot.StartReceiving();   // Разрешаем приём сообщений
        }

        async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var msg = e.Message;
            async void RemoveK()
            {
                var rmv = new ReplyKeyboardRemove();
                
                await bot.SendTextMessageAsync(msg.Chat.Id, "Done", Telegram.Bot.Types.Enums.ParseMode.Default, false, false, 0, replyMarkup: rmv);
                await bot.SendChatActionAsync(msg.Chat.Id, Telegram.Bot.Types.Enums.ChatAction.Typing);
            };  // Метод который убирает клавиатуру

            if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                if (msg.Text == "Remove Keyboard")
                {
                    RemoveK();
                }

                if (msg.Text == "/weather" || msg.Text == "/погода")
                {
                    var RKM = new ReplyKeyboardMarkup();
                    RKM.Keyboard = new KeyboardButton[][]
                    {
                              new KeyboardButton []
                              {
                                  new KeyboardButton("Погода"){ RequestLocation = true},
                              },
                              new KeyboardButton[]
                              {
                                new KeyboardButton("Remove Keyboard")
                              }
                    };
                    await bot.SendTextMessageAsync(msg.Chat.Id, "Requesting location..", replyMarkup: RKM);
                }

                if (msg.Text == "Погода сегодня")
                {
                    var rmv = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardRemove();
                    string cord = coord.GetCoord(msg.Chat.Id.ToString());
                    if (cord != "error" || cord != null)
                    {
                        string lat = cord.Split('/')[0];
                        string lon = cord.Split('/')[1];
                        await bot.SendTextMessageAsync(msg.Chat.Id, weather.GetForecast(lat, lon, "today"), replyMarkup: rmv);
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(msg.Chat.Id, "Координаты не указаны", replyMarkup: rmv);
                    }
                }

                if (msg.Text == "Прогноз погоды")
                {
                    var rmv = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardRemove();
                    string cord = coord.GetCoord(msg.Chat.Id.ToString());
                    if (cord != "error" || cord != null)
                    {
                        string lat = cord.Split('/')[0];
                        string lon = cord.Split('/')[1];
                        await bot.SendTextMessageAsync(msg.Chat.Id, weather.GetForecast(lat, lon, "3 days"));
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(msg.Chat.Id, "Коррдинаты не указаны", replyMarkup: rmv);
                    }
                }

                if (msg.Text.StartsWith("/city"))
                {
                    try
                    {
                        string txt = msg.Text.ToString();
                        string[] split = txt.Split(' ');
                        string city = split[1];
                        await bot.SendTextMessageAsync(msg.Chat.Id, weather.SetCity(city));
                    }
                    catch
                    {
                        await bot.SendTextMessageAsync(msg.Chat.Id, "Некорректное выражение\n" + "/city cityname");
                    }
                }
            }

            if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Location)
            {
                if (msg.Location.Latitude > 0.01)
                {
                    if (msg.ReplyToMessage.Text == "Requesting location..")
                    {
                        var forecast = new ReplyKeyboardMarkup();
                        forecast.Keyboard = new KeyboardButton[][]
                        {
                                new KeyboardButton []
                              {
                                  new KeyboardButton("Погода сегодня"),
                                  new KeyboardButton("Прогноз погоды"),
                              },
                              new KeyboardButton[]
                              {
                                new KeyboardButton("Remove Keyboard")
                              }
                        };
                        coord.SetCoord(msg.Chat.Id.ToString(), msg.Location.Latitude, msg.Location.Longitude);
                        string urlocation = "Ширина : " + coord.GetCoord(msg.Chat.Id.ToString()).Split('/')[0] + "\nДолгота : " + coord.GetCoord(msg.Chat.Id.ToString()).Split('/')[1];
                        await bot.SendTextMessageAsync(msg.Chat.Id, urlocation, replyMarkup: forecast);
                    }
                }
                else { await bot.SendTextMessageAsync(msg.Chat.Id, "Something went wrong"); }
            }

            if (msg == null || msg.Type != Telegram.Bot.Types.Enums.MessageType.Text) return;
        }

        public void Stop(TelegramBotClient bot)
        {
            bot.StopReceiving();
        }
    }
}
