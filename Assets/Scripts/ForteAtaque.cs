using UnityEngine;

// O forte atira sozinho no inimigo mais próximo que estiver no alcance
public class ForteAtaque : MonoBehaviour
{
    [SerializeField] private float alcance = 4f;
    [SerializeField] private float intervalo = 1f;
    [SerializeField] private float dano = 2f;
    [SerializeField] private AudioClip somDoTiro;
    [SerializeField] private float volume = 0.5f;

    private float cooldown;
    private ForteVisual visual;

    private void Awake()
    {
        visual = GetComponent<ForteVisual>();
    }

    private void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown > 0f) return;

        Enemy alvo = InimigoMaisProximo();
        if (alvo == null) return;

        cooldown = intervalo;
        alvo.TakeDamage(dano);
        if (visual != null) visual.MostrarTiro();
        if (somDoTiro != null) AudioSource.PlayClipAtPoint(somDoTiro, Camera.main.transform.position, volume);
    }

    private Enemy InimigoMaisProximo()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, alcance);
        Enemy maisProximo = null;
        float menorDistancia = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy == null || enemy.Morto) continue;

            float distancia = Vector2.Distance(transform.position, enemy.transform.position);
            if (distancia < menorDistancia)
            {
                maisProximo = enemy;
                menorDistancia = distancia;
            }
        }
        return maisProximo;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, alcance);
    }
}
