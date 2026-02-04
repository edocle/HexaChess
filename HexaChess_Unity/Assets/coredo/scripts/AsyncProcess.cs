
using System;
using System.Collections.Generic;

// @todos:
// - add timeout
// - add sub process handling
// -- adding sub process
// -- checking sub process
// - 

namespace edocle.core
{
    /// <summary>
    /// Generic way to keep track of the progress of a task
    /// </summary>
    public class AsyncProcessTask
    {
        // initial variables
        public string Name { get; private set; }
        float m_TimeoutValue = -1; // in seconds
        float m_StartTime; // in seconds, time since start of app

        public ProcessTaskState State { get; private set; }
        List<AsyncProcessTask> m_SubTasks;

        // calls
        public AsyncProcessTask(string name, Action<AsyncProcessTask> onCompletedSuccess, Action<AsyncProcessTask> onCompletedFail, float timeOutValue = 0)
        {
            Name = name;
            m_StartTime = UnityEngine.Time.time;

            OnCompletedSuccess = onCompletedSuccess;
            OnCompletedFail = onCompletedFail;

            if (timeOutValue != 0)
                m_TimeoutValue = timeOutValue;

            State = ProcessTaskState.inProgress;
        }

        // actions
        public Action<AsyncProcessTask> OnCompletedSuccess;
        public Action<AsyncProcessTask> OnCompletedFail;
        public Action<AsyncProcessTask> OnSubTaskCompleted;


        // implementation
        public void CompleteProcess(ProcessTaskState state = ProcessTaskState.done_success)
        {
            if (state == ProcessTaskState.inProgress)
                return; // has no sense

            if (State != ProcessTaskState.inProgress)
                return; // already completed

            State = state;

            switch(State)
            {
                case ProcessTaskState.done_success:
                    OnCompletedSuccess?.Invoke(this);
                    break;
                default:
                case ProcessTaskState.done_fail_timeout:
                case ProcessTaskState.done_fail_unknown:
                case ProcessTaskState.done_fail_subtaskFailed:
                    OnCompletedFail?.Invoke(this);
                    break;
            }
        }
    }

    public enum ProcessTaskState
    {
        inProgress,
        done_success,
        done_fail_timeout,
        done_fail_unknown,
        done_fail_subtaskFailed,
    }
}