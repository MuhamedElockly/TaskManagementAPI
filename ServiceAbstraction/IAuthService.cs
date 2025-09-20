using SharedData.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction
{
	public interface IAuthService
	{
		public Task<LoginResultDTO> LoginAsync(LoginDTO loginDTO);
		public Task<RegisterResultDTO> Register(RegisterDTO registerDTO);
	}
}
