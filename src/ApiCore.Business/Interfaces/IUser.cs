using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ApiCore.Business.Intefaces
{
    public interface IUser
    {
        string Name { get; }

        IEnumerable<Claim> GetClaimsIdentity();

        string GetUserEmail();

        Guid GetUserId();

        bool IsAuthenticated();

        bool IsInRole(string role);
    }
}