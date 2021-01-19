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
    private Transform attack1HitBoxPos;

    [SerializeField]
    private LayerMask whatIsDamageable;

    private bool gotInput, isAttacking, isFirstAttack;
    private float lastInputTime = Mathf.NegativeInfinity;

    private float[] attackDetails = new float[2];

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

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

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

    private void Damage(float[] attackDetails)
    {
        if (!PC.GetDashStatus())
        {
            int direction;

            PS.DecreaseHealth(attackDetails[0]);

            if (attackDetails[1] < transform.position.x)
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
