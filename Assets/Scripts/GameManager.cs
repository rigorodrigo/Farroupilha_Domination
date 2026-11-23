using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Controla tudo no jogo.
public class GameManager : MonoBehaviour
{
    [SerializeField] private string nomeDaFase = "";
    [SerializeField] private float tempoLimite = 120f; // Em segundos por favor vamos lembrar
    [SerializeField] private bool sobrevivencia = false; // Timer vira um contador de tempo sobrevivente
    [SerializeField] private PlayerBase forte;
    [SerializeField] private WaveSpawner geradorDeOndas;

    [Header("Tela de fim de jogo")]
    [SerializeField] private GameObject painelFim;
    [SerializeField] private TMP_Text textoFim;
    [SerializeField] private GameObject botaoProximaFase;

    [Header("Velocidade do jogo")]
    [SerializeField] private float[] velocidades = { 1f, 2f, 4f };

    [Header("Diálogos da fase")]
    [SerializeField] private CaixaDeDialogo caixaDeDialogo;
    [SerializeField] private List<Fala> falasDoInicio;
    [SerializeField] private List<Fala> falasDoFim;

    public float TempoRestante { get; private set; }
    public float Velocidade => velocidades[indiceVelocidade];
    public bool Sobrevivencia => sobrevivencia;
    public string NomeDaFase => nomeDaFase;

    // static: sobrevive à troca de cena, então soma a campanha inteira
    private static int pontuacao = 0;
    private static int pontuacaoInicioFase = 0; 

    public int PontuacaoCampanha => pontuacao;
    
    public static void NovaCampanha()
    {
        pontuacao = 0;
        pontuacaoInicioFase = 0;
    }

    private bool acabou;
    private bool pontuacaoRegistrada; 
    private int indiceVelocidade;

    private void Awake()
    {
        Time.timeScale = 1f; // garante que o jogo volta a andar depois de resetar
        TempoRestante = tempoLimite;
        pontuacao = pontuacaoInicioFase; // se reiniciou a fase, descarta os pontos da tentativa anterior
        painelFim.SetActive(false);
        botaoProximaFase.SetActive(false);

        // Na sobrevivência as ondas não acabam daí, pra descomplicar, depois da última, volta pra primeira
        if (sobrevivencia) geradorDeOndas.RepetirOndas = true;
    }

    private void Start()
    {
        if (caixaDeDialogo != null) caixaDeDialogo.Mostrar(falasDoInicio);
    }

    private void OnEnable()
    {
        forte.Destroyed += ForteCaiu;
        geradorDeOndas.FaseConcluida += Vencer;
        Enemy.Died += InimigoMorreu;
    }

    private void OnDisable()
    {
        forte.Destroyed -= ForteCaiu;
        geradorDeOndas.FaseConcluida -= Vencer;
        Enemy.Died -= InimigoMorreu;
    }

    private void InimigoMorreu(Enemy inimigo)
    {
        pontuacao++;
    }

    private void Update()
    {
        if (acabou) return;
        if (geradorDeOndas.EsperandoJogador) return; // relógio parado enquanto ondas tão paradas

        TempoRestante -= Time.deltaTime;
        if (TempoRestante <= 0f)
        {
            TempoRestante = 0f;
            if (sobrevivencia) Vencer(); // resistiu até o fim
            else Perder("O tempo acabou!");
        }
    }

    private void ForteCaiu()
    {
        Perder("O forte caiu!");
    }

    private void Vencer()
    {
        if (acabou) return;

        acabou = true;
        StartCoroutine(VencerDepoisDasFalas());
    }

    private IEnumerator VencerDepoisDasFalas()
    {
        if (caixaDeDialogo != null) yield return caixaDeDialogo.Mostrar(falasDoFim);

	// Cada fase é uma cena
        bool temProximaFase = SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;
        botaoProximaFase.SetActive(temProximaFase);
        
        if (sobrevivencia) TerminarPartida("Vitória! Os reforços chegaram.", !temProximaFase);
        else if (!temProximaFase) TerminarPartida("Vitória! A República Rio-Grandense chegou à Corte.", true);
        else TerminarPartida("Vitória! O forte resistiu.", false);
    }

    private void Perder(string motivo)
    {
        if (acabou) return;
        TerminarPartida($"Derrota! {motivo}", true);
    }

    private void TerminarPartida(string mensagem, bool fimDeJogo)
    {
        acabou = true;
        Time.timeScale = 0f; // pausa o jogo
        textoFim.text = mensagem;
        painelFim.SetActive(true);

        if (fimDeJogo) RegistrarNoRanking();
    }

    private void RegistrarNoRanking()
    {
        if (pontuacaoRegistrada) return;

        pontuacaoRegistrada = true;
        MenuPrincipal.RegistrarPontuacao(PontuacaoCampanha);
    }

    public void TrocarVelocidade()
    {
        if (acabou) return;

        indiceVelocidade++;
        if (indiceVelocidade >= velocidades.Length) indiceVelocidade = 0; // volta pra primeira
        Time.timeScale = Velocidade; // o relógio da missão também tinha que acelerar
    }

    // Chamados pelos botões da tela de fim de jogo
    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ProximaFase()
    {
        pontuacaoInicioFase = pontuacao; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void VoltarAoMenu()
    {
        RegistrarNoRanking(); 
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }
}
