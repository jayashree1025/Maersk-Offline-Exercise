using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;
        private ConcurrentQueue<SortJob> _jobs = new ConcurrentQueue<SortJob>();
        public ConcurrentQueue<SortJob> _completedJobs = new ConcurrentQueue<SortJob>();

        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
        }

        public void EnqueueJob(SortJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            _jobs.Enqueue(job);
        }

        public List<SortJob> GetAllJobs()
        {
            var jobs = new List<SortJob>();
            if (_jobs != null && _jobs.Count > 0)
            {
                foreach (SortJob obj in _jobs)
                    jobs.Add(obj);
            }
            if (_completedJobs != null && _completedJobs.Count > 0)
            {
                foreach (SortJob obj in _completedJobs)
                    jobs.Add(obj);
            }
            return jobs;
        }

        public SortJob GetJob(string jobId)
        {
            var job = GetAllJobs().SingleOrDefault(p => p.Id.ToString() == jobId);
            return job;
        }

        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(10000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            return new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);
        }

        public async Task ProcessAsync()
        {
            _jobs.TryPeek(out var job);
            try
            {
                await Task.Delay(1000);
                if (job != null)
                {
                    _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

                    var stopwatch = Stopwatch.StartNew();

                    var output = job.Input.OrderBy(n => n).ToArray();
                    await Task.Delay(10000); // NOTE: This is just to simulate a more expensive operation

                    var duration = stopwatch.Elapsed;

                    _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);
                    var completedJob = new SortJob(
                        id: job.Id,
                        status: SortJobStatus.Completed,
                        duration: duration,
                        input: job.Input,
                        output: output);
                    _completedJobs.Enqueue(completedJob);
                    _jobs.TryDequeue(out var dequedJob);
                }
                else
                {
                    _logger.LogInformation("No job found in the queue.");

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error occurred executing {Job}.", nameof(job));
            }
        }
    }
}
