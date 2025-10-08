using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public State CurrentState = State.Idle;
    public enum State
    {
        Walking,
        Idle
    }
}
