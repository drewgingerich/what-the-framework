using System;
using System.Collections;
using UnityEngine;

public class WTState<T> : ScriptableObject
{
    public Action<T> Enter = null;
    public Func<T, IEnumerator> EnterRoutine = null;
    public Action<T> Update = null;
    public Action<T> Exit = null;
    public Func<T, WTState<T>> GetNextState = null;

    public WTState(
        Action<T> Enter,
        Func<T, IEnumerator> EnterRoutine,
        Action<T> Update,
        Action<T> Exit,
        Func<T, WTState<T>> GetNextState
    )
    {
        this.Enter = Enter;
        this.EnterRoutine = EnterRoutine;
        this.Update = Update;
        this.Exit = Exit;
        this.GetNextState = GetNextState;
    }
}