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

        // Methode om een lijst met alle shows op te halen, inclusief locatie en datums.
        public async Task<List<TheatreShow>> GetShows()
        {
            string connectionString = @"Data Source=webdev.sqlite"; // SQLite-verbinding.
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

            List<TheatreShow> shows = new List<TheatreShow>(); // Lijst van shows.


            using (var connection = new SqliteConnection(connectionString)) // Maakt verbinding met de SQLite database.
            {
                await connection.OpenAsync();

                using (var command = new SqliteCommand(query, connection)) // Voert SQL-query uit.
                using (var reader = await command.ExecuteReaderAsync()) // Leest de resultaten van de query.
                {
                    var showMap = new Dictionary<int, TheatreShow>(); // Dictionary om dubbele shows te vermijden.

                    while (await reader.ReadAsync()) // Loopt door de query resultaten.
                    {
                        int showId = reader.GetInt32(reader.GetOrdinal("TheatreShowId"));

                        // Controleert of de show al in de dictionary staat, anders maakt het een nieuwe aan.
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

                        // Voeg showdatum toe als deze beschikbaar is.
                        if (!reader.IsDBNull(reader.GetOrdinal("ShowDate")))
                        {
                            var showDate = new TheatreShowDate
                            {
                                TheatreShowDateId = reader.GetInt32(reader.GetOrdinal("TheatreShowDateId")),
                                DateAndTime = reader.GetDateTime(reader.GetOrdinal("ShowDate"))
                            };

                            showMap[showId].theatreShowDates.Add(showDate); // Voegt de datum toe aan de show.
                        }
                    }

                    shows = new List<TheatreShow>(showMap.Values); // Converteert de dictionary naar een lijst.
                    return shows;
                }
            }


        }

        // Methode om een show op te halen op basis van het ID.
        public async Task<TheatreShow> ShowWithId(int id)
        {
            var show = await _context.TheatreShow
                                .Include(s => s.Venue) // Inclusief gerelateerde locatie.
                                .Include(s => s.theatreShowDates) // Inclusief gerelateerde showdatums.
                                .FirstOrDefaultAsync(s => s.TheatreShowId == id);
            return show;
        }

        // Methode om shows te filteren op basis van titel of beschrijving.
        public async Task<List<TheatreShow>> ShowFilterTitle(string filter)
        {
            var query = _context.TheatreShow.AsQueryable(); // Maakt een query object.

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower(); // Zet filter om naar kleine letters.

                query = query.Where(ts =>
                    (ts.Title != null && ts.Title.ToLower().Contains(filter)) ||
                    (ts.Description != null && ts.Description.ToLower().Contains(filter))); // Vergelijkt strings in kleine letters.
            }

            return await query.ToListAsync(); // Voert de query uit en retourneert de resultaten.
        }

        // Methode om shows te filteren op basis van locatie (venue).
        public async Task<List<TheatreShow>> ShowFilterLocationAsync(int location)
        {
            var shows = await _context.TheatreShow
                                      .Include(ts => ts.Venue) // Inclusief gerelateerde locatie.
                                      .Where(ts => ts.Venue.VenueId == location) // Filtert op locatie ID.
                                      .ToListAsync();

            return shows;
        }

        // Methode om shows te filteren op basis van een datumbereik.
        public async Task<List<dynamic>> FilterShowsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Haalt alle locaties en hun shows op.
            var venues = await _context.Venue
                .Include(v => v.TheatreShows)
                    .ThenInclude(ts => ts.theatreShowDates) // Inclusief showdatums.
                .ToListAsync();

            // Filtert shows op basis van het datumbereik.
            var filteredShows = venues
                .SelectMany(v => v.TheatreShows, (v, ts) => new { Venue = v, TheatreShow = ts })
                .SelectMany(ts => ts.TheatreShow.theatreShowDates
                    .Where(tsd => tsd.DateAndTime >= startDate && tsd.DateAndTime <= endDate) // Filtert op datum.
                    .Select(tsd => (dynamic)new // Cast naar dynamic type.
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

        // Methode om een nieuwe show aan te maken.
        public async Task<string> CreateShowAsync(TheatreShow newShow)
        {
            // Controleert of de locatie al bestaat om duplicaten te vermijden.
            var existingVenue = await _context.Venue.FindAsync(newShow.Venue.VenueId);

            if (existingVenue == null)
            {
                return "Venue not found.";
            }

            newShow.Venue = existingVenue; // Wijst de bestaande locatie toe aan de show.

            _context.TheatreShow.Add(newShow);
            await _context.SaveChangesAsync();
            return "Show created successfully.";
        }

        // Methode om een bestaande show te updaten.
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

            // Update de titel, beschrijving en prijs.
            oldShow.Title = updatedShow.Title;
            oldShow.Description = updatedShow.Description;
            oldShow.Price = updatedShow.Price;

            // Update de venue.
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

            // Update showdatums.
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

        // Methode om een show te verwijderen.
        public async Task<string> DeleteShowAsync(int id)
        {
            // Zoekt naar de show op basis van ID.
            var show = await _context.TheatreShow.Include(s => s.theatreShowDates)
                                                 .FirstOrDefaultAsync(s => s.TheatreShowId == id);

            if (show == null)
            {
                return "Show not found.";
            }

            // Verwijdert de bijbehorende showdatums en de show zelf.
            _context.TheatreShowDate.RemoveRange(show.theatreShowDates);
            _context.TheatreShow.Remove(show);

            await _context.SaveChangesAsync();
            return "Show deleted successfully.";
        }

    }


}