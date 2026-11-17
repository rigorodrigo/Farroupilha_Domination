using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuPrincipal : MonoBehaviour
{
    public static int dificuldade = 0; // 0 = Fácil, 1 = Difícil

    [SerializeField] private string cenaDoJogo = "Main";
    [SerializeField] private string[] nomesDificuldade = { "Fácil", "Difícil" };

    [Header("Painéis")]
    [SerializeField] private GameObject painelPrincipal;
    [SerializeField] private GameObject painelRanking;
    [SerializeField] private GameObject painelSobre;

    [Header("Textos")]
    [SerializeField] private TMP_Text textoDificuldade;
    [SerializeField] private TMP_Text textoRanking;

    private void Start()
    {
        MostrarPainel(painelPrincipal);
        AtualizarTextoDificuldade();
    }


    public void Jogar()
    {
        SceneManager.LoadScene(cenaDoJogo);
    }

    public void TrocarDificuldade()
    {
        dificuldade++;
        if (dificuldade >= nomesDificuldade.Length) dificuldade = 0; // volta pra primeira
        AtualizarTextoDificuldade();
    }

    public void AbrirRanking()
    {
        textoRanking.text = MontarRanking();
        MostrarPainel(painelRanking);
    }

    public void AbrirSobre()
    {
        MostrarPainel(painelSobre);
    }

    public void Voltar()
    {
        MostrarPainel(painelPrincipal);
    }

    public void Sair()
    {
        Application.Quit(); // Confia, isso daqui funciona quando o jogo tiver buildado
    }

    // Deixa só um painel ligado por vez
    private void MostrarPainel(GameObject painel)
    {
        painelPrincipal.SetActive(painel == painelPrincipal);
        painelRanking.SetActive(painel == painelRanking);
        painelSobre.SetActive(painel == painelSobre);
    }

    private void AtualizarTextoDificuldade()
    {
        textoDificuldade.text = $"Dificuldade: {nomesDificuldade[dificuldade]}";
    }

    // Lê as 5 melhores pontuações salvas nas PlayerPrefs (Provavlemte tá bugado).
    private string MontarRanking()
    {
        string texto = "";
        for (int i = 0; i < 5; i++)
        {
            int pontos = PlayerPrefs.GetInt($"Ranking{i}", 0);
            if (pontos > 0) texto += $"{i + 1}º   {pontos} pontos\n";
        }

        if (texto == "") texto = "Nenhuma partida registrada ainda.";
        return texto;
    }
}
