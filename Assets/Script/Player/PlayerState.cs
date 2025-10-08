using UnityEngine;

public class PlayerState
{
    private State _currentState = State.Idle;
    public State CurrentState => _currentState;
    public enum State
    {
        Walking,
        Idle
    }
}
