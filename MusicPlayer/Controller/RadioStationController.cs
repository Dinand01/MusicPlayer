using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicPlayer.Helpers;
using MusicPlayer.Interface;
using MusicPlayer.Models;

namespace MusicPlayer.Controller
{
    /// <summary>
    /// A controller for handling radio station data.
    /// </summary>
    internal class RadioStationController : IRadioStation
    {
        private readonly Db _db;

        public RadioStationController(Db db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets a station by it's id.
        /// </summary>
        /// <param name="id">The station id.</param>
        /// <returns>The station.</returns>
        public Task<RadioStation> GetStation(int id)
        {
            return _db.RadioStations.SingleOrDefaultAsync(s => s.ID == id);
        }

        /// <summary>
        /// Gets a station by it's url.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <returns>The station.</returns>
        public Task<RadioStation> GetStation(string url)
        {
            url = url.ToLower();
            return _db.RadioStations.SingleOrDefaultAsync(s => s.Url.ToLower() == url);
        }

        /// <summary>
        /// Gets the radio stations for the provided searchtext.
        /// </summary>
        /// <param name="searchText">The searchtext.</param>
        /// <returns>The radio stations.</returns>
        public async Task<List<RadioStation>> GetStations(string searchText)
        {
            searchText = searchText.ToLower();
            var stations = await _db.RadioStations
                            .Where(s =>
                                searchText == null
                                || searchText == string.Empty
                                || s.Name.ToLower().Contains(searchText)
                                || s.Genre.ToLower().Contains(searchText)
                                || s.Url.ToLower().Contains(searchText))
                            .ToListAsync();
            if (stations?.Count > 0)
            {
                return stations;
            }

            if (!await _db.RadioStations.AnyAsync() && await RefreshDirbleStations())
            {
                return await GetStations(searchText);
            }

            return new List<RadioStation>();
        }

        /// <summary>
        /// Refreshes the radio stations from dirble.
        /// </summary>
        /// <returns>A value indicating whether stations were added to the database.</returns>
        public async Task<bool> RefreshDirbleStations()
        {
            var stations = await GetFromDirble();
            if (stations.Any())
            {
                await AddStations(stations: stations);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the radio stations to the database.
        /// </summary>
        /// <param name="station">The radio station.</param>
        /// <param name="stations">Multiple radio stations.</param>
        /// <returns>A task.</returns>
        public async Task AddStations(RadioStation station = null, IEnumerable<RadioStation> stations = null)
        {
            if (stations == null)
            {
                stations = new List<RadioStation>();
            }
            else
            {
                stations = stations.ToList();
            }

            if (station != null)
            {
                ((List<RadioStation>)stations).Add(station);
            }

            if (stations.Count() > 0)
            {
                string[] urls = stations.Select(s => s.Url.ToLower()).ToArray();
                var existing = await _db.RadioStations.Where(s => urls.Contains(s.Url.ToLower())).ToListAsync();
                stations = stations.Where(s => !existing.Any(es => es.Url.ToLower() == s.Url.ToLower())).ToList();
                _db.RadioStations.AddRange(stations);
                await _db.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Updates an existing radio station.
        /// </summary>
        /// <param name="station">The radio station with the desired values.</param>
        /// <returns>A task.</returns>
        public Task UpdateStation(RadioStation station)
        {
            _db.Entry(station).State = EntityState.Modified;
            return _db.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the radio station.
        /// </summary>
        /// <param name="id">The id of the radio station.</param>
        /// <returns>The task.</returns>
        public async Task DeleteStation(int id)
        {
            _db.RadioStations.RemoveRange(_db.RadioStations.Where(r => r.ID == id));
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the radio stations from dirble (Popular stations).
        /// </summary>
        /// <returns>The radio stations.</returns>
        private async Task<List<RadioStation>> GetFromDirble()
        {
            try
            {
                using (var client = new DirbleClient())
                {
                    List<RadioStation> stations = new List<RadioStation>();
                    for (int i = 1; i <= 5; i++)
                    {
                        stations.AddRange((await client.GetPopularStations(i))?.Select(s => new RadioStation(s)));
                    }

                    return stations;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Dirble radio station retrieval failed.");
                return null;
            }
        }
    }
}
