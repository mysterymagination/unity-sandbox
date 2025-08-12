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
        IEnumerator taskHandle = InvokeDelayed(tag, timedEvent.unityEvent, timedEvent.delay, timedEvent.period, timedEvent.loop);
        TimedTaskMap.Add(tag, new TimedTask(taskHandle));
        StartCoroutine(taskHandle);
    }

    private IEnumerator InvokeDelayed(string tag, UnityEvent unityEvent, float delay, float period, bool loop)
    {
        yield return new WaitForSeconds(delay);
        unityEvent.Invoke();
        if (loop)
        {
            // TODO: need to be able to cancel this at some point; can we pass in our own IEnumerator?
            //  1. Check if we have a period handle yet
            //  2. iff no handle yet, call InvokeDelayed() again and store IEnumerator as the periodicHandle
            StartCoroutine(InvokeDelayed(unityEvent, period, 0, loop));
        }
    }

}
