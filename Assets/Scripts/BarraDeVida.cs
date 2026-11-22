using UnityEngine;

public class BarraDeVida : MonoBehaviour
{
    [SerializeField] private float largura = 0.5f;
    [SerializeField] private float altura = 0.07f;
    [SerializeField] private float distanciaAcima = 0.05f;
    [SerializeField] private Color corDoFundo = new Color(0.15f, 0.15f, 0.15f);
    [SerializeField] private Color corVidaCheia = Color.green;
    [SerializeField] private Color corVidaBaixa = Color.red;

    private GameObject barra;
    private Transform preenchimento;
    private SpriteRenderer corDoPreenchimento;

    private void Awake()
    {
        float escala = transform.lossyScale.x;

        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        float topo = sprite.bounds.max.y - transform.position.y;

        barra = new GameObject("BarraDeVida");
        barra.transform.SetParent(transform, false);
        barra.transform.localPosition = new Vector3(0f, (topo + distanciaAcima) / escala, 0f);
        barra.transform.localScale = Vector3.one / escala;

        SpriteRenderer fundo = CriarRetangulo("Fundo", barra.transform, corDoFundo, 10);
        fundo.transform.localScale = new Vector3(largura, altura, 1f);

        corDoPreenchimento = CriarRetangulo("Vida", barra.transform, corVidaCheia, 11);
        preenchimento = corDoPreenchimento.transform;
        preenchimento.localScale = new Vector3(largura, altura, 1f);

        barra.SetActive(false);
    }

    public void Atualizar(float vida)
    {
        vida = Mathf.Clamp01(vida);
        barra.SetActive(vida < 1f);
        preenchimento.localScale = new Vector3(largura * vida, altura, 1f);
        corDoPreenchimento.color = Color.Lerp(corVidaBaixa, corVidaCheia, vida);
    }

    private SpriteRenderer CriarRetangulo(string nome, Transform pai, Color cor, int ordem)
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.SetParent(pai, false);

        Texture2D branco = Texture2D.whiteTexture;
        Sprite sprite = Sprite.Create(branco, new Rect(0, 0, branco.width, branco.height), new Vector2(0f, 0.5f), branco.width);

        SpriteRenderer sr = objeto.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = cor;
        sr.sortingOrder = ordem;

        objeto.transform.localPosition = new Vector3(-largura / 2f, 0f, 0f);
        return sr;
    }
}
