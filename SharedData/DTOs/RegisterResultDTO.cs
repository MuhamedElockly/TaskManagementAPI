using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs
{
	public class RegisterResultDTO
	{
		public bool IsSuccess { get; set; }
		public string Message { get; set; }
		public string? Token { get; set; }
		public int? UserId { get; set; }
	}
}
