using System;
using FS.Commons.Models.DTOs;

namespace FS.BLL.Services.Interfaces;

public interface IEmailService
{
    public Task<bool> SendEmailAsync(EmailDTO emailDTO);
}