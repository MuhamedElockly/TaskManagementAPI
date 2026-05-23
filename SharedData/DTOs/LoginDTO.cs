using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs;

/// <summary>Credentials for user login.</summary>
public class LoginDTO
{
    /// <summary>Registered email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Account password.</summary>
    public string Password { get; set; } = string.Empty;
}
