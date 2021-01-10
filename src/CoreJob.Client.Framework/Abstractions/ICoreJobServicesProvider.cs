using System.Threading.Tasks;

namespace  CoreJob.Client.Framework.Abstractions
{
    public interface ICoreJobServicesProvider<TInterface>
    {
        TInterface GetInstance(string key);

        Task<TInterface> GetInstanceAsync(string key);
    }
}
