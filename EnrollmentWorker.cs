using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TmsApi
{
    // Background worker that performs enrollment related work.
    public class EnrollmentWorker : BackgroundService
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentWorker(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Minimal loop - real logic goes here.
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}