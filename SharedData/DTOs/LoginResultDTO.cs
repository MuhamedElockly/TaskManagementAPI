using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs
{
	public class LoginResultDTO
	{
		public string Token { get; set; }
		public string IsBlocked { get; set; }
	}
}
