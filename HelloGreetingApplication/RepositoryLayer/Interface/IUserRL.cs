﻿using System;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
	public interface IUserRL
	{
        User RegisterUser(RegisterDTO registerDTO);
        UserResponseDTO LoginUser(LoginDTO loginDTO);
        bool ResetPassword(string token, string newPassword);
        bool ForgetPassword(string email);
    }
}

