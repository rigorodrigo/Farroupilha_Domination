using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Mostra na tela as informações da partida e a lista de torres
public class HUD : MonoBehaviour
{
    [Header("De onde vêm os dados")]
    [SerializeField] private PlayerStats jogador;
    [SerializeField] private PlayerBase forte;
    [SerializeField] private WaveSpawner geradorDeOndas;
    [SerializeField] private LojaDeTorres loja;
    [SerializeField] private GameManager gerenciador;

    [Header("Barra de cima")]
    [SerializeField] private TMP_Text textoPila;
    [SerializeField] private TMP_Text textoVida;
    [SerializeField] private Image barraVida;
    [SerializeField] private TMP_Text textoOnda;
    [SerializeField] private TMP_Text textoTempo;
    [SerializeField] private TMP_Text legendaTempo;
    [SerializeField] private TMP_Text textoPontuacao;

    [Header("Lista de torres")]
    [SerializeField] private CartaoTorre prefabCartao;
    [SerializeField] private Transform listaTorres; // tem um "Horizontal Layout Group" que arruma os cards. Absurdo isso.

    [Header("Botão de chamar a onda")]
    [SerializeField] private GameObject botaoProximaOnda;
    [SerializeField] private TMP_Text textoBotaoOnda;

    [Header("Botão de velocidade")]
    [SerializeField] private TMP_Text textoVelocidade;

    [Header("Aviso no meio da tela")]
    [SerializeField] private TMP_Text textoAviso;
    [SerializeField] private float duracaoAviso = 2f;

    [Header("Cores")]
    [SerializeField] private Color corTexto = Color.white;
    [SerializeField] private Color corAlerta = new Color(0.95f, 0.4f, 0.35f);
    [SerializeField] private Color corVidaCheia = new Color(0.92f, 0.92f, 0.92f);
    [SerializeField] private Color corVidaBaixa = new Color(0.85f, 0.25f, 0.2f);
    [SerializeField] private float tempoAlerta = 20f; 

    private void OnEnable()
    {
        geradorDeOndas.OndaComecou += AvisarOnda;
        loja.SemPila += PiscarPila;
    }

    private void OnDisable()
    {
        geradorDeOndas.OndaComecou -= AvisarOnda;
        loja.SemPila -= PiscarPila;
    }

    private void Start()
    {
        textoAviso.gameObject.SetActive(false);

        // Cria um card pra cada torre da loja
        for (int i = 0; i < loja.QuantidadeOpcoes; i++)
        {
            CartaoTorre cartao = Instantiate(prefabCartao, listaTorres);
            cartao.Configurar(loja, i);
        }

        // Na sobrevivência o relógio é o tempo  até os reforços chegarem
        if (gerenciador.Sobrevivencia)
        {
            legendaTempo.text = "REFORÇOS";
            StartCoroutine(MostrarAviso("Resista até os reforços chegarem!"));
        }
    }

    private void Update()
    {
        textoPila.text = $"{jogador.pila}";
        textoPontuacao.text = $"{gerenciador.PontuacaoCampanha}";
        
	if (gerenciador.Sobrevivencia) textoOnda.text = $"{geradorDeOndas.OndaAtual}";
        else textoOnda.text = $"{geradorDeOndas.OndaAtual}/{geradorDeOndas.TotalOndas}";

        botaoProximaOnda.SetActive(geradorDeOndas.EsperandoJogador);
        textoBotaoOnda.text = geradorDeOndas.OndaAtual == 0 ? "Começar" : "Próxima onda";

        textoVelocidade.text = $"{gerenciador.Velocidade}x";

        float fracaoVida = (float)forte.Health / forte.MaxHealth;
        barraVida.fillAmount = fracaoVida;
        barraVida.color = Color.Lerp(corVidaBaixa, corVidaCheia, fracaoVida);
        textoVida.text = $"{forte.Health}/{forte.MaxHealth}";

        // Converte os segundos restantes para o formato minutos:segundos
        int segundos = Mathf.CeilToInt(gerenciador.TempoRestante);
        int minutos = segundos / 60;
        int resto = segundos % 60;
        textoTempo.text = $"{minutos}:{resto:00}";	
	
	bool alertar = !gerenciador.Sobrevivencia && gerenciador.TempoRestante <= tempoAlerta;
        textoTempo.color = alertar ? corAlerta : corTexto;
    }

    private void AvisarOnda(int numero)
    {
        bool ultima = !gerenciador.Sobrevivencia && numero == geradorDeOndas.TotalOndas;
        string texto = ultima ? "Última onda!" : $"Onda {numero}";
        StartCoroutine(MostrarAviso(texto));
    }

    private IEnumerator MostrarAviso(string texto)
    {
        textoAviso.text = texto;
        textoAviso.gameObject.SetActive(true);
        yield return new WaitForSeconds(duracaoAviso);
        textoAviso.gameObject.SetActive(false);
    }

    private void PiscarPila()
    {
        StartCoroutine(PiscarTexto(textoPila));
    }

    // Deixa o texto vermelho num pisque
    private IEnumerator PiscarTexto(TMP_Text texto)
    {
        texto.color = corAlerta;
        yield return new WaitForSeconds(0.4f);
        texto.color = corTexto;
    }
}
