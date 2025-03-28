using Microsoft.AspNetCore.Components;
using MudBlazor;
using OpxRecon.Models;
using OpxRecon.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace OpxRecon.Components.Pages.Setup
{
    public partial class EmailServerSetup
    {
        [Inject]
        private ApplicationDbContext DbContext { get; set; }

        private List<EmailServerSettings> emailServerSettingsList = new List<EmailServerSettings>();
        private EmailServerSettings newEmailServerSettings = new();

        protected override async Task OnInitializedAsync()
        {
            emailServerSettingsList = await DbContext.EmailServerSettings.ToListAsync();
        }

        private async Task AddEmailServerSettingsAsync()
        {
            Log.Information("--- EmailServerSetup - AddEmailServerSettings() -- Begin");
            try
            {
                // Add the new email server settings to the database
                DbContext.EmailServerSettings.Add(newEmailServerSettings);
                await DbContext.SaveChangesAsync();

                // Update the local list of email server settings
                emailServerSettingsList.Add(newEmailServerSettings);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "++++++ EmailServerSetup - AddEmailServerSettings() -- Error adding email server settings");
            }
            newEmailServerSettings = new EmailServerSettings();

            StateHasChanged();
        }

        private async Task RemoveEmailServerSettingsAsync(EmailServerSettings emailServerSettings)
        {
            Log.Information("--- EmailServerSetup - RemoveEmailServerSettings() -- Removing Email Server Settings");
            try
            {
                // Remove the email server settings from the database
                DbContext.EmailServerSettings.Remove(emailServerSettings);
                await DbContext.SaveChangesAsync();

                // Update the local list of email server settings
                emailServerSettingsList.Remove(emailServerSettings);

                Log.Information("--- EmailServerSetup - RemoveEmailServerSettings() -- Email Server Settings Removed");
            }
            catch (Exception ex)
            {
                Log.Error("++++++ EmailServerSetup - RemoveEmailServerSettings() -- " + ex.Message);
            }
        }
    }
}
