using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext
{
    PlayerState myState;
    bool hurtEffect;
    public PlayerContext()
    {
        myState = new IdleState();
        hurtEffect = false;
    }

    public void ChangeState(PlayerState state)
    {
        myState = state;
    }

    public PlayerState GetState()
    {
        return myState;
    }

    public void SetHurtEffect(bool effect) 
    {
        hurtEffect = effect;
    }

    public bool GetHurtEffect()
    {
        return hurtEffect;
    }

    public void CanPlayerAttack()
    {
        myState.PlayerAttack(this);
    }

    public void CanPlayerMove()
    {
        myState.PlayerMove(this);
    }

    public void CanPlayerIdle()
    {
        myState.PlayerIdle(this);
    }

    public void CanPlayerDodge()
    {
        myState.PlayerDodge(this);
    }

    public void CanPlayerChargeAttack()
    {
        myState.PlayerChargeAttack(this);
    }

    public void CanPlayerFullChargeAttack()
    {
        myState.PlayerFullChargeAttack(this);
    }

    public void CanPlayerDamaged()
    {
        myState.PlayerDamaged(this);
    }

    public void CanPlayerDied()
    {
        myState.PlayerDied(this);
    }
}
