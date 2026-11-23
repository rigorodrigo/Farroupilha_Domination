using System;
using UnityEngine;
using System.Collections.Generic;
[System.Serializable]

public class OpcaoTorre
{
    public string nome;
    public Tower prefab;
    public int custo;
    public Sprite icone; // imagem mostrada no card
}

public class LojaDeTorres : MonoBehaviour
{

    [SerializeField] private PlayerStats jogador;
    [SerializeField] private List<OpcaoTorre> opcoes;
    private int selecionada = 0;

    public event Action SemPila; // Sem money money

    // Só leitura pro HUD montar a lista de torres
    public int Selecionada => selecionada;
    public int QuantidadeOpcoes => opcoes.Count;
    public OpcaoTorre PegarOpcao(int indice) => opcoes[indice];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Tecla 1 = primeira torre, 2 = segunda, e assim por diante
        for (int i = 0; i < opcoes.Count && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) Selecionar(i);
        }
    }

    public Tower Comprar(Vector3 posicao)
    {
        OpcaoTorre opcao = opcoes[selecionada];

        if (!jogador.GastarPila(opcao.custo))
        {
            Debug.Log($"Você não tem pila para compra a Torre {opcao.nome}! Ela custa {opcao.custo}");
            SemPila?.Invoke();
            return null;
        }

        return Instantiate(opcao.prefab, posicao, Quaternion.identity);
    }

    public bool PodeComprarSelecionada()
    {
        return PodeComprar(selecionada);
    }

    public bool PodeComprar(int indice)
    {
        return jogador.pila >= opcoes[indice].custo;
    }

    public void Selecionar (int indice)
    {
        selecionada = indice;
        Debug.Log($"Selecionou a torre: {opcoes[indice].nome}");
    }

}
