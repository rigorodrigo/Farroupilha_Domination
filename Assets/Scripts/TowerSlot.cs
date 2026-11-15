using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TowerSlot : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite spriteLivre;
    [SerializeField] private Sprite spriteOcupado;

    [Header("Cores com o mouse em cima")]
    [SerializeField] private Color corPodeComprar = new Color(0.55f, 1f, 0.55f);
    [SerializeField] private Color corSemPila = new Color(1f, 0.45f, 0.45f);

    private LojaDeTorres loja;
    private Tower torreAtual;
    private SpriteRenderer sr;
    private bool mouseEmCima;

    private bool Ocupado => torreAtual != null;

    void Start()
    {
        loja = FindFirstObjectByType<LojaDeTorres>();
        sr = GetComponent<SpriteRenderer>();
        if (spriteLivre != null) sr.sprite = spriteLivre;
    }

    void Update()
    {
        if (Ocupado) return;

        Color cor = Color.white;
        if (mouseEmCima) cor = loja.PodeComprarSelecionada() ? corPodeComprar : corSemPila;

        cor.a = mouseEmCima ? 1f : 0.8f + Mathf.Sin(Time.time * 3f) * 0.2f;
        sr.color = cor;
    }

    private void OnMouseEnter() { mouseEmCima = true; }
    private void OnMouseExit() { mouseEmCima = false; }

    private void OnMouseDown()
    {
        if (Ocupado) return;

        torreAtual = loja.Comprar(transform.position);

        if (Ocupado)
        {
            if (spriteOcupado != null) sr.sprite = spriteOcupado;
            sr.color = Color.white;
        }
    }
}
