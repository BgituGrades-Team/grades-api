using System.Diagnostics;

namespace BgituGrades.Services
{
    public interface IScheduleLoaderService
    {
        Task<bool> RunAsync(string apiKey, CancellationToken cancellationToken);
    }
    public class ScheduleLoaderService(IConfiguration config, ILogger<IScheduleLoaderService> logger) : IScheduleLoaderService
    {
        private readonly IConfiguration _config = config;
        private readonly ILogger<IScheduleLoaderService> _logger = logger;

        public async Task<bool> RunAsync(string apiKey, CancellationToken cancellationToken)
        {
            var loaderPath = _config["ScheduleLoader:ExecutablePath"];
            if (string.IsNullOrEmpty(loaderPath) || !File.Exists(loaderPath))
            {
                return false;
            }

            var psi = new ProcessStartInfo
            {
                FileName = loaderPath,
                Arguments = "--headless",
                WorkingDirectory = Path.GetDirectoryName(loaderPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Environment = {
                    ["GRADES_API_KEY"] = apiKey
                }
            };

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    _logger.LogInformation("[Loader] {Line}", e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                    _logger.LogError("[Loader] {Line}", e.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();


            await process.WaitForExitAsync(cancellationToken);

            return process.ExitCode == 0;
        }
    }
}
