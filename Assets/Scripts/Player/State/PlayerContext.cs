using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContext
{
    PlayerState myState;
    bool _hurtEffect;
    bool _invincible;

    public PlayerContext()
    {
        myState = new IdleState();
        _hurtEffect = false;
        _invincible = false;
    }

    public void ChangeState(PlayerState state)
    {
        myState = state;
    }

    public PlayerState GetState()
    {
        return myState;
    }

    public void SetEffect(bool invincible, bool effect) 
    {
        _invincible = invincible;
        _hurtEffect = effect;
    }

    public bool GetHurtEffect()
    {
        return _hurtEffect;
    }

    public bool GetInvincible()
    {
        return _invincible;
    }

    public void CanPlayerAttack()
    {
        myState.PlayerAttack(this);
    }

    public void CanPlayerHit()
    {
        myState.PlayerHit(this);
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
