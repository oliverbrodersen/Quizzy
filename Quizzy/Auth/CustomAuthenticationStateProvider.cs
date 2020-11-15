using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Quizzy.Data;
using Quizzy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Quizzy.Auth
{
        public class CustomAuthenticationStateProvider : AuthenticationStateProvider
        {
            private readonly IJSRuntime jsRuntime;
            private readonly IUserService userService;
            public UserInfo cachedUser;

            public CustomAuthenticationStateProvider(IJSRuntime jsRuntime, IUserService userService)
            {
                this.jsRuntime = jsRuntime;
                this.userService = userService;
            }

            public override async Task<AuthenticationState> GetAuthenticationStateAsync()
            {
                var identity = new ClaimsIdentity();
                if (cachedUser == null)
                {
                    string userAsJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "currentUser");
                    if (!string.IsNullOrEmpty(userAsJson))
                    {
                        cachedUser = JsonSerializer.Deserialize<UserInfo>(userAsJson);

                        identity = SetupClaimsForUser(cachedUser);
                    }
                }
                else
                {
                    identity = SetupClaimsForUser(cachedUser);
                }

                ClaimsPrincipal cachedClaimsPrincipal = new ClaimsPrincipal(identity);
                return await Task.FromResult(new AuthenticationState(cachedClaimsPrincipal));
            }

            public async Task ValidateLoginAsync(string username, string password)
            {
                if (string.IsNullOrEmpty(username)) throw new Exception("Enter username");
                if (string.IsNullOrEmpty(password)) throw new Exception("Enter password");

                ClaimsIdentity identity = new ClaimsIdentity();
                try
                {
                    UserInfo user = await userService.ValidateUserAsync(username, password);
                    identity = SetupClaimsForUser(user);
                    string serialisedData = JsonSerializer.Serialize(user);
                    await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", serialisedData);
                    cachedUser = user;
                }
                catch (Exception)
                {
                    throw;
                }

                NotifyAuthenticationStateChanged(
                    Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
            }
            public async Task RegisterAsync(string username, string password)
            {
                await userService.RegisterAsync(username, password);
                await ValidateLoginAsync(username, password);
            }

            public void Logout()
            {
                cachedUser = null;
                var user = new ClaimsPrincipal(new ClaimsIdentity());
                jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "currentUser", "");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            }

            private ClaimsIdentity SetupClaimsForUser(UserInfo user)
            {
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, user.Id));
                claims.Add(new Claim("Level", user.SecurityLevel.ToString()));

                ClaimsIdentity identity = new ClaimsIdentity(claims, "apiauth_type");
                return identity;
            }
            public UserInfo GetUser()
        {
            return cachedUser;
        }
        }
}
