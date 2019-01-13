using System;
using Refit;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Text;

namespace ParallelxReactive
{
    class HttpTests
    {
        private Stopwatch stopWatch = new Stopwatch();
        private IPageService pageService = RestService.For<IPageService>(@"http://www.microsoft.com");
        private StringBuilder builder = new StringBuilder();

        public async Task Run()
        {
            await RunSequential();
            await RunParallel();
            await RunAsyncObservable();
            RunObservable();

            Console.WriteLine(builder.ToString());
        }

        private async Task RunAsyncObservable()
        {
            stopWatch.Reset();
            stopWatch.Start();
            await Observable
            .Range(1, 10, ThreadPoolScheduler.Instance)
            .Select(OnEach)
            .Switch()
            .LastOrDefaultAsync();
            stopWatch.Stop();
            builder.AppendLine($"Async Observable {stopWatch.ElapsedMilliseconds}");
        }

        private void RunObservable()
        {
            stopWatch.Reset();
            stopWatch.Start();
            Observable
            .Range(1, 10, ThreadPoolScheduler.Instance)
            .Select(OnEach)
            .Switch()
            .Finally(OnFinish);
        }

        private IObservable<string> OnEach(int i)
        {
            return pageService.Get();
        }        

        private void OnFinish()
        {
            stopWatch.Stop();
            builder.AppendLine($"Observable {stopWatch.ElapsedMilliseconds}");
        }

        private async Task RunParallel()
        {
            stopWatch.Reset();
            stopWatch.Start();
            Task<string>[] tasks = new Task<string>[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] =  pageService.GetAsync();
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
            stopWatch.Stop();
            builder.AppendLine($"Parallel {stopWatch.ElapsedMilliseconds}");
        }

        private async Task RunSequential()
        {
            stopWatch.Reset();
            stopWatch.Start();
            for(int i = 0; i < 10; i++)
            {
                await pageService.GetAsync().ConfigureAwait(false);
            }
            stopWatch.Stop();
            builder.AppendLine($"Sequential {stopWatch.ElapsedMilliseconds}");
        }
    }
}
