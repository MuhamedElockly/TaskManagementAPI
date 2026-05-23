using SharedData.DTOs;
using SharedData.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
    public interface IAuthService
    {
        public Task<ApiResponse<LoginResultDTO>> LoginAsync(LoginDTO loginDTO);
        public Task<ApiResponse<RegisterResultDTO>> Register(RegisterDTO registerDTO);
        public  Task<ApiResponse<RegisterResultDTO>> RefreshAccessToken(string token);
    }
}
