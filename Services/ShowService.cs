using Microsoft.EntityFrameworkCore;
using StarterKit.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using StarterKit.Models;
using Microsoft.EntityFrameworkCore;

using System.Text.Json;
using Services;

namespace Services
{
    public class ShowService
    {
        private readonly DatabaseContext _context;

        public ShowService(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<List<TheatreShow>> GetShows()
        {
            string connectionString = @"Data Source=webdev.sqlite";
            string query = @"
            SELECT 
                ts.TheatreShowId, 
                ts.Title, 
                ts.Description, 
                ts.Price, 
                v.VenueId, 
                v.Name as VenueName, 
                v.Capacity as VenueCapacity,
                tsd.TheatreShowDateId, 
                tsd.DateAndTime as ShowDate
            FROM 
                TheatreShow ts
            LEFT JOIN 
                Venue v ON ts.VenueId = v.VenueId
            LEFT JOIN 
                TheatreShowDate tsd ON ts.TheatreShowId = tsd.TheatreShowId
            ORDER BY 
                ts.TheatreShowId, tsd.DateAndTime;
        ";

            List<TheatreShow> shows = new List<TheatreShow>();


            using (var connection = new SqliteConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Dictionary to avoid duplicate shows
                    var showMap = new Dictionary<int, TheatreShow>();

                    while (await reader.ReadAsync())
                    {
                        int showId = reader.GetInt32(reader.GetOrdinal("TheatreShowId"));

                        // Checken of de show al in de map zit zo niet maakt die hem aan.
                        if (!showMap.ContainsKey(showId))
                        {
                            var show = new TheatreShow
                            {
                                TheatreShowId = showId,
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDouble(reader.GetOrdinal("Price")),
                                Venue = new Venue
                                {
                                    VenueId = reader.GetInt32(reader.GetOrdinal("VenueId")),
                                    Name = reader.IsDBNull(reader.GetOrdinal("VenueName")) ? null : reader.GetString(reader.GetOrdinal("VenueName")),
                                    Capacity = reader.GetInt32(reader.GetOrdinal("VenueCapacity"))
                                },
                                theatreShowDates = new List<TheatreShowDate>()
                            };

                            showMap[showId] = show;
                        }

                        // Add the show date if available
                        if (!reader.IsDBNull(reader.GetOrdinal("ShowDate")))
                        {
                            var showDate = new TheatreShowDate
                            {
                                TheatreShowDateId = reader.GetInt32(reader.GetOrdinal("TheatreShowDateId")),
                                DateAndTime = reader.GetDateTime(reader.GetOrdinal("ShowDate"))
                            };

                            showMap[showId].theatreShowDates.Add(showDate);
                        }
                    }

                    // Convert the dictionary to a list
                    shows = new List<TheatreShow>(showMap.Values);
                    return shows;
                }
            }


        }
        public async Task<TheatreShow> ShowWithId(int id)
        {
            var show = await _context.TheatreShow
                                .Include(s => s.Venue)
                                .Include(s => s.theatreShowDates)
                                .FirstOrDefaultAsync(s => s.TheatreShowId == id);
            return show;
        }

        public async Task<List<TheatreShow>> ShowFilterTitle(string filter)
        {
            var query = _context.TheatreShow.AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower(); // Convert the filter to lowercase

                query = query.Where(ts =>
                    (ts.Title != null && ts.Title.ToLower().Contains(filter)) || // Compare lowercase strings
                    (ts.Description != null && ts.Description.ToLower().Contains(filter))); // Compare lowercase strings
            }

            return await query.ToListAsync();
        }
        public async Task<List<TheatreShow>> ShowFilterLocationAsync(int location)
        {
            var shows = await _context.TheatreShow
                                      .Include(ts => ts.Venue)
                                      .Where(ts => ts.Venue.VenueId == location)
                                      .ToListAsync();

            return shows;
        }
        public async Task<List<dynamic>> FilterShowsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Fetch all venues and their shows
            var venues = await _context.Venue
                .Include(v => v.TheatreShows)
                    .ThenInclude(ts => ts.theatreShowDates)
                .ToListAsync();

            // Filter shows based on the date range
            var filteredShows = venues
                .SelectMany(v => v.TheatreShows, (v, ts) => new { Venue = v, TheatreShow = ts })
                .SelectMany(ts => ts.TheatreShow.theatreShowDates
                    .Where(tsd => tsd.DateAndTime >= startDate && tsd.DateAndTime <= endDate)
                    .Select(tsd => (dynamic)new // Cast the anonymous type to dynamic here
                    {
                        tsd.TheatreShowDateId,
                        tsd.DateAndTime,
                        TheatreShowTitle = ts.TheatreShow.Title,
                        VenueName = ts.Venue.Name,
                        Price = ts.TheatreShow.Price
                    }))
                .ToList();

            return filteredShows;
        }
        public async Task<string> CreateShowAsync(TheatreShow newShow)
        {
            // Detach the venue if it exists to avoid re-inserting it
            var existingVenue = await _context.Venue.FindAsync(newShow.Venue.VenueId);

            if (existingVenue == null)
            {
                return "Venue not found.";
            }

            // Assign the existing venue to the new show
            newShow.Venue = existingVenue;

            _context.TheatreShow.Add(newShow);
            await _context.SaveChangesAsync();
            return "Show created successfully.";
        }

        public async Task<string> UpdateShowAsync(int id, TheatreShow updatedShow)
        {
            // Find the existing show by id
            var oldShow = await _context.TheatreShow.Include(s => s.theatreShowDates)
                                                    .Include(s => s.Venue)
                                                    .FirstOrDefaultAsync(s => s.TheatreShowId == id);

            if (oldShow == null)
            {
                return "Show not found.";
            }

            // Update the Title, Description adn the Price
            oldShow.Title = updatedShow.Title;
            oldShow.Description = updatedShow.Description;
            oldShow.Price = updatedShow.Price;

            // Update the venue
            if (oldShow.Venue.VenueId != updatedShow.Venue.VenueId)
            {
                var existingVenue = await _context.Venue.FindAsync(updatedShow.Venue.VenueId);
                if (existingVenue != null)
                {
                    oldShow.Venue = existingVenue;
                }
                else
                {
                    return "Venue not found.";
                }
            }

            // Update theatre show dates
            oldShow.theatreShowDates.Clear();

            foreach (var date in updatedShow.theatreShowDates)
            {
                oldShow.theatreShowDates.Add(new TheatreShowDate
                {
                    DateAndTime = date.DateAndTime
                });
            }
            await _context.SaveChangesAsync();
            return "Show updated successfully.";
        }

        public async Task<string> DeleteShowAsync(int id)
        {
            // Find the show by id
            var show = await _context.TheatreShow.Include(s => s.theatreShowDates)
                                                 .FirstOrDefaultAsync(s => s.TheatreShowId == id);

            if (show == null)
            {
                return "Show not found.";
            }

            // Remove the associated TheatreShowDates and the show
            _context.TheatreShowDate.RemoveRange(show.theatreShowDates);
            _context.TheatreShow.Remove(show);

            await _context.SaveChangesAsync();
            return "Show deleted successfully.";
        }

    }


}