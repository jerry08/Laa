using System.Threading.Tasks;

namespace LaaSender
{
    public interface IBluetoothRequestService
    {
        bool IsEnabled();
        
        Task<bool> Enable();
    }
}