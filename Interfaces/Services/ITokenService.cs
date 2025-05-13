using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlashApp.Models;

namespace FlashApp.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateToken(AppUser appUser);
    }
}
