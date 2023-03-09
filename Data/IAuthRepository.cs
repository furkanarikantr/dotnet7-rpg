using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotnet_rpg.Data
{
    public interface IAuthRepository
    {
        //Kayıt olma
        Task<ServiceResponse<int>> Register(User user, string password);
        //Giriş Yapma
        Task<ServiceResponse<string>> Login(string username, string password);
        //Kontrol
        Task<bool> UserExists(string username);
    }
}