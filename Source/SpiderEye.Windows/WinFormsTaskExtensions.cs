using System.Threading.Tasks;

namespace SpiderEye.Windows
{
    /// <summary>
    /// Extension methods to run a <see cref="Task"/> synchronously on the UI thread.
    /// I know, it's very ugly but other solutions aren't any better.
    /// </summary>
    internal static class WinFormsTaskExtensions
    {
        public static void RunSyncWithPump(this Task task)
        {
            while (!task.IsCompleted) { System.Windows.Forms.Application.DoEvents(); }

            task.GetAwaiter().GetResult();
        }

        public static T RunSyncWithPump<T>(this Task<T> task)
        {
            while (!task.IsCompleted) { System.Windows.Forms.Application.DoEvents(); }

            return task.GetAwaiter().GetResult();
        }
    }
}
