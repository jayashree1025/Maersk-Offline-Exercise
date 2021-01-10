using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);
        Task ProcessAsync();
        void EnqueueJob(SortJob job);
        List<SortJob> GetAllJobs();
        SortJob GetJob(string jobId);
    }
}