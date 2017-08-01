using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace AjeGroupCore.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public string SecretQuestion { get; set; }

        public string SecretResponse { get; set; }


        //public bool IsGoogleAuthenticatorEnabled { get; set; }

        //public string GoogleAuthenticatorSecretKey { get; set; }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        //{
        //    var authenticationType = "Basic";
        //    var userIdentity = new ClaimsIdentity(await manager.GetClaimsAsync(this), authenticationType);

        //    return userIdentity;
        //}
    }
}
