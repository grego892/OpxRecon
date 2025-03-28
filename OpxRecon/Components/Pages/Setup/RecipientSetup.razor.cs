using Microsoft.AspNetCore.Components;
using MudBlazor;
using OpxRecon.Models;
using OpxRecon.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace OpxRecon.Components.Pages.Setup
{
    public partial class RecipientSetup
    {
        [Inject]
        private ApplicationDbContext DbContext { get; set; }

        private List<Station> stations = new List<Station>();
        private List<Recipient> recipients = new List<Recipient>();
        private Station? selectedStation;
        private Recipient? selectedRecipient;
        private Station newStation = new();
        private Recipient newRecipient = new();
        private TimeSpan? _time;

        protected override async Task OnInitializedAsync()
        {
            stations = await DbContext.Stations.ToListAsync();
            recipients = await DbContext.Recipients.ToListAsync();
        }

        private async Task AddStationAsync()
        {
            Log.Information("--- Setup - AddStation() -- Begin");
            try
            {
                // Check if there are any stations in the database
                int maxSortOrder = stations.Any() ? stations.Max(s => s.StationSortOrder) : 0;
                newStation.StationSortOrder = maxSortOrder + 1;

                // Add the new station to the database
                DbContext.Stations.Add(newStation);
                await DbContext.SaveChangesAsync();

                // Update the local list of stations
                stations.Add(newStation);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "++++++ Setup - AddStation() -- Error adding station");
            }
            newStation = new Station();

            StateHasChanged();
        }

        private async Task RemoveStationAsync(Station station)
        {
            Log.Information("--- Setup - RemoveStation() -- Removing Station");
            try
            {
                // Remove the station from the database
                DbContext.Stations.Remove(station);
                await DbContext.SaveChangesAsync();

                // Update the local list of stations
                stations.Remove(station);

                // Reorder the remaining stations to ensure StationSortOrder values are consecutive
                for (int i = 0; i < stations.Count; i++)
                {
                    stations[i].StationSortOrder = i + 1;
                }

                Log.Information("--- Setup - RemoveStation() -- Station Removed");
            }
            catch (Exception ex)
            {
                Log.Error("++++++ Setup - RemoveStation() -- " + ex.Message);
            }
        }

        public async Task AddRecipientAsync()
        {
            Log.Information("--- Setup - AddRecipient() -- Begin");
            try
            {
                // Add the new recipient to the database
                DbContext.Recipients.Add(newRecipient);
                await DbContext.SaveChangesAsync();
                // Update the local list of recipients
                recipients.Add(newRecipient);
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "++++++ Setup - AddRecipient() -- Error adding recipient");
            }
            newRecipient = new Recipient();
            StateHasChanged();
        }

        public async Task RemoveRecipientAsync(Recipient recipient)
        {
            Log.Information("--- Setup - RemoveRecipient() -- Removing Recipient");
            try
            {
                // Remove the recipient from the database
                DbContext.Recipients.Remove(recipient);
                await DbContext.SaveChangesAsync();
                // Update the local list of recipients
                recipients.Remove(recipient);
                Log.Information("--- Setup - RemoveRecipient() -- Recipient Removed");
            }
            catch (Exception ex)
            {
                Log.Error("++++++ Setup - RemoveRecipient() -- " + ex.Message);
            }
        }
    }
}
