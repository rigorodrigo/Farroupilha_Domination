using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Mostra uma conversa na parte de baixo da tela. O jogo fica pausado enquanto ela está aberta.
// Clique, Espaço ou Enter passa pra próxima fala
public class CaixaDeDialogo : MonoBehaviour
{
    public static bool Aberta { get; private set; } // as torres não podem ser compradas com a conversa aberta

    [SerializeField] private GameObject painel;
    [SerializeField] private Image retrato;
    [SerializeField] private TMP_Text textoNome;
    [SerializeField] private TMP_Text textoFala;
    [SerializeField] private float letrasPorSegundo = 40f;

    [Header("Margem do texto (com e sem retrato)")]
    [SerializeField] private float margemComRetrato = 270f;
    [SerializeField] private float margemSemRetrato = 40f;

    private bool pular;

    private void Awake()
    {
        painel.SetActive(false);
        Aberta = false; // static sobrevive à troca de cena, então zera aqui
    }

    // Devolve a Coroutine pra quem quiser esperar a conversa acabar (yield return)
    public Coroutine Mostrar(List<Fala> falas)
    {
        return StartCoroutine(MostrarFalas(falas));
    }

    // Chamado pelo botão "Pular"
    public void Pular()
    {
        pular = true;
    }

    private IEnumerator MostrarFalas(List<Fala> falas)
    {
        if (falas == null || falas.Count == 0) yield break;

        while (Aberta) yield return null; // se já tem uma conversa na tela, espera ela acabar

        Aberta = true;
        pular = false;
        float velocidadeAntes = Time.timeScale;
        Time.timeScale = 0f; // pausa o jogo
        painel.SetActive(true);

        foreach (Fala fala in falas)
        {
            if (pular) break;
            MostrarPersonagem(fala.personagem);

            // Efeito de máquina de escrever. Usa unscaledDeltaTime porque o jogo está pausado
            textoFala.text = fala.texto;
            float letras = 0f;
            textoFala.maxVisibleCharacters = 0;
            while (letras < fala.texto.Length)
            {
                if (pular || Avancou()) break; // clicar no meio mostra a fala inteira
                letras += letrasPorSegundo * Time.unscaledDeltaTime;
                textoFala.maxVisibleCharacters = (int)letras;
                yield return null;
            }
            textoFala.maxVisibleCharacters = fala.texto.Length;

            yield return null; // espera um frame pro mesmo clique não pular duas falas
            while (!pular && !Avancou()) yield return null;
            yield return null;
        }

        painel.SetActive(false);
        Time.timeScale = velocidadeAntes;
        Aberta = false;
    }

    private bool Avancou()
    {
        return Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return);
    }

    private void MostrarPersonagem(Personagem personagem)
    {
        bool narrador = personagem == null;
        bool temRetrato = !narrador && personagem.retrato != null;

        textoNome.text = narrador ? "" : personagem.nome;
        textoFala.fontStyle = narrador ? FontStyles.Italic : FontStyles.Normal;

        retrato.gameObject.SetActive(temRetrato);
        if (temRetrato) retrato.sprite = personagem.retrato;

        // Sem retrato o texto ocupa o espaço dele
        float margem = temRetrato ? margemComRetrato : margemSemRetrato;
        textoNome.rectTransform.offsetMin = new Vector2(margem, textoNome.rectTransform.offsetMin.y);
        textoFala.rectTransform.offsetMin = new Vector2(margem, textoFala.rectTransform.offsetMin.y);
    }
}
