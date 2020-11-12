using System.Threading.Tasks;

namespace CQRSlite.Commands
{
    public interface ICommandSender
    {
        Task SendAsync<T>(T command) where T : ICommand;
    }
}