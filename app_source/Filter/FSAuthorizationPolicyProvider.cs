using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace App.API.Filter;

public class FSAuthorizationPolicyProvider : IAuthorizationPolicyProvider
	{
		public const string EMAIL_CONFIRM = "EmailConfirm";
		public const string IN_ROLES = "InRole";
		public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
		public FSAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
		{
			// ASP.NET Core only uses one authorization policy provider, so if the custom implementation
			// doesn't handle all policies (including default policies, etc.) it should fall back to an
			// alternate provider.
			//
			// In this sample, a default authorization policy provider (constructed with options from the 
			// dependency injection container) is used if this custom provider isn't able to handle a given
			// policy name.
			//
			// If a custom policy provider is able to handle all expected policy names then, of course, this
			// fallback pattern is unnecessary.
			FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
		}
		public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
		public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

		// Policies are looked up by string name, so expect 'parameters' (like age)
		// to be embedded in the policy names. This is abstracted away from developers
		// by the more strongly-typed attributes derived from AuthorizeAttribute
		// (like [MinimumAgeAuthorize] in this sample)
		public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
		{
			if (policyName.Contains("."))
			{
				string function = policyName.Split('.')[0];
				string action = policyName.Split('.')[1];
				var policy = new AuthorizationPolicyBuilder();
				policy.AddRequirements(new FSActionRequirement(function, action));
				return Task.FromResult(policy.Build());
			}
			else if (policyName == EMAIL_CONFIRM)
			{
				var policy = new AuthorizationPolicyBuilder();
				policy.AddRequirements(new FSEmailConfirmRequirement());
				return Task.FromResult(policy.Build());
			}

			// If the policy name doesn't match the format expected by this policy provider,
			// try the fallback provider. If no fallback provider is used, this would return 
			// Task.FromResult<AuthorizationPolicy>(null) instead.
			return FallbackPolicyProvider.GetPolicyAsync(policyName);
		}
	}