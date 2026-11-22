using System;
using UnityEngine;

// Código base de um inimigo base
[RequireComponent(typeof(Rigidbody2D))] // Unity adiciona rigidbody sozinha daí quando criar um inimigo, magia isso
public class Enemy : MonoBehaviour
{
    // Eventos do inimiog
    public static event Action<Enemy> Died;

    [SerializeField] private float maxHealth = 10f; // Vida inicial também
    [SerializeField] private float speed = 1.5f;

    public WaypointPath path; // Caminho que será seguido

    public float VidaPercentual => health / maxHealth;
    public bool Morto => isRemoved;
    public bool Parado { get; private set; }
    public bool ChegouNoFim => path != null && targetIndex >= path.waypoints.Length;

    private Rigidbody2D body;
    private BarraDeVida barra;
    private float health;
    private int targetIndex;
    private bool isRemoved;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        barra = GetComponent<BarraDeVida>();
        health = maxHealth;
    }

    // Basicamente anda até chegar no próximo waypont do caminho, depois manda "avança" a target pro próximo waypont
    private void FixedUpdate()
    {
        if (Parado || path == null || ChegouNoFim) return;

        Vector2 target = path.waypoints[targetIndex].position;
        Vector2 newPosition = Vector2.MoveTowards(body.position, target, speed * Time.fixedDeltaTime);
        body.MovePosition(newPosition);

        if (newPosition == target) targetIndex++;
    }

    public void Parar()
    {
        Parado = true;
    }

    public void TakeDamage(float amount)
    {
        if (isRemoved) return;

        health -= amount;
        if (barra != null) barra.Atualizar(VidaPercentual);

        if (health <= 0f)
        {
            isRemoved = true;
            Died?.Invoke(this);
            Destroy(gameObject);
        }
    }

    public void MultiplicarVida (float multiplicador)
    {
        maxHealth *= multiplicador;
        health = maxHealth;
    }
}
