using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaKill : MonoBehaviour

{
    public int health;
    public int maxHealth = 1;
    private bool isDead;

    public GameManaScript gameMana;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0 && !isDead)
        {
            isDead = true;
            Destroy(gameObject);
            gameMana.GameOver();
        }
    }

    public void HealUp(int amount)
    {
        health += amount;
        if (health >= 1 && !isDead)
        {
            isDead = true;
            Destroy(gameObject);
            gameMana.GameWin();
        }
    }
}
