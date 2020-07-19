using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TelegramWeatherBot.Weather
{
    class SaveCoord
    {
        int i=0;
        List<List<string>> list = new List<List<string>>();
        internal void SetCoord(string id, float latitude, float longitude)
        {
            if (list.Count == 0)        //Если коллекция пустая
            {
                list.Add(new List<string>());
                list[i].Add(id);
                list[i].Add(latitude.ToString());
                list[i].Add(longitude.ToString());
                i++;
            }
            else
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j][0] == id)           //Проверяет существует ли такой же id в коллекции, если да то заменяет старые координаты на новые
                    {
                        list[j][0] = id;
                        list[j][1] = latitude.ToString();
                        list[j][2] = longitude.ToString();
                        break;
                    }
                    if (j == list.Count - 1)    //Если такой же id не найден
                    {
                        list.Add(new List<string>());
                        list[i].Add(id);
                        list[i].Add(latitude.ToString());
                        list[i].Add(longitude.ToString());
                        i++;
                    }
                }
            }
        }

        internal string GetCoord(string id) //Возвращает координаты по id в таком формате latitude/longtitude
        {
            if (list != null && list.Count > 0 && list[0] != null)
            {
                for (int j = 0; j < list.Count(); j++)
                {
                    if (list[j][0] == id)
                    {
                        return list[j][1] + "/" + list[j][2];
                    }
                    return "error";
                }
            }
            return "error";
        }

    }
}
