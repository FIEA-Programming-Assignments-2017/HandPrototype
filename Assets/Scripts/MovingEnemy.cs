using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : Enemy {

    private GameObject player;

    public float distanceFromPlayerToAttack;

    public bool IsEnemyCloseEnoughToAttack()
    {
        Vector3 vectorFromPlayerToEnemy = transform.position -
            player.transform.position;
        float distanceFromPlayerToEnemy = vectorFromPlayerToEnemy.magnitude;
        return (distanceFromPlayerToEnemy <= distanceFromPlayerToAttack);
    }

    private void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    /* Function:    Update
     * Description: This function is called once per frame. For the class
     *              MovingEnemy, the update function acts as a finite state
     *              machine, transferring the EnemyState state object variable
     *              between a number of different values based on actions taken
     *              by the player and by the A.I.-controlled enemy represented
     *              by this object.
     */
    private void Update()
    {
        /* In each frame, we must take a certain action depending on the state
         * of the game and the state of this enemy. */

        // CONDITION 1
        /* If the player is not in the enemy's territory, then the state should
         * be set to IDLE1. */
        if (!IsPlayerInTerritory())
        {
            SetState(EnemyState.IDLE1);
        }

        // CONDITION 2
        /* If the player is in the enemy's territory, then it should either (a)
         * move in the player's direction, or (b) attack the player, assuming
         * it's not already in the process of attacking or dying. */
        if (IsPlayerInTerritory())
        {
            /* If the enemy is already dying or dead, do nothing. */
            if (GetState() == EnemyState.DYING || GetState() == EnemyState.DEAD)
            {

            }
            /* If the enemy has finished its attack, reset its state. */
            else if ((GetState() == EnemyState.ATTACKING) &&
                (Time.timeSinceLevelLoad - getLastAttackTime() > durationOfAttack))
            {
                SetState(EnemyState.MOVING);
            }
            /* If the enemy is close enough to the player to mount an attack,
             * and it's not currently attacking, attack! */
            else if (IsEnemyCloseEnoughToAttack())
            {
                if (GetState() != EnemyState.ATTACKING)
                {
                    SetState(EnemyState.ATTACKING);
                    SetLastAttackTime();
                }
            }
            /* If the player is in the enemy's territory, and all else fails,
             * the enemy just keeps moving toward the player. */
            else
            {
                Vector2 playerPosition = new Vector2(player.transform.position.x, player.transform.position.z);
                Vector2 enemyPosition = new Vector2(transform.position.x, transform.position.z);
                Vector2 enemyToPlayer2D = playerPosition - enemyPosition;
                Vector3 enemyToPlayer3D = new Vector3(enemyToPlayer2D.x, 0.0f, enemyToPlayer2D.y);
                enemyToPlayer3D = Vector3.Normalize(enemyToPlayer3D);
                enemyToPlayer3D *= movementSpeed;
                transform.position += enemyToPlayer3D;
            }
        }

        // CONDITION 3
        /* If the enemy has been attacked by the player, this should result in
         * a decrease to the enemy's health, causing the enemy to turn red to
         * indicate to the player that they have successfully landed a blow.
         * All of this is handled by the function DecreaseHealth; in this
         * branch of Update, we restore the enemy's original color if the
         * redness has been around for as long as it should be. */
        if (Time.timeSinceLevelLoad - GetLastHitTime() > hitDuration)
        {
            GetComponent<MeshRenderer>().material.color = GetColor();
        }

        // CONDITION 4
        /* If the player gets hit by this enemy's attacks, and the enemy
         * celebrates its attacks, this enemy should enter its CELEBRATING
         * state. */

        // CONDITION 5
        /* If the enemy runs out of health, then no matter what state they are
         * in (except DEAD or DYING), they must start dying. */
        if ((health <= 0) && (GetState() != EnemyState.DYING) && 
            (GetState() != EnemyState.DEAD))
        {
            Debug.Log("Moving enemy is dying now.");//DEBUG
            SetState(EnemyState.DYING);
            SetDeathTime();
        }

        // CONDITION 6
        /* If the enemy's state is DYING, and the dying animation has finished,
         * then we should change the enemy's state to DEAD and disable the
         * enemy's game object. */
        if ((GetState() == EnemyState.DYING) &&
            (Time.timeSinceLevelLoad - GetDeathTime() > durationOfDeath))
        {
            SetState(EnemyState.DEAD);
            gameObject.SetActive(false);
        }
    }
}
