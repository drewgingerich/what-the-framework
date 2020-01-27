using System;
using System.Collections;
using UnityEngine;

public class WTStateMachine<T> : MonoBehaviour
{
    public bool useUnityUpdate = false;
    public T data;

    private bool waiting = false;
    private WTState<T> state;
    private Coroutine enterRoutine;


    public void Update()
    {
        if (useUnityUpdate)
            StateUpdate();
    }

    public void StateUpdate()
    {
        if (waiting)
            return;

        var nextState = this.state.GetNextState(data);
        if (nextState != null)
            ChangeState(nextState);
    }

    public void ChangeState(WTState<T> nextState, bool waitForRoutine=false)
    {
        if (waitForRoutine)
        {
            StartCoroutine(ChangeOnRoutineEnd(nextState));
            return;
        }

        if (enterRoutine != null)
            StopCoroutine(enterRoutine);

        while(nextState != null)
        {
            state.Exit(data);
            nextState.Enter(data);
            if (nextState.EnterRoutine != null)
                StartCoroutine(RunEnterRoutine(nextState.EnterRoutine));
            state = nextState;
            nextState = state.GetNextState(data);
        }
    }

    private IEnumerator ChangeOnRoutineEnd(WTState<T> nextState)
    {
        waiting = true;
        while (enterRoutine != null)
            yield return null;
        ChangeState(nextState);
        waiting = false;
    }

    private IEnumerator RunEnterRoutine(Func<T, IEnumerator> routine)
    {
        enterRoutine = StartCoroutine(routine(data));
        yield return enterRoutine;
        enterRoutine = null;
    }
}