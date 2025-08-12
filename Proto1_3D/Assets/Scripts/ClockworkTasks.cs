using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ClockworkTasks : MonoBehaviour
{
    [Serializable]
    public class TimedEvent
    {
        /**
         * The UnityEvent to run.
         */
        public UnityEvent unityEvent;
        /** 
         * Delay before the event initially runs, in seconds.
         */
        public float delay;
        /** 
         * Period at which the event should repeat after the initial run iff loop is true, in seconds.
         */
        public float period;
        /** 
         * True to run the event periodically every period seconds after the initial run, false to run only once.
         */
        public bool loop;
    }

    private class TimedTask
    {
        public TimedTask(){}
        public TimedTask(IEnumerator delayHandle)
        {
            initDelayHandle = delayHandle;
        }
        public TimedTask(IEnumerator delayHandle, IEnumerator periodHandle)
        {
            initDelayHandle = delayHandle;
            periodicHandle = periodHandle;
        }
        public IEnumerator initDelayHandle;
        public IEnumerator periodicHandle;
    }

    [SerializeField] public Dictionary<String, TimedTask> TimedTaskMap;

    public void LaunchClock(string tag, TimedEvent timedEvent)
    {
        IEnumerator delayTaskHandle = InvokeDelayed(tag, timedEvent.unityEvent, timedEvent.delay, timedEvent.loop);
        TimedTask task = new TimedTask(delayTaskHandle);
        if (timedEvent.loop)
        {
            task.periodicHandle = InvokeDelayed(tag, timedEvent.unityEvent, timedEvent.period, timedEvent.loop);
        }
        TimedTaskMap.Add(tag, task);
        StartCoroutine(delayTaskHandle);
    }

    private IEnumerator InvokeDelayed(string tag, UnityEvent unityEvent, float delay, bool loop)
    {
        yield return new WaitForSeconds(delay);
        unityEvent.Invoke();
        if (loop)
        {
            TimedTask task = new TimedTask();
            if (TimedTaskMap.TryGetValue(tag, out task))
            {
                StartCoroutine(task.periodicHandle);
            }
        }
    }

}
