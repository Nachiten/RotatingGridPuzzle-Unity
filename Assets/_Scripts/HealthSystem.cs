using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health = 100;
    
    public event Action OnDead;
    public event Action OnDamaged;
    
    private int healthMax;

    private void Awake()
    {
        healthMax = health;
    }

    public void Damage(int damageAmount)
    {
        health = Math.Max(health - damageAmount, 0);

        OnDamaged?.Invoke();

        if (health == 0)
            Die();

        Debug.Log($"{name} damaged! Health: {health}");
    }

    private void Die()
    {
        OnDead?.Invoke();
    }

    public float GetHealthNormalized()
    {
        return (float) health / healthMax;
    }
}