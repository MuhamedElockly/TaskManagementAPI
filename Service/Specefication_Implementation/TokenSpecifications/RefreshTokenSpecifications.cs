using Domain.Entities.IdentityEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specefication_Implementation.TokenSpecifications
{
    public class RefreshTokenSpecifications : Specification<RefreshToken, int>
    {
        public RefreshTokenSpecifications(string refreshToken) : base(t => t.Token == refreshToken)
        {
            Includes.Add(t => t.User);
        }
    }
}