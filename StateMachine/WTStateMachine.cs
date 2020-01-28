using System;
using System.Collections;
using UnityEngine;

public class WTStateMachine<T> : MonoBehaviour
{
    public event Action<WTState<T>> OnTransitionState;
    public event Action<WTState<T>> OnEnterState;
    public event Action<WTState<T>> OnExitState;
    public event Action<WTState<T>> OnUpdateState;



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

        state.Update(data);
        OnUpdateState?.Invoke(state);
    }

    public void ChangeState(WTState<T> nextState)
    {
        if (enterRoutine != null)
            StopCoroutine(enterRoutine);

        while(nextState != null)
        {
            state.Exit(data);
            OnExitState?.Invoke(state);
            nextState.Enter(data);
            OnEnterState?.Invoke(state);
            if (nextState.EnterRoutine != null)
                StartCoroutine(RunEnterRoutine(nextState.EnterRoutine));
            state = nextState;
            nextState = state.GetNextState(data);
        }
        OnTransitionState?.Invoke(state);
    }

    private IEnumerator RunEnterRoutine(Func<T, IEnumerator> routine)
    {
        enterRoutine = StartCoroutine(routine(data));
        yield return enterRoutine;
        enterRoutine = null;
    }
}