using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Controla tudo no jogo.
public class GameManager : MonoBehaviour
{
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

    public float TempoRestante { get; private set; }
    public float Velocidade => velocidades[indiceVelocidade];
    public bool Sobrevivencia => sobrevivencia;

    private bool acabou;
    private int indiceVelocidade;

    private void Awake()
    {
        Time.timeScale = 1f; // garante que o jogo volta a andar depois de resetar
        TempoRestante = tempoLimite;
        painelFim.SetActive(false);
        botaoProximaFase.SetActive(false);

        // Na sobrevivência as ondas não acabam daí, pra descomplicar, depois da última, volta pra primeira
        if (sobrevivencia) geradorDeOndas.RepetirOndas = true;
    }

    private void OnEnable()
    {
        forte.Destroyed += ForteCaiu;
        geradorDeOndas.FaseConcluida += Vencer;
    }

    private void OnDisable()
    {
        forte.Destroyed -= ForteCaiu;
        geradorDeOndas.FaseConcluida -= Vencer;
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
	
	// Cada fase é uma cena
        bool temProximaFase = SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings;
        botaoProximaFase.SetActive(temProximaFase);

        if (sobrevivencia) TerminarPartida("Vitória! Os reforços chegaram.");
        else if (!temProximaFase) TerminarPartida("Vitória! Todas as fases vencidas.");
        else TerminarPartida("Vitória! O forte resistiu.");
    }

    private void Perder(string motivo)
    {
        TerminarPartida($"Derrota! {motivo}");
    }

    private void TerminarPartida(string mensagem)
    {
        if (acabou) return;

        acabou = true;
        Time.timeScale = 0f; // pausa o jogo
        textoFim.text = mensagem;
        painelFim.SetActive(true);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void VoltarAoMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Menu");
    }
}
