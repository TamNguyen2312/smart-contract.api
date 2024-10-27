using System;
using System.IdentityModel.Tokens.Jwt;
using FS.BaseModels;
using FS.BaseModels.IdentityModels;
using FS.BLL.Services.Interfaces;
using FS.Commons;
using FS.Commons.Models;
using FS.Commons.Models.DTOs;
using FS.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace FS.BLL.Services.Implementations;

public class IdentityBizLogic : IIdentityBizLogic
{
	IIdentityRepository _identityRepository;
	public IdentityBizLogic(IIdentityRepository identityRepository)
	{
		_identityRepository = identityRepository;
	}
	public async Task<long> AddUserAsync(ApplicationUser dto, string password)
	{
		return await _identityRepository.AddUserAsync(dto, password);
	}
	public async Task<bool> UpdateAsync(ApplicationUser dto)
	{
		return await _identityRepository.UpdateAsync(dto);
	}
	public async Task<JwtSecurityTokenDTO> GenerateJwtToken(ApplicationUser user, bool isRemember, bool isAdmin, bool isManager = false, bool isEmployee = false)
	{
		return await _identityRepository.GenerateJwtToken(user, isRemember, isAdmin, isManager, isEmployee);
	}

	public async Task<string> GenerateRefreshToken(ApplicationUser user, JwtSecurityToken jwtToken, bool isRemember)
	{
		return await _identityRepository.GenerateRefreshToken(user, jwtToken, isRemember);
	}

	/// <summary>
	/// This is used to authen and check validation of access and refresh token
	/// </summary>
	/// <param name="baseTokenModel"></param>
	/// <returns></returns>
	public async Task<BaseResponse<RefreshToken>> ValidateAndVerifyToken(BaseTokenModel baseTokenModel)
	{
		return await _identityRepository.ValidateAndVerifyToken(baseTokenModel);
	}

	/// <summary>
	/// This is used to update an refresh token
	/// </summary>
	/// <param name="token"></param>
	/// <returns></returns>
	public async Task<bool> UpdateTokenAsync(RefreshToken token)
	{
		return await _identityRepository.UpdateTokenAsync(token);
	}

	/// <summary>
	/// This is used to log out an account
	/// </summary>
	/// <param name="dto"></param>
	/// <param name="user"></param>
	/// <returns></returns>
	public async Task<FSResponse> LogOutAsync(LogOutDTO dto, ApplicationUser user)
	{
		return await _identityRepository.LogOutAsycn(dto, user);
	}
	public async Task<ApplicationUser> GetByEmailAsync(string email)
	{
		return await _identityRepository.GetByEmailAsync(email);
	}
	public async Task<ApplicationUser> GetByIdAsync(long id)
	{
		return await _identityRepository.GetByIdAsync(id);
	}
	public async Task<ApplicationUser> GetByExternalIdAsync(string id)
	{
		return await _identityRepository.GetByExternalIdAsync(id);
	}
	public async Task<bool> CheckPasswordAsync(ApplicationUser dto, string password)
	{
		return await _identityRepository.CheckPasswordAsync(dto, password);
	}
	public async Task<bool> HasPasswordAsync(ApplicationUser dto)
	{
		return await _identityRepository.HasPasswordAsync(dto);
	}
	public async Task<IdentityResult> AddPasswordAsync(ApplicationUser dto, string password)
	{
		return await _identityRepository.AddPasswordAsync(dto, password);
	}
	public async Task<ApplicationUser> GetByPhoneAsync(string phoneNumber)
	{
		return await _identityRepository.GetByPhoneAsync(phoneNumber);
	}
	public async Task<bool> ConfirmEmailAsync(string userId, string token)
	{
		return await _identityRepository.ConfirmEmailAsync(userId, token);
	}

	public async Task<bool> VerifyEmailAsync(ApplicationUser user, string token)
	{
		return await _identityRepository.VerifyEmailAsycn(user, token);
	}
	public async Task<bool> ChangePassword(string userId, string passwordNew)
	{
		return await _identityRepository.ChangePassword(userId, passwordNew);
	}
	public async Task<bool> AddRoleByNameAsync(string userId, string roleName)
	{
		return await _identityRepository.AddRoleByNameAsync(userId, roleName);
	}
	public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
	{
		return await _identityRepository.GenerateEmailConfirmationTokenAsync(user);
	}
	public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
	{
		return await _identityRepository.GeneratePasswordResetTokenAsync(user);
	}
	public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
	{
		return await _identityRepository.ResetPasswordAsync(userId, token, newPassword);
	}

	public async Task<bool> CreateUpdateRoleAsync(string roleName, bool isAdmin)
	{
		return await _identityRepository.CreateUpdateRoleAsync(roleName, isAdmin);
	}

	public async Task<bool> IsUserInRoles(long userId, string RoleName)
	{
		var user = await _identityRepository.GetByIdAsync(userId);
		if (user == null)
			return false;
		var roles = RoleName.Split(",");
		for (int i = 0; i < roles.Length; i++)
		{
			var role = roles[i];
			bool isInRole = await _identityRepository.IsUserInRole(user, role);
			if (isInRole)
				return true;
		}
		return false;
	}
	public async Task<bool> DeleteRoleByUser(long userId)
	{
		return await _identityRepository.DeleteRoleByUser(userId);
	}
	public async Task<bool> DeleteListRole(long[] ids)
	{
		return await _identityRepository.DeleteListRole(ids);
	}
	public async Task<string[]> GetRolesAsync(long id)
	{
		return await _identityRepository.GetRolesAsync(id);
	}
	public async Task<bool> VerifyPermission(long userId, string function, string action)
	{
		return await _identityRepository.VerifyPermission(userId, $"{function}.{action}");
	}
	public async Task<List<Role>> GetRolesAdmin()
	{
		return await _identityRepository.GetRolesAdmin();
	}
}
