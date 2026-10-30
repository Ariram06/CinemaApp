using System.IO;

namespace CinemaApp.Services
{
    public class AuditLogService
    {
        private readonly string _logFilePath;

        public AuditLogService()
        {
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo? dir = new DirectoryInfo(projectRoot);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Program.cs")))
            {
                dir = dir.Parent;
            }

            string baseDir = dir != null ? dir.FullName : Directory.GetCurrentDirectory();
            string logsDir = Path.Combine(baseDir, "Logs");

            try
            {
                if (!Directory.Exists(logsDir))
                {
                    Directory.CreateDirectory(logsDir);
                }
            }
            catch
            {
                logsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                if (!Directory.Exists(logsDir))
                {
                    Directory.CreateDirectory(logsDir);
                }
            }

            _logFilePath = Path.Combine(logsDir, "audit-log.txt");
        }

        public void Log(string module, string action, string details)
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"[{timestamp}] {module} | {action} | {details}{Environment.NewLine}";
                File.AppendAllText(_logFilePath, logEntry);
            }
            catch
            {
                // Silently ignore file errors to avoid breaking successful operations
            }
        }
    }
}
