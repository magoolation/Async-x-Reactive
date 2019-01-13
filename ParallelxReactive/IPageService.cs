using Refit;
using System;
using System.Threading.Tasks;

namespace ParallelxReactive
{
    interface IPageService
    {
        [Get("")]
        Task<string> GetAsync();
        [Get("")]
        IObservable<string> Get();
    }
}
