using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private Sprite[] andando;
    [SerializeField] private Sprite[] atacando;
    [SerializeField] private float quadrosPorSegundo = 6f;
    [SerializeField] private float duracaoDoAtaque = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Enemy enemy;
    private Vector3 ultimaPosicao;
    private float tempo;
    private float tempoDeAtaque;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemy = GetComponent<Enemy>();
        ultimaPosicao = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        if (tempoDeAtaque > 0f)
        {
            tempoDeAtaque -= Time.deltaTime;
            float progresso = 1f - tempoDeAtaque / duracaoDoAtaque;
            int quadro = Mathf.Min((int)(progresso * atacando.Length), atacando.Length - 1);
            spriteRenderer.sprite = atacando[quadro];
            return;
        }

        if (enemy != null && enemy.Parado)
        {
            spriteRenderer.sprite = andando[0];
            return;
        }

        tempo += Time.deltaTime;
        int indice = (int)(tempo * quadrosPorSegundo) % andando.Length;
        spriteRenderer.sprite = andando[indice];

        float deslocamentoX = transform.position.x - ultimaPosicao.x;
        if (Mathf.Abs(deslocamentoX) > 0.001f) spriteRenderer.flipX = deslocamentoX < 0f;
        ultimaPosicao = transform.position;
    }

    public void MostrarAtaque(Vector2 alvo)
    {
        if (atacando.Length == 0) return;

        tempoDeAtaque = duracaoDoAtaque;
        spriteRenderer.flipX = alvo.x < transform.position.x;
    }
}
