using System;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public event Action<int> HealthChanged; // Eventos são show de bola
    public event Action Destroyed;          

    [SerializeField] private int maxHealth = 20;

    private int health;

    // Só leitura, pro HUD mostrar a vida
    public int Health => health;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        health = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        health = Mathf.Max(health - amount, 0);
        Debug.Log($"Base atingida! Vida: {health}/{maxHealth}");
        HealthChanged?.Invoke(health);
        if (health == 0) Destroyed?.Invoke();
    }
}
