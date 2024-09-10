using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Attack;

public class HammerScript : MonoBehaviour
{
    private Attack _attack;
    private PlayerController _playerController;
    public GameObject _player;
    private Boss _boss;
    // Start is called before the first frame update
    void Start()
    {
        _playerController = _player.GetComponent<PlayerController>();
        _attack = _player.GetComponent<Attack>();
        _boss = FindObjectOfType<Boss>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            float damage = 0f;
            switch (_attack.attackVariable)
            {
                case AttackVariable.Normal:
                    damage = _playerController.normalAttackDamage;
                    break;
                case AttackVariable.NormalCombo:
                    damage = _playerController.normalAttackComboDamage;
                    break;
                case AttackVariable.NormalLastCombo:
                    damage = _playerController.normalAttackLastComboDamage;
                    break;
                case AttackVariable.Charge:
                    damage = _playerController.chargeAttackDamage;
                    break;
                case AttackVariable.FullCharge:
                    damage = _playerController.fullChargeAttackDamage;
                    break;
                default:
                    damage = 0f;
                    break;
            }

            //collision.gameObject.GetComponent<Boss>().OnDamaged(damage);
            _boss.OnDamaged(damage);
            _playerController.TurnOffHammerCollider();
        }
    }
}
