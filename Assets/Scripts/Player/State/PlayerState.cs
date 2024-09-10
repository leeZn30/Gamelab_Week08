using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// reference
// https://inpa.tistory.com/entry/GOF-%F0%9F%92%A0-%EC%83%81%ED%83%9CState-%ED%8C%A8%ED%84%B4-%EC%A0%9C%EB%8C%80%EB%A1%9C-%EB%B0%B0%EC%9B%8C%EB%B3%B4%EC%9E%90
// https://steadycodist.tistory.com/entry/CUnity%EB%94%94%EC%9E%90%EC%9D%B8%ED%8C%A8%ED%84%B4-%EC%83%81%ED%83%9C-%ED%8C%A8%ED%84%B4State-Pattern

public enum State
{
    Idle, Move, Attack, Dodge, ChargeAttack, FullChargeAttack, Damaged, Dead
}

public interface PlayerState
{
    void PlayerIdle(PlayerContext pc);

    void PlayerMove(PlayerContext pc);

    void PlayerDodge(PlayerContext pc);

    void PlayerAttack(PlayerContext pc);

    void PlayerHit(PlayerContext pc);

    void PlayerChargeAttack(PlayerContext pc);

    void PlayerFullChargeAttack(PlayerContext pc);

    void PlayerDamaged(PlayerContext pc);

    void PlayerDied(PlayerContext pc);
}

class IdleState : PlayerState
{
    #region singleton
    private static class SingleInstanceHolder
    {
        private static IdleState _instance;
        public static IdleState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new IdleState();

                return _instance;
            }
        }
    }

    public static IdleState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }
    #endregion

    public void PlayerAttack(PlayerContext pc)
    {
        pc.ChangeState(AttackState.getInstance());
    }

    public void PlayerMove(PlayerContext pc)
    {
        pc.ChangeState(MoveState.getInstance());
        Debug.Log("State : Move");
    }

    public void PlayerIdle(PlayerContext pc)
    {
        // current Idle
    }

    public void PlayerDodge(PlayerContext pc)
    {
        pc.ChangeState(DodgeState.getInstance());
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {
        pc.ChangeState(ChargeState.getInstance());
    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        pc.ChangeState(FullChargeState.getInstance());
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        //pc.ChangeState(DamagedState.getInstance());
        pc.SetEffect(false, true);
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class AttackState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static AttackState _instance;
        public static AttackState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AttackState();

                return _instance;
            }
        }
    }

    public static AttackState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerMove(PlayerContext pc)
    {
        // Can't move while attacking
    }

    public void PlayerAttack(PlayerContext pc)
    {
        // Can't attack while attacking
    }

    public void PlayerIdle(PlayerContext pc)
    {
        //pc.ChangeState(IdleState.getInstance());
    }

    public void PlayerDodge(PlayerContext pc)
    {
        pc.ChangeState(DodgeState.getInstance());
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {

    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        // cant go full charge attack state from attack state
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        // no behaviour canceling, but damaged
        pc.SetEffect(false, true);
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        pc.ChangeState(HitState.getInstance());
    }
}

class HitState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static HitState _instance;
        public static HitState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new HitState();

                return _instance;
            }
        }
    }

    public static HitState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerMove(PlayerContext pc)
    {
        // Can't move while attacking
    }

    public void PlayerAttack(PlayerContext pc)
    {
        // Can't attack while attacking
    }

    public void PlayerIdle(PlayerContext pc)
    {
        pc.ChangeState(IdleState.getInstance());
    }

    public void PlayerDodge(PlayerContext pc)
    {
        // cant dodge while attacking
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {

    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        // cant go full charge attack state from attack state
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        // no behaviour canceling, but damaged
        pc.SetEffect(false, false);
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class MoveState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static MoveState _instance;
        public static MoveState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MoveState();

                return _instance;
            }
        }
    }

    public static MoveState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerMove(PlayerContext pc)
    {
        // already Moving
    }

    public void PlayerAttack(PlayerContext pc)
    {
        pc.ChangeState(AttackState.getInstance());
    }

    public void PlayerIdle(PlayerContext pc)
    {
        pc.ChangeState(IdleState.getInstance());
        Debug.Log("State : Idle");
    }

    public void PlayerDodge(PlayerContext pc)
    {
        pc.ChangeState(DodgeState.getInstance());
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        //pc.ChangeState(DamagedState.getInstance());
        // get hurt animation
        pc.SetEffect(false, true);
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class DodgeState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static DodgeState _instance;
        public static DodgeState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DodgeState();

                return _instance;
            }
        }
    }

    public static DodgeState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerIdle(PlayerContext pc)
    {
        pc.ChangeState(IdleState.getInstance());
    }

    public void PlayerMove(PlayerContext pc)
    {
        // cant move while dodging
    }

    public void PlayerDodge(PlayerContext pc)
    {
        // already dodging
    }

    public void PlayerAttack(PlayerContext pc)
    {
        // cant attack while dodging
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        // invincible while dodging
        pc.SetEffect(true, false);
    }

    public void PlayerDied(PlayerContext pc)
    {
        // invincible while dodging
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class ChargeState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static ChargeState _instance;
        public static ChargeState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ChargeState();

                return _instance;
            }
        }
    }

    public static ChargeState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerIdle(PlayerContext pc)
    {
        // pc.ChangeState(IdleState.getInstance());
    }

    public void PlayerMove(PlayerContext pc)
    {
        //
    }

    public void PlayerDodge(PlayerContext pc)
    {
        //
    }

    public void PlayerAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        //
        pc.ChangeState(FullChargeState.getInstance());
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        // no behaviour canceling, but damaged
        pc.SetEffect(false, false);
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class FullChargeState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static FullChargeState _instance;
        public static FullChargeState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FullChargeState();

                return _instance;
            }
        }
    }

    public static FullChargeState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerIdle(PlayerContext pc)
    {
        //pc.ChangeState(IdleState.getInstance());
    }

    public void PlayerMove(PlayerContext pc)
    {
        //
    }

    public void PlayerDodge(PlayerContext pc)
    {
        //
    }

    public void PlayerAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        // no behaviour canceling, but damaged
        pc.SetEffect(false, false);
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class DamagedState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static DamagedState _instance;
        public static DamagedState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DamagedState();

                return _instance;
            }
        }
    }

    public static DamagedState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerIdle(PlayerContext pc)
    {
        pc.ChangeState(IdleState.getInstance());
    }

    public void PlayerMove(PlayerContext pc)
    {
        //
    }

    public void PlayerDodge(PlayerContext pc)
    {
        //
    }

    public void PlayerAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {
        //
    }

    public void PlayerDamaged(PlayerContext pc)
    {
        //
    }

    public void PlayerDied(PlayerContext pc)
    {
        pc.ChangeState(DeadState.getInstance());
    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}

class DeadState : PlayerState
{
    private static class SingleInstanceHolder
    {
        private static DeadState _instance;
        public static DeadState Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DeadState();

                return _instance;
            }
        }
    }

    public static DeadState getInstance()
    {
        return SingleInstanceHolder.Instance;
    }

    public void PlayerIdle(PlayerContext pc)
    {

    }

    public void PlayerMove(PlayerContext pc)
    {

    }

    public void PlayerDodge(PlayerContext pc)
    {

    }

    public void PlayerAttack(PlayerContext pc)
    {

    }

    public void PlayerChargeAttack(PlayerContext pc)
    {

    }

    public void PlayerFullChargeAttack(PlayerContext pc)
    {

    }

    public void PlayerDamaged(PlayerContext pc)
    {

    }

    public void PlayerDied(PlayerContext pc)
    {

    }

    public void PlayerHit(PlayerContext pc)
    {
        
    }
}