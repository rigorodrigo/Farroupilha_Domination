using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[System.Serializable]
public class EnemyGroup {
    public Enemy prefab;
    public int quantidade;
    public float intervalo;
    public WaypointPath caminho; 
}

[System.Serializable]
public class Wave
{
    public float pausa;
    public List<EnemyGroup> grupos;
    public bool ondaDificil;
    public List<Fala> falasAntes; // Diálogo depois de clicar em "Próxima onda"
}

public class WaveSpawner : MonoBehaviour
{

    [SerializeField] private List<Wave> ondas;
    [SerializeField] private int inimigosVivos;
    [SerializeField] private bool esperarJogador = true; 

    [SerializeField] private float multiplicadorVidaDificil = 1.5f;
    [SerializeField] private CaixaDeDialogo caixaDeDialogo;

    public event Action FaseConcluida; // Avisa o GameManager que todas as ondas acabaram
    public event Action<int> OndaComecou; // Avisa o HUD o número da onda que começou

    // Só leitura pro HUD mostrar em qual onda estamos
    public int OndaAtual { get; private set; }
    public int TotalOndas {get; private set;}
    public bool EsperandoJogador { get; private set; } 
    public bool RepetirOndas { get; set; }

    private bool liberarOnda;
    private int reforcosPendentes;

    // Chamado pelo botão "Próxima onda" do HUD
    public void ChamarProximaOnda()
    {
        liberarOnda = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ExecutarFase());
    }

    private void OnEnable()
    {
        Enemy.Died += InimigoMorreu;
    }

    private void OnDisable()
    {
        Enemy.Died -= InimigoMorreu;
    }

    private void InimigoMorreu(Enemy inimigo)
    {
        inimigosVivos --;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnarInimigo (EnemyGroup grupo)
    {
        Vector3 posicao = grupo.caminho.waypoints[0].position;
        Enemy novoInimigo = Instantiate(grupo.prefab,posicao, Quaternion.identity); 
        novoInimigo.path = grupo.caminho;
        if (MenuPrincipal.dificuldade == 1) novoInimigo.MultiplicarVida(multiplicadorVidaDificil);
        inimigosVivos++;
    }

    // Chamado pelo Boss
    public void SpawnarReforco(Enemy prefab, int quantidade, WaypointPath caminho)
    {
        EnemyGroup reforco = new EnemyGroup { prefab = prefab, quantidade = quantidade, intervalo = 0.5f, caminho = caminho };
        reforcosPendentes += quantidade;
        StartCoroutine(SpawnarReforcoAosPoucos(reforco));
    }

    private IEnumerator SpawnarReforcoAosPoucos(EnemyGroup grupo)
    {
        for (int i = 0; i < grupo.quantidade; i++)
        {
            SpawnarInimigo(grupo);
            reforcosPendentes--;
            yield return new WaitForSeconds(grupo.intervalo);
        }
    }

    private IEnumerator SpawnarGrupo (EnemyGroup grupo)
    {
        for (int i = 0; i < grupo.quantidade; i ++)
        {
            SpawnarInimigo(grupo);
            yield return new WaitForSeconds(grupo.intervalo);
        }
    }

    private IEnumerator SpawnarOnda (Wave onda)
    {
        yield return new WaitForSeconds(onda.pausa);

        foreach (EnemyGroup grupo in onda.grupos) 
 	{
            yield return StartCoroutine(SpawnarGrupo(grupo));
        }
        yield return new WaitUntil(() => inimigosVivos == 0 && reforcosPendentes == 0);
    }

    private IEnumerator ExecutarFase ()
    {
        if (MenuPrincipal.dificuldade == 1) TotalOndas = ondas.Count;  // se a fase for dificil, pega todas as ondas
        else // se for fácil, pega apenas aquelas que estão marcadas para serem nesta fase
        {
            TotalOndas = ondas.Count(o => o.ondaDificil == false);   
        }
        bool primeiraVolta = true; // na sobrevivência as ondas repetem, mas as falas não
        do
        {

            for (int i = 0; i< ondas.Count; i++)
            {
                if (ondas[i].ondaDificil && MenuPrincipal.dificuldade == 0) continue;  // se a onda é apenas para fase difícil, e a dificuldade está setada como fácil, pula a onda
                // Espera o jogador clicar no botão antes de mandar a onda
                if (esperarJogador)
                {
                    liberarOnda = false;
                    EsperandoJogador = true;
                    yield return new WaitUntil(() => liberarOnda);
                    EsperandoJogador = false;
                }

                if (primeiraVolta && caixaDeDialogo != null) yield return caixaDeDialogo.Mostrar(ondas[i].falasAntes);

                OndaAtual++;
                OndaComecou?.Invoke(OndaAtual);
                yield return StartCoroutine(SpawnarOnda(ondas[i]));
                Debug.Log($"Onda número {i}");
            }
            primeiraVolta = false;
        } while (RepetirOndas);

        Debug.Log("Fase Acabou.");
        FaseConcluida?.Invoke();
    }
}
