using UnityEngine;
using System.Collections;
using System;

public static class WaitAndDoExtension
{
    /// <summary>
    /// Waits given time and then call given action. Note: It starts new coroutine and returns reference to it, so there is no need to start this coroutine manually.
    /// </summary>
    public static IEnumerator WaitAndDo(this MonoBehaviour monoBehaviour, Action action, float delay)
    {
        IEnumerator Coroutine()
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        IEnumerator x = Coroutine();
        monoBehaviour.StartCoroutine(x);
        return x;
    }

    /// <summary>
    /// Waits given realtime and then call given action. Note: It starts new coroutine and returns reference to it, so there is no need to start this coroutine manually.
    /// </summary>
    public static IEnumerator WaitAndDoRealtime(this MonoBehaviour monoBehaviour, Action action, float delay)
    {
        IEnumerator Coroutine()
        {
            yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
        }

        IEnumerator x = Coroutine();
        monoBehaviour.StartCoroutine(x);
        return x;
    }

    /// <summary>
    /// Waits for end of the frame and then call given action.
    /// </summary>
    public static void WaitForEndOfFrameAndDo(this MonoBehaviour monoBehaviour, Action action)
    {
        IEnumerator Coroutine()
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
        monoBehaviour.StartCoroutine(Coroutine());
    }
}