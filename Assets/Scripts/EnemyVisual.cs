using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyVisual : MonoBehaviour
{
    [SerializeField] private Sprite[] andando;
    [SerializeField] private float quadrosPorSegundo = 6f;
    
    private SpriteRenderer spriteRenderer;
    private Vector3 ultimaPosicao;
    private float tempo;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ultimaPosicao = transform.position;
    }
    

    // Update is called once per frame
    void Update()
    {
     tempo += Time.deltaTime;
     int indice = (int)(tempo * quadrosPorSegundo) % andando.Length;
     spriteRenderer.sprite = andando[indice];   
     
     float deslocamentoX = transform.position.x - ultimaPosicao.x;
     if (Mathf.Abs(deslocamentoX) > 0.001f) spriteRenderer.flipX = deslocamentoX < 0f;
     ultimaPosicao = transform.position;
    }
}
