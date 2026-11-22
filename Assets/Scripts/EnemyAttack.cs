using UnityEngine;

// A cada X segundos o inimigo ataca: o forte se estiver perto dele, senão a torre mais próxima
[RequireComponent(typeof(Enemy))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float intervalo = 2f;
    [SerializeField] private float alcanceNasTorres = 2f;
    [SerializeField] private float alcanceNoForte = 1.5f; // medido a partir da borda do forte
    [SerializeField] private float danoNaTorre = 2f;
    [SerializeField] private int danoNoForte = 1;
    [SerializeField] private AudioClip somDoAtaque;
    [SerializeField] private float volume = 0.6f;

    private Enemy enemy;
    private EnemyVisual visual;
    private PlayerBase forte;
    private Collider2D colisorDoForte;
    private float cooldown;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        visual = GetComponent<EnemyVisual>();
    }

    private void Start()
    {
        forte = FindFirstObjectByType<PlayerBase>();
        if (forte != null) colisorDoForte = forte.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (enemy.Morto) return;

        if (!enemy.Parado && PertoDoForte()) enemy.Parar();

        cooldown -= Time.deltaTime;
        if (cooldown > 0f) return;

        if (enemy.Parado)
        {
            Atacar(forte.transform.position);
            forte.TakeDamage(danoNoForte);
            return;
        }

        Tower alvo = TorreMaisProxima();
        if (alvo == null) return; // nenhuma torre perto, só segue caminhando

        Atacar(alvo.transform.position);
        alvo.ReceberDano(danoNaTorre);
    }

    private bool PertoDoForte()
    {
        if (forte == null) return false;
        if (enemy.ChegouNoFim) return true;

        Vector2 pontoMaisPerto = colisorDoForte != null
            ? colisorDoForte.ClosestPoint(transform.position)
            : (Vector2)forte.transform.position;

        return Vector2.Distance(transform.position, pontoMaisPerto) <= alcanceNoForte;
    }

    private void Atacar(Vector2 alvo)
    {
        cooldown = intervalo;
        if (visual != null) visual.MostrarAtaque(alvo);
        if (somDoAtaque != null) AudioSource.PlayClipAtPoint(somDoAtaque, Camera.main.transform.position, volume);
    }

    private Tower TorreMaisProxima()
    {
        Tower maisProxima = null;
        float menorDistancia = alcanceNasTorres;

        foreach (Tower torre in Tower.Todas)
        {
            float distancia = Vector2.Distance(transform.position, torre.transform.position);
            if (distancia <= menorDistancia)
            {
                maisProxima = torre;
                menorDistancia = distancia;
            }
        }
        return maisProxima;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alcanceNasTorres);
    }
}
