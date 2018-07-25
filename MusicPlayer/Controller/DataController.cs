using MusicPlayer.EmbeddedData;
using MusicPlayer.Interface;
using MusicPlayer.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The controller to access app data.
    /// </summary>
    public static class DataController
    {
        /// <summary>
        /// The lock for changing settings.
        /// </summary>
        private static readonly object _settingLock = new object();

        /// <summary>
        /// Gets a setting value.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="setting">The setting to retrieve.</param>
        /// <param name="def">The default value of the setting.</param>
        /// <returns>The setting value.</returns>
        public static T GetSetting<T>(SettingType setting, T def)
        {
            using (var db = new Db())
            {
                var dbval = db.Settings.Find(setting.ToString());
                if (dbval != null)
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<T>(dbval.Value);
                    }
                    catch
                    {
                        return def != null ? def : default(T);
                    }
                }

                return def != null ? def : default(T);
            }
        }

        /// <summary>
        /// Sets a setting.
        /// </summary>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <param name="setting">The setting to set.</param>
        /// <param name="value">The value to set.</param>
        public static void SetSetting<T>(SettingType setting, T value)
        {
            lock (_settingLock)
            {
                using (var db = new Db())
                {
                    var set = db.Settings.Find(setting.ToString());
                    if (set == null)
                    {
                        set = new Setting
                        {
                            Name = setting.ToString(),
                            Value = JsonConvert.SerializeObject(value)
                        };

                        db.Settings.Add(set);
                    }
                    else
                    {
                        set.Value = JsonConvert.SerializeObject(value);
                        db.Entry(set).CurrentValues.SetValues(set);
                    }

                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Gets the radio stations.
        /// </summary>
        /// <returns>The radio stations.</returns>
        public static List<RadioStation> GetStations()
        {
            using (var db = new Db())
            {
                if (!db.RadioStations.Any())
                {
                    db.RadioStations.AddRange(RadioStations.Get());
                    db.SaveChanges();
                }

                return db.RadioStations.OrderByDescending(r => r.Priority).ToList();
            }
        }
    }
}
