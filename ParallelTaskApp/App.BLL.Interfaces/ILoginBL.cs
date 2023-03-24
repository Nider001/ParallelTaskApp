namespace ParallelTaskApp.App.BL.Interfaces
{
    public interface ILoginBL
    {
        bool CheckLogin(string login);
        bool CheckPassword(string login, string password);
    }
}