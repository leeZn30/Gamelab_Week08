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
        if (collision.transform.CompareTag("EnemyAttack")) // ������ ���� ����
        {
            _playerController.playerContext.CanPlayerDamaged();
            
            if(_playerController.playerContext.GetState().GetType() == typeof(DamagedState)) // Ÿ�� ���� -> �ǰ� ���� ����
            {
                _playerController.currentHP -= 1;
                if (_playerController.currentHP <= 0)
                {
                    _playerController.playerContext.CanPlayerDied();
                    // �״� �ִϸ��̼�

                    // ���� �ð� �� ������

                }
                else
                {
                    // Ÿ�� ���ϴ� �ִϸ��̼� �ʿ�

                }
            }
            else // ���� ������ �� ������, ���� �Ƹ� ���¶� �������� �԰� ���´� �״�� �����ǰ� ���� �� ����
            {
                if(_playerController.playerContext.GetHurtEffect())
                {
                    _playerController.currentHP -= 1;
                    if (_playerController.currentHP <= 0) // �������� �԰�, ������ �׾��� �ǰ� ����Ʈ�� �������� �ʴ´�.
                    {
                        _playerController.playerContext.CanPlayerDied();
                        // �״� �ִϸ��̼�

                        // ���� �ð� �� ������

                    }
                }
            }
        }
    }
}
