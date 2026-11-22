using System;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public event Action<int> HealthChanged; // Eventos são show de bola
    public event Action Destroyed;          

    [SerializeField] private int maxHealth = 100;

    private int health;
    private BarraDeVida barra;

    // Só leitura, pro HUD mostrar a vida
    public int Health => health;
    public int MaxHealth => maxHealth;

    private void Awake()
    {
        health = maxHealth;
        barra = GetComponent<BarraDeVida>();
    }

    public void TakeDamage(int amount)
    {
        if (health <= 0) return;

        health = Mathf.Max(health - amount, 0);
        HealthChanged?.Invoke(health);
        if (barra != null) barra.Atualizar((float)health / maxHealth);
        if (health == 0) Destroyed?.Invoke();
    }
}
