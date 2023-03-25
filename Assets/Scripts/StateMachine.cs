using System;
using System.Collections.Generic;
/*

public class StateMachine
{
    protected State state_;
    protected List<State> lockedStates_ = new List<State>();
    protected List<(State, State)> invalidTransitions_ = new List<(State, State)>();
    protected bool allowForceChange_;
    public StateMachine(State state = State.none, bool allowForceChange = false) 
    { 
        state_ = state;
        allowForceChange_ = allowForceChange;
    }
    public State GetState() => state_;
    private bool SetState(State state)  { state_ = state; return true; }
    public void AllowForceChange() => allowForceChange_ = !allowForceChange_;
    public void AllowForceChange(bool allowForceChange) => allowForceChange_ = allowForceChange;
    public bool CheckLockedState(State state) => state_ != state && (lockedStates_.Contains(state) || lockedStates_.Contains(State.all));
    public void LockState(State state) => lockedStates_.Add(state);
    public void KeepState() => lockedStates_.Add(State.all);
    public void ResetKeepState() => lockedStates_.Remove(State.all);
    public void UnlockAll() => lockedStates_.Clear();
    public void UnlockState(State state) => lockedStates_.Remove(state);
    public bool ChangeState(State state) => CheckValidTransition(state_, state) ? SetState(state) : false;
    public void ForceState(State state) => state_ = state;
    public bool CheckValidTransition(State stateA, State stateB) => !invalidTransitions_.Contains((stateA, stateB));
    public void BlockTransition(State stateA, State stateB) => invalidTransitions_.Add((stateA, stateB));
    public bool IsIdle() => state_ == State.idle;
    public bool IsRunning() => state_ == State.running;
    public bool IsDashing() => state_ == State.dashing;
    public bool IsJumping() => state_ == State.jumping;
    public bool IsDoubleJumping() => state_ == State.doubleJumping;
    public bool IsAirDodging() => state_ == State.airDodging;
    public bool IsFastFalling() => state_ == State.fastFalling;
    public bool IsAttacking() => state_ == State.attacking;
    public void Idle() => state_ = State.idle;
    public void Running() => state_ = State.running;
    public void Dashing() => state_ = State.dashing;
    public void Jumping() => state_ = State.jumping;
    public void DoubleJumping() => state_ = State.doubleJumping;
    public void AirDodging() => state_ = State.airDodging;
    public void FastFalling() => state_ = State.fastFalling;
    public void Attacking() => state_ = State.attacking;
}
[Flags]
public enum State
{
    none = 0,
    idle = 1,
    running = 2,
    dashing = 4,
    GROUND = idle | running | dashing,
    jumping = 16,
    doubleJumping = 32,
    airDodging = 64,
    fastFalling = 128,
    INAIR = jumping | airDodging | fastFalling,
    attacking = 512,
    all = 1024,

}

*/