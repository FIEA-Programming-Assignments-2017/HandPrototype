/* Author:      Peter Finn
 * Description: This file contains all the code for objects of the class
 *              StationaryEnemy, which inherits from the class Enemy. A
 *              stationary enemy is one that moves between a fixed number of
 *              stationary positions and attacks, as opposed to moving enemies,
 *              which constantly move toward the player and attack when at a
 *              specific distance from the player.
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryEnemy : Enemy {

    public Vector2[] attackPositions;

    public GameObject bullet;

    private int indexOfNextPosition = 0;
    private bool firing = false;
    
    public void SetFiring(bool isFiring)
    {
        firing = isFiring;
    }

    /* Function:    Update
     * Description: This function is called once per frame. For the class
     *              StationaryEnemy, the update function acts as a finite state
     *              machine, transferring the EnemyState state object variable
     *              between a number of different values based on actions taken
     *              by the player and by the A.I.-controlled enemy represented
     *              by this object.
     */
    void Update ()
    {
        /* In each frame, we must take a certain action depending on the state
         * of the game and the state of this enemy. */

        // CONDITION 1
        /* If the player is not in the enemy's territory, then the state should
         * be set to IDLE1 if the enemy is still alive. */
        if (!IsPlayerInTerritory() && (GetState() != EnemyState.DYING)
            && GetState() != EnemyState.DYING)
        {
            SetState(EnemyState.IDLE1);
        }

        // CONDITION 2
        /* If the player is in the enemy's territory, and the enemy is
         * currently in its IDLE1 state, then the enemy should transition to
         * its MOVING state and start moving towards one of its predefined
         * attack positions. If the enemy is not in its IDLE1 state, then it
         * should attack the player if it is has reached the predefined
         * position it was heading for. */
        if (IsPlayerInTerritory())
        {
            if ((GetState() == EnemyState.IDLE1) ||
                (GetState() == EnemyState.MOVING))
            {
                SetState(EnemyState.MOVING);
                Vector2 currentPosition = new Vector2(transform.position.x, transform.position.z);
                Vector2 nextPosition = attackPositions[indexOfNextPosition];
                Vector2 directionToMove2D = nextPosition - currentPosition;
                if (directionToMove2D.magnitude < 0.1)
                { // reached position!
                    transform.position = new Vector3(attackPositions[indexOfNextPosition].x,
                        transform.position.y, attackPositions[indexOfNextPosition].y);
                    SetState(EnemyState.ATTACKING);
                    StartCoroutine(FirstPersonControler.ShootingBullet(GameObject.FindGameObjectWithTag("Player"), this));
                    SetLastAttackTime();
                    indexOfNextPosition = (indexOfNextPosition + 1) % attackPositions.Length;
                }
                else
                {
                    Vector3 directionToMove3D =
                        new Vector3(directionToMove2D.x, 0.0f,
                        directionToMove2D.y);
                    directionToMove3D = Vector3.Normalize(directionToMove3D);
                    directionToMove3D *= movementSpeed;
                    transform.position += directionToMove3D;
                }
            }
            else if (GetState() == EnemyState.ATTACKING)
            {
                if (Time.timeSinceLevelLoad - getLastAttackTime() > durationOfAttack)
                {
                    SetState(EnemyState.MOVING);
                }
            }
        }

        // CONDITION 3
        /* If the enemy has been attacked by the player, this should result in
         * a decrease to the enemy's health, causing the enemy to turn red to
         * indicate to the player that they have successfully landed a blow.
         * All of this is handled by the function DecreaseHealth; in this
         * branch of Update, we restore the enemy's original color. */
        if (Time.timeSinceLevelLoad - GetLastHitTime() > hitDuration)
        {
            GetComponent<MeshRenderer>().material.color = GetColor();
        }

        // CONDITION 4
        /* If the player gets hit by this enemy's attacks, and the enemy
         * celebrates its attacks, this enemy should enter its CELEBRATING
         * state. */
        /*if (shouldCelebrate)
        {
            SetState(EnemyState.CELEBRATING);
            SetLastCelebrationTime();
        }*/

        // CONDITION 5
        /* If the enemy runs out of health, then no matter what state they are
         * in (except DEAD or DYING), they must start dying. */
        if ((health <= 0) && (GetState() != EnemyState.DYING) &&
            (GetState() != EnemyState.DEAD))
        {
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
