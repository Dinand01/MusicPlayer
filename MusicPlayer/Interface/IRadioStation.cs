using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;

namespace MusicPlayer.Interface
{
    /// <summary>
    /// The interface for handling radio station data.
    /// </summary>
    public interface IRadioStation
    {
        /// <summary>
        /// Gets the radio stations for the provided searchtext.
        /// </summary>
        /// <param name="searchText">The searchtext.</param>
        /// <returns>The radio stations.</returns>
        Task<List<RadioStation>> GetStations(string searchText);

        /// <summary>
        /// Refreshes the radio stations from dirble.
        /// </summary>
        /// <returns>A value indicating whether stations were added to the database.</returns>
        Task<bool> RefreshDirbleStations();

        /// <summary>
        /// Adds the radio stations to the database.
        /// </summary>
        /// <param name="station">The radio station.</param>
        /// <param name="stations">Multiple radio stations.</param>
        /// <returns>A task.</returns>
        Task AddStations(RadioStation station = null, IEnumerable<RadioStation> stations = null);
    }
}
