using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CGD.API.Tests
{
    public static class ControllerTestHelpers
    {
        public static T CreateWithUser<T>(Guid userId, object service) where T : ControllerBase
        {
            var controller = (T)Activator.CreateInstance(typeof(T), service);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuthentication"))
                }
            };
            return controller;
        }

        public static void AddModelError(ControllerBase controller)
        {
            controller.ModelState.AddModelError("Test", "Invalid");
        }
    }
}