using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;

namespace ParallelxReactive
{
    internal class FileSystemTests
    {
        private Stopwatch stopWatch = new Stopwatch();
        private Random random = new Random();
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

        private IObservable<int> OnEach(int i) => CreateFileAsync(i).ToObservable();
        
        private void RunObservable()
        {
            stopWatch.Reset();
            stopWatch.Start();
            Observable
                .Range(1, 10, ThreadPoolScheduler.Instance)
                .ForEach(CreateFile);
            stopWatch.Stop();
            builder.AppendLine($"Reactive {stopWatch.ElapsedMilliseconds}");
        }                        
        
        private async Task RunSequential()
        {
            stopWatch.Reset();
            stopWatch.Start();
            for (int i = 0; i < 10; i++)
            {
                await CreateFileAsync(i).ConfigureAwait(false);
            }
            stopWatch.Stop();
            builder.AppendLine($"Sequential {stopWatch.ElapsedMilliseconds}");
        }

        private async Task RunParallel()
        {
            var tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = CreateFileAsync(i);
            }
            stopWatch.Reset();
            stopWatch.Start();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            stopWatch.Stop();
            builder.AppendLine($"Parallel {stopWatch.ElapsedMilliseconds}");
        }

        private async Task<int> CreateFileAsync(int i)
        {
            var buffer = new byte[i * 1048576];
            using (var file = File.Create($"file-{i}.txt"))
            {
                random.NextBytes(buffer);
                await file.WriteAsync(buffer).ConfigureAwait(false);
            }
            return i;
        }

        private void CreateFile(int i)
        {
            var buffer = new byte[i * 1048576];
            using (var file = File.Create($"file-{i}.txt"))
            {
                random.NextBytes(buffer);
                file.Write(buffer);
            }
        }
    }
}