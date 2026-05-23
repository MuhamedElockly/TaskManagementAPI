using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs;

/// <summary>JWT tokens returned after successful login or refresh.</summary>
public class LoginResultDTO
{
    /// <summary>JWT access token for the Authorize header.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Refresh token used with POST /api/Auth/RefreshToken.</summary>
    public string RefreshToken { get; set; } = string.Empty;
}