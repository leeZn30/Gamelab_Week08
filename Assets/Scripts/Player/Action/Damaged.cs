using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damaged : MonoBehaviour
{
    private PlayerController _playerController;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("EnemyAttack")) // 적에게 공격 당함
        {
            _playerController.playerContext.CanPlayerDamaged();
            
            if(_playerController.playerContext.GetState().GetType() == typeof(DamagedState)) // 타격 상태 -> 피격 경직 존재
            {
                _playerController.currentHP -= 1;
                if (_playerController.currentHP <= 0)
                {
                    _playerController.playerContext.CanPlayerDied();
                    // 죽는 애니메이션

                    // 일정 시간 후 리스폰

                }
                else
                {
                    // 타격 당하는 애니메이션 필요

                }
            }
            else // 무적 상태일 수 있지만, 슈퍼 아머 상태라서 데미지만 입고 상태는 그대로 유지되고 있을 수 있음
            {
                if(_playerController.playerContext.GetHurtEffect())
                {
                    _playerController.currentHP -= 1;
                    if (_playerController.currentHP <= 0) // 데미지만 입고, 죽으면 죽었지 피격 이펙트는 존재하지 않는다.
                    {
                        _playerController.playerContext.CanPlayerDied();
                        // 죽는 애니메이션

                        // 일정 시간 후 리스폰

                    }
                }
            }
        }
    }
}
