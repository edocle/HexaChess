
using System;

namespace edocle.core
{
    /// <summary>
    /// Useful to schedule async processes to launch later
    /// Made to be paired with AsyncProcessReport
    /// </summary>
    public class AsyncProcessExecution
    {
        public AsyncProcessExecution(string name, Action<AsyncProcessTask> action, int priority = 0,
            bool mandatoryForProgression = false, bool killProcessIfFail = false, float progressWeight = 1)
        {
            Name = name;
            Action = action;
            Priority = priority;
            MandatoryForProgression = mandatoryForProgression;
            KillProcessIfFail = killProcessIfFail;
            ProgressWeight = progressWeight;
        }

        public string Name { get; private set; }

        public Action<AsyncProcessTask> Action { get; private set; }

        // Sets priority
        // higher value = higher priority
        public int Priority { get; private set; }

        // Won't execute other tasks of lesser priority until this task is not finished
        public bool MandatoryForProgression { get; private set; }

        // Will kill process if failure (obviously, only works if "MandatoryForProgression" is on
        public bool KillProcessIfFail { get; private set; }

        // Weight of this task in the whole process
        public float ProgressWeight { get; private set; }
    }
}
