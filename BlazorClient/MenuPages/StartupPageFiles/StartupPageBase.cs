﻿using Microsoft.AspNetCore.Components;

namespace BlazorClient.MenuPages.StartupPageFiles
{
    public class StartupPageBase : ComponentBase
    {
        [Inject]
        public NavigationManager NavManager { get; set; }

        public void NavigateToLoginPage()
        {
            NavManager.NavigateTo("/login");
        }

        public void NavigateToRegisterPage()
        {
            NavManager.NavigateTo("/register");
        }
    }
}
