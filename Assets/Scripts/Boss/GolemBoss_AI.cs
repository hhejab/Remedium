using System.Collections;
using UnityEngine;

public class GolemBoss_AI : Boss_AI
{
    [Header("Golem Settings")]
    public float heavyAttackDelay = 0.25f;

    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        StopMoving();

        animator.SetBool("isMoving", false);
        animator.SetBool("isRunning", false);

        FacePlayer();

        yield return new WaitForSeconds(heavyAttackDelay);

        yield return new WaitForSeconds(telegraphTime);

        animator.SetBool("isAttacking", true);

        StartCoroutine(EnableAttackHitbox());

        yield return new WaitForSeconds(attackAnimTime);

        animator.SetBool("isAttacking", false);

        nextAttackTime = Time.time + attackCooldown;

        isAttacking = false;
    }
}