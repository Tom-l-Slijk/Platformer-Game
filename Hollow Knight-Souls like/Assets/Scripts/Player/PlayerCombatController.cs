using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField]
    private bool combatEnabled;

    [SerializeField]
    private float inputTimer, attack1Radius, attack1Damage;
    [SerializeField]
    private float stunDamageAmount = 1;

    [SerializeField]
    private Transform attack1HitBoxPos;

    [SerializeField]
    private LayerMask whatIsDamageable;

    private bool gotInput, isAttacking, isFirstAttack;
    private float lastInputTime = Mathf.NegativeInfinity;

    private AttackDetails attackDetails;

    public float timeBetweenAttack = 0.2f;

    private PlayerControllerScript PC;

    private PlayerStats PS;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        PC = GetComponent<PlayerControllerScript>();
        PS = GetComponent<PlayerStats>();
    }

    void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    void CheckCombatInput()
    {
        if (Input.GetButtonDown("Attack"))
        {
            if (combatEnabled)
            {
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    private void CheckAttacks()
    {
        if (gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
                StartCoroutine(AttackTimer());
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    IEnumerator AttackTimer()
    {
        combatEnabled = false;
        yield return new WaitForSeconds(timeBetweenAttack);
        combatEnabled = true;
    }

    private void CheckAttackHitbox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitBoxPos.position, attack1Radius, whatIsDamageable);

        attackDetails.damageAmount = attack1Damage;
        attackDetails.position = transform.position;
        attackDetails.stunDamageAmount = stunDamageAmount;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attackDetails);
        }
    }

    private void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack1", false);
    }

    private void Damage(AttackDetails attackDetails)
    {
        if (!PC.GetDashStatus())
        {
            int direction;

            PS.DecreaseHealth(attackDetails.damageAmount);

            if (attackDetails.position.x < transform.position.x)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }

            PC.Knockback(direction);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }

}
