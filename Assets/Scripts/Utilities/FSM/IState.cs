using System;

public interface IState<T> where T : Enum
{
    public IState<T> Initialize(FiniteStateMachine<T> fsm);
    void OnEnter(params object[] parameters);
    void OnUpdate();
    void OnExit();
}