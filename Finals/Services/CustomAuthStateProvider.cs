using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Finals.Models; // <-- Necessary to recognize the Finals.Models.User class

namespace Finals.Services
{
    // This bridges your AuthService with Blazor's built-in Authorization system.
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        // Inject your custom AuthService to read the current user state
        public CustomAuthStateProvider(AuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Assuming your AuthService holds the currently logged-in user object
            var user = _authService.CurrentUser; 
            var identity = new ClaimsIdentity();

            if (user != null)
            {
                // Determine the role based on your user object's property (e.g., IsAdmin)
                string role = user.IsAdmin ? "Admin" : "Customer";
                
                identity = new ClaimsIdentity(new[]
                {
                    // FIXED: Using 'user.Name' which is the correct property in the User model.
                    new Claim(ClaimTypes.Name, user.Name),
                    // CRITICAL: This Role Claim enables the [Authorize(Roles="...")] attribute.
                    new Claim(ClaimTypes.Role, role), 
                }, "CustomAuth");
            }

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
        
        // This is necessary for the AuthService to trigger UI updates on login/logout
        public void NotifyAuthenticationStateChanged()
        {
            var authState = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(authState);
        }
    }
}