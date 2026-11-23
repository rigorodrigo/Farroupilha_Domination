using UnityEngine;

// Troca o sprite do forte pro quadro "atirando" quando ele pupupu
[RequireComponent(typeof(SpriteRenderer))]
public class ForteVisual : MonoBehaviour
{
    [SerializeField] private Sprite parado;
    [SerializeField] private Sprite atirando;
    [SerializeField] private float duracaoDoTiro = 0.25f;

    private SpriteRenderer spriteRenderer;
    private float tempo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = parado;
    }

    private void Update()
    {
        if (tempo <= 0f) return;

        tempo -= Time.deltaTime;
        if (tempo <= 0f) spriteRenderer.sprite = parado;
    }

    public void MostrarTiro()
    {
        tempo = duracaoDoTiro;
        spriteRenderer.sprite = atirando;
    }
}
