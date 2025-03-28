using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Reflection;
using Serilog;

namespace OpxRecon.Components.Layout
{
    public partial class MainLayout
    {
        bool _drawerOpen = true;
        private MudThemeProvider? _mudThemeProvider;
        private bool _isDarkMode = true;
        [Inject] private IDialogService? DialogService { get; set; }

        void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
            StateHasChanged();
            Log.Information("--- MainLayout -- DrawerToggle() - _drawerOpen: {_drawerOpen}", _drawerOpen);
        }


        MudTheme OpxReconTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = Colors.LightBlue.Default,
                Secondary = Colors.Teal.Default,
                AppbarBackground = Colors.LightBlue.Darken4
            },
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.LightBlue.Default,
                Secondary = Colors.Teal.Default,
                AppbarBackground = Colors.LightBlue.Darken4
            },

            LayoutProperties = new LayoutProperties()
            {
                DrawerWidthLeft = "180px",
            }
        };

        private async Task ShowAboutSnackbar()
        {
            bool? result = await DialogService.ShowMessageBox(
                "OpxRecon",
                (MarkupString)$"Version: {GetVersion()}");
            string? state = result == null ? "Canceled" : "Deleted!";
            StateHasChanged();
        }

        string GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version.ToString();
        }
    }
}
