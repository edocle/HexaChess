
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace edocle.core
{
    /// <summary>
    /// Useful to launch multiple async tasks in the right order
    /// - Based on priority
    /// - Can wait end of task to launch less priorized tasks
    /// - Can kill whole process if tasks fail
    /// </summary>
    public class AsyncProcessReport
    {
        // All scheduled tasks
        List<AsyncProcessExecution> m_ScheduledProcesses = new List<AsyncProcessExecution>();
        // iteration in progress
        int m_IterationIndex = 0;
        // Task to call when everything is done
        AsyncProcessTask Callback = null;

        public Action<AsyncProcessReport> OnReportUpdated = null;
        public float Progression { get; private set; }


        float m_ProgressionDoneWeight = 0;
        float m_ProgressionTotalWeight = 0;
        bool m_Cancelled = false;


        /// <summary>
        /// Schedule task to execute later among other tasks
        /// Use async execution to later launch all tasks in specific order
        /// </summary>
        /// <param name="execution"></param>
        public void ScheduleTask(AsyncProcessExecution execution)
        {
            // Safeguard
            if (execution == null || execution.Action == null)
            {
                Debug.LogError($"Async report> Execution or action is null; should always exist; ignored...");
                return;
            }

            m_ScheduledProcesses.Add(execution);
        }

        /// <summary>
        /// Launch execution of all schedules tasks
        /// </summary>
        /// <param name="callback"></param>
        public void ExecuteTasks(string name, AsyncProcessTask callback)
        {
            Callback = callback;
            // Sort processes by priority
            m_ScheduledProcesses.Sort((a, b) => a.Priority >= b.Priority ? 1 : -1);
            // Get total progress weight to handle percentage
            m_ProgressionTotalWeight = m_ScheduledProcesses.Sum(f => f.ProgressWeight);

            ExecuteNextTasks();
        }

        int m_MandatoryTasksInProgress = 0;

        void ExecuteNextTasks()
        {
            if (m_Cancelled) // whole process has been cancelled
                return;

            // 1_ Check if already over
            if (m_ScheduledProcesses.Count == m_IterationIndex)
            {
                Callback.CompleteProcess();
                return;
            }

            // 2_ Launch next tasks
            // Launch all tasks with same priority
            int nextPriority = m_ScheduledProcesses[m_IterationIndex].Priority;
            List<AsyncProcessExecution> nextProcesses = m_ScheduledProcesses.Where(f => f.Priority == nextPriority).ToList();

            bool mandatoryTasks = false;
            foreach(var process in nextProcesses)
            {
                // add "wip" token to make sure not to go to next step
                if (process.MandatoryForProgression)
                {
                    mandatoryTasks = true;
                    m_MandatoryTasksInProgress++;
                }

                AsyncProcessTask task = new AsyncProcessTask($"AsyncReport[{Callback.Name}]>Task[{process.Name}]",
                (task) =>
                { // on complete success

                    ProcessNewDoneTask(process);
                },
                (task) =>
                { // on complete fail

                    // Stop everything, call fail
                    if (process.MandatoryForProgression && process.KillProcessIfFail)
                        KillWholeProcess(task);

                    ProcessNewDoneTask(process);
                }
                );

                // 2.1_ Trigger action
                process.Action.Invoke(task);
                Debug.Log($"[color=cyan]{task.Name}>[/color] Starting task (priority: {process.Priority} / mandatory: {process.MandatoryForProgression} / cancel if fail: {process.KillProcessIfFail})");
            }

            // 3_ Prepare next step
            m_IterationIndex += nextProcesses.Count;
            // in case there is no mandatory task
            if (!mandatoryTasks)
                TryTriggerNextTask();
        }

        void ProcessNewDoneTask(AsyncProcessExecution process)
        {
            // Progress
            m_ProgressionDoneWeight += process.ProgressWeight;
            Progression = m_ProgressionTotalWeight / m_ProgressionDoneWeight;

            OnReportUpdated?.Invoke(this);

            // When process ends, remove "wip" token
            if (process.MandatoryForProgression)
            {
                m_MandatoryTasksInProgress--;
                TryTriggerNextTask();
            }
        }

        void TryTriggerNextTask()
        {
            if (m_MandatoryTasksInProgress == 0)
                ExecuteNextTasks();
        }

        void KillWholeProcess(AsyncProcessTask responsible)
        {
            Debug.LogError($"[color=cyan]{responsible.Name}>[/color] {responsible.State}: Cancelling whole process");
            m_Cancelled = true;
            Callback.CompleteProcess(ProcessTaskState.done_fail_subtaskFailed);
        }
    }
}