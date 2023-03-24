namespace ParallelTaskApp.App.DAL.Interfaces
{
    public interface ILoginDA
    {
        bool CheckLogin(string login);
        bool CheckPassword(string login, string password);
    }
}