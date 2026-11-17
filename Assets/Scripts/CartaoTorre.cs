using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Cardizinho da torreta no HUD. Mostra tudo que precisa
public class CartaoTorre : MonoBehaviour
{
    [SerializeField] private Image moldura;
    [SerializeField] private Image fundo;
    [SerializeField] private Image icone;
    [SerializeField] private TMP_Text textoTecla;
    [SerializeField] private TMP_Text textoNome;
    [SerializeField] private TMP_Text textoCusto;

    [Header("Cores")]
    [SerializeField] private Color corMolduraNormal = new Color(0.32f, 0.32f, 0.32f);
    [SerializeField] private Color corMolduraSelecionada = Color.white;
    [SerializeField] private Color corFundoNormal = new Color(0.12f, 0.12f, 0.12f);
    [SerializeField] private Color corFundoSelecionado = new Color(0.24f, 0.24f, 0.24f);
    [SerializeField] private Color corCusto = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private Color corSemPila = new Color(0.95f, 0.4f, 0.35f);

    private LojaDeTorres loja;
    private int indice;

    // O HUD chama logo depois de criar o cartão
    public void Configurar(LojaDeTorres loja, int indice)
    {
        this.loja = loja;
        this.indice = indice;

        OpcaoTorre opcao = loja.PegarOpcao(indice);
        icone.sprite = opcao.icone;
        textoTecla.text = $"{indice + 1}";
        textoNome.text = opcao.nome;
        textoCusto.text = $"{opcao.custo} pila";
    }

    public void Clicar()
    {
        loja.Selecionar(indice);
    }

    private void Update()
    {
        if (loja == null) return;

        // Cartão selecionado
        bool selecionado = loja.Selecionada == indice;
        moldura.color = selecionado ? corMolduraSelecionada : corMolduraNormal;
        fundo.color = selecionado ? corFundoSelecionado : corFundoNormal;
        transform.localScale = selecionado ? Vector3.one * 1.08f : Vector3.one;

        // Sem money
        bool temPila = loja.PodeComprar(indice);
        textoCusto.color = temPila ? corCusto : corSemPila;
        icone.color = temPila ? Color.white : Color.gray;
    }
}
