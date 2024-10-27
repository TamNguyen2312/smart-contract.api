using System;
using System.Security.Claims;
using FS.Commons;
using Microsoft.AspNetCore.Mvc;

namespace FS.BaseAPI;

public class BaseAPIController : ControllerBase
{
    /// <summary>
    /// Errors the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="data">The extend data.</param>
    /// <returns></returns>
    protected ActionResult Error(string message, object data = null)
    {
        return new BadRequestObjectResult(new FSResponse
        {
            Data = data,
            StatusCode = System.Net.HttpStatusCode.BadRequest,
            Message = message
        });
    }

    protected ActionResult GetNotFound(string message, object data = null)
    {
        return new NotFoundObjectResult(new FSResponse
        {
            Data = data,
            Message = message,
            StatusCode = System.Net.HttpStatusCode.NotFound
        });
    }

    protected ActionResult GetUnAuthorized(string message, object data = null)
    {
        return new UnauthorizedObjectResult(new FSResponse
        {
            Data = data,
            Message = message,
            StatusCode = System.Net.HttpStatusCode.Unauthorized
        });
    }

    /// <summary>
    /// Gets the data failed.
    /// </summary>
    /// <returns></returns>
    protected ActionResult GetError()
    {
        return Error(Constants.GetDataFailed);
    }

    /// <summary>
    /// Gets the data failed.
    /// </summary>
    /// <returns></returns>
    protected ActionResult GetError(string message)
    {
        return Error(message);
    }

    /// <summary>
    /// Saves the data failed.
    /// </summary>
    /// <returns></returns>
    protected ActionResult SaveError(object data = null)
    {
        return Error(Constants.SaveDataFailed, data);
    }

    /// <summary>
    /// Models the invalid.
    /// </summary>
    /// <returns></returns>
    protected ActionResult ModelInvalid()
    {
        var errors = ModelState.Where(m => m.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key.ToCamelCase(),
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).First()).ToList();
        return new BadRequestObjectResult(new FSResponse
        {
            Errors = errors,
            StatusCode = System.Net.HttpStatusCode.BadRequest,
            Message = Constants.SaveDataFailed
        });
    }

    /// <summary>
    /// Successes request.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="message">The message.</param>
    /// <returns></returns>
    protected ActionResult Success(object data, string message)
    {
        return new OkObjectResult(new FSResponse
        {
            Data = data,
            StatusCode = System.Net.HttpStatusCode.OK,
            Message = message,
            Success = true
        });
    }

    /// <summary>
    /// Gets the data successfully.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    protected ActionResult GetSuccess(object data)
    {
        return Success(data, Constants.GetDataSuccess);
    }

    /// <summary>
    /// Saves the data successfully
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    protected ActionResult SaveSuccess(object data)
    {
        return Success(data, Constants.SaveDataSuccess);
    }

    /// <summary>
    /// Get the loged in UserName;
    /// </summary>
    protected string UserName => User.FindFirst(ClaimTypes.Name)?.Value;

    /// <summary>
    /// Get the logged in user email.
    /// </summary>
    protected string UserEmail => User.FindFirst(Constants.CLAIM_EMAIL)?.Value;

    /// <summary>
    /// Get the loged in UserId;
    /// </summary>
    protected long UserId
    {
        get
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            long.TryParse(id, out long userId);
            return userId;
        }
    }

    /// <summary>
    /// The boolean value that determined whether current user is a Seller
    /// </summary>
    protected bool IsSeller
    {
        get
        {
            var isseller = User.FindFirst(Constants.IS_SELLER)?.Value;
            bool.TryParse(isseller, out bool isSeller);
            return isSeller;
        }
    }

    protected bool IsAdmin
    {
        get
        {
            var isadmin = User.FindFirst(Constants.IS_ADMIN)?.Value;
            bool.TryParse(isadmin, out bool isAdmin);
            return isAdmin;
        }
    }

    protected bool IsManager
    {
        get
        {
            var isManager = User.FindFirst(Constants.IS_MANAGER)?.Value;
            bool.TryParse(isManager, out bool isManagerReal);
            return isManagerReal;
        }
    }

    protected bool IsEmployee
    {
        get
        {
            var isEmployee = User.FindFirst(Constants.IS_EMPLOYEE)?.Value;
            bool.TryParse(isEmployee, out bool is_employee);
            return is_employee;
        }
    }

    protected bool IsRemember
    {
        get
        {
            var isRemeber = User.FindFirst(Constants.IS_REMEMBER)?.Value;
            bool.TryParse(isRemeber, out bool is_remember);
            return is_remember;
        }
    }

    protected bool IsCreator
    {
        get
        {
            var iscreator = User.FindFirst(Constants.IS_CREATOR)?.Value;
            bool.TryParse(iscreator, out bool isCreator);
            return isCreator;
        }
    }
}
