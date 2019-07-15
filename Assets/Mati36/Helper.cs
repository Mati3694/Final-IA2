using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour {
    
    public void ExecuteWithDelay(Action action, float delay)
    {
        StartCoroutine(DelayRoutine(action, delay));
    }

    public void ExecuteAfterFrame(Action action)
    {
        StartCoroutine(AfterFrameRoutine(action));
    }

    IEnumerator DelayRoutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    IEnumerator AfterFrameRoutine(Action action)
    {
        yield return new WaitForEndOfFrame();
        action();
    }
}
