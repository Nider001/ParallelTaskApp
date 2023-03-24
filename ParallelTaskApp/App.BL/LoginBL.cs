using ParallelTaskApp.App.BL.Interfaces;
using ParallelTaskApp.App.DAL;
using ParallelTaskApp.App.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelTaskApp.App.BL
{
    public class LoginBL : ILoginBL
    {
        private ILoginDA loginDA;

        public LoginBL()
        {
            loginDA = new LoginDA();
        }

        public bool CheckLogin(string login)
        {
            return loginDA.CheckLogin(login);
        }

        public bool CheckPassword(string login, string password)
        {
            return loginDA.CheckPassword(login, password);
        }
    }
}
