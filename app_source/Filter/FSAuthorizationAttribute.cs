using Microsoft.AspNetCore.Authorization;

namespace App.API.Filter;

public class FSAuthorizeAttribute : AuthorizeAttribute
{
    public FSAuthorizeAttribute(string function, string action) : this($"{function}.{action}")
    {
    }
    public FSAuthorizeAttribute(string permission) : this()
    {
        Policy = permission;
    }
    public FSAuthorizeAttribute() : base()
    {
    }
}