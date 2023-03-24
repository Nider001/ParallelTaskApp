using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParallelTaskApp.App.BL.Interfaces;
using ParallelTaskApp.App.DAL.Interfaces;
using Microsoft.VisualBasic;
using ParallelTaskApp.App.BL;

namespace ParallelTaskApp.App.WinPL
{
    public static class LoginHandler
    {
        private static ILoginBL loginBL;

        static LoginHandler()
        {
            loginBL = new LoginBL();
        }

        public static bool HandleLogin()
        {
            bool wrongLogin = false;
            bool wrongPassword = false;
            string login = "";
            string password = "";

            while (true)
            {
                login = Interaction.InputBox(wrongLogin ? "Логин не найден, попробуйте еще раз:" : "Введите имя пользователя:", "Вход", "логин");

                if (login.Length == 0)
                    return false;

                if (loginBL.CheckLogin(login))
                    break;
                else
                    wrongLogin = true;
            }

            while (true)
            {
                password = Interaction.InputBox(wrongPassword ? "Пароль неверный, попробуйте еще раз:" : "Введите пароль пользователя:", "Вход", "пароль");

                if (password.Length == 0)
                    return false;

                if (loginBL.CheckPassword(login, password))
                    break;
                else
                    wrongPassword = true;
            }

            return true;
        }
    }
}
