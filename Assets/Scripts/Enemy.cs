using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public enum EnemyState { IDLE1 = 0, IDLE2, MOVING, ATTACKING, CELEBRATING, DYING, DEAD };
    public float durationOfAttack;
    public float durationOfDeath;
    public float movementSpeed;
    public float hitDuration;
    public int health;
    public bool shouldCelebrate;

    protected Collider territory;

    private EnemyState state = EnemyState.IDLE1;
    private bool playerIsInTerritory = false;
    private float lastAttackTime = 0.0f;
    private float deathTime = 0.0f;
    private float lastHitTime = 0.0f;
    private float lastCelebrationTime = 0.0f;
    private Color color;

    public void DecreaseHealth(int amount)
    {
        
        int tempHealth = health - amount;
        if (tempHealth <= 0)
        {
            health = 0;
        }
        else
        {
            health = tempHealth;
        }
        
        if (amount > 0)
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
            SetLastHitTime();
        }

        Debug.Log(name + "'s health is now " + health);//DEBUG
    }

    public float GetLastHitTime()
    {
        return lastHitTime;
    }

    public void SetLastHitTime()
    {
        lastHitTime = Time.timeSinceLevelLoad;
    }

    public Color GetColor()
    {
        return color;
    }

    public void SetColor(Color c)
    {
        color = c;
    }

    public float GetDeathTime()
    {
        return deathTime;
    }

    public void SetDeathTime()
    {
        deathTime = Time.timeSinceLevelLoad;
    }

    public void setPlayerIsInTerritory(bool input)
    {
        playerIsInTerritory = input;
    }

    protected bool IsPlayerInTerritory()
    {
        return playerIsInTerritory;
    }

    public float getLastAttackTime()
    {
        return lastAttackTime;
    }

    public void SetLastAttackTime()
    {
        lastAttackTime = Time.timeSinceLevelLoad;
    }

    protected EnemyState SetState(EnemyState newState)
    {
        state = newState;
        return state;
    }

    protected EnemyState GetState()
    {
        return state;
    }

    /* Function:    Start
     * Description: This function is called at the beginning of the scene. It
     *              initializes the "territory" member object of this class and
     *              stores the enemy's default color.
     */
    public void Start()
    {
        GameObject child = GameObject.Find(name + " Territory");
        territory = child.GetComponent<Collider>();
        SetColor(GetComponent<MeshRenderer>().material.color);
    }
	
}
