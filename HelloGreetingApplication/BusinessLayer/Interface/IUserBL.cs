using System;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
	public interface IUserBL
	{
        User RegisterUser(RegisterDTO userRegisterDTO);
        User LoginUser(LoginDTO loginDTO);
    }
}

