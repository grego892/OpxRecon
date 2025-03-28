using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using OpxRecon.Data;
using OpxRecon.Models;
using System.Collections.Generic;

namespace OpxRecon.Components.Pages
{
    public partial class ReconLogs
    {
        [Inject]
        private ApplicationDbContext DbContext { get; set; }


        protected override async Task OnInitializedAsync()
        {

        }
    }
}
