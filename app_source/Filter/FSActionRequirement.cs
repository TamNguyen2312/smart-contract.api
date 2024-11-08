using Microsoft.AspNetCore.Authorization;

namespace App.API.Filter;

public class FSActionRequirement : IAuthorizationRequirement
{
    public string FunctionCode { get; private set; }
    public string ActionCode { get; private set; }

    public FSActionRequirement(string fnc, string act)
    {
        FunctionCode = fnc;
        ActionCode = act;
    }
}
  
public class FSEmailConfirmRequirement : IAuthorizationRequirement
{
}
  
public class FSRoleRequirement : IAuthorizationRequirement
{
    public string Roles { get; private set; }
    public FSRoleRequirement(string roles)
    {
        Roles = roles;
    }
}