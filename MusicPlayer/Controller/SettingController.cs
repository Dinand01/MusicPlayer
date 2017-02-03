using MusicPlayer.DAL;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// The settings controller.
    /// </summary>
    public static class SettingController
    {
        /// <summary>
        /// Gets the setting value.
        /// </summary>
        /// <param name="setting">The setting to get.</param>
        /// <returns>The value.</returns>
        public static string Get(SettingType setting)
        {
            var set = GetSetting(setting);
            return set.Value;
        }

        /// <summary>
        /// Updates the setting.
        /// </summary>
        /// <param name="setting">The setting to update.</param>
        /// <param name="value">The value to update the setting to.</param>
        public static void Set(SettingType setting, string value)
        {
            var set = GetSetting(setting);
            set.Value = value;
            DbContextStore.Ctrl.Context.Entry(set).CurrentValues.SetValues(set);
            DbContextStore.Ctrl.Context.SaveChanges();
        }

        /// <summary>
        /// Gest the db setting, ensures it exists.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>The db setting.</returns>
        private static Setting GetSetting(SettingType setting)
        {
            var set = DbContextStore.Ctrl.Context.Settings.FirstOrDefault(s => s.Name == setting.ToString());
            if (set == null)
            {
                set = new Setting
                {
                    Name = setting.ToString(),
                    Value = string.Empty
                };

                set = DbContextStore.Ctrl.Context.Settings.Add(set);
                DbContextStore.Ctrl.Context.SaveChanges();
            }

            return set;
        }
    }
}
