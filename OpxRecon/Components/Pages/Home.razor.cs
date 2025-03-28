using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OpxRecon.Data;
using OpxRecon.Models;
using System.Collections.Generic;

namespace OpxRecon.Components.Pages
{
    public partial class Home
    {
        [Inject]
        private ApplicationDbContext DbContext { get; set; }
        private List<Station> stations = new List<Station>();
        private List<Recipient> recipients = new List<Recipient>();

        protected override async Task OnInitializedAsync()
        {
            stations = await DbContext.Stations.ToListAsync();
            recipients = await DbContext.Recipients.ToListAsync();
        }
    }
}
