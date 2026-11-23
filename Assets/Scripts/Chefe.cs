using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Enemy))]
public class Chefe : MonoBehaviour
{
    public static event Action<Chefe> Apareceu;
    public static event Action<Chefe> SegundaFaseComecou;

    [SerializeField] private string nome = "Chefe";

    [Header("Diálogos (ficam no prefab, valem em qualquer fase)")]
    [SerializeField] private List<Fala> falasAoAparecer;
    [SerializeField] private List<Fala> falasAoCair;

    [Header("Segunda fase")]
    [SerializeField] private bool temSegundaFase = false;
    [SerializeField] private float vidaSegundaFase = 0.5f; // 0.5 = metade da vida
    [SerializeField] private float multiplicadorVelocidade = 1.5f;
    [SerializeField] private Color corSegundaFase = new Color(1f, 0.6f, 0.6f);
    [SerializeField] private Enemy reforco;
    [SerializeField] private int quantidadeReforco = 6;

    public string Nome => nome;
    public float VidaPercentual => enemy.VidaPercentual;

    private Enemy enemy;
    private bool segundaFase;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void OnEnable() { Enemy.Died += InimigoMorreu; }
    private void OnDisable() { Enemy.Died -= InimigoMorreu; }

    private void Start()
    {
        Apareceu?.Invoke(this);
        MostrarFalas(falasAoAparecer);
    }

    private void InimigoMorreu(Enemy inimigo)
    {
        if (inimigo == enemy) MostrarFalas(falasAoCair);
    }

    private void MostrarFalas(List<Fala> falas)
    {
        CaixaDeDialogo caixa = FindFirstObjectByType<CaixaDeDialogo>();
        if (caixa != null) caixa.Mostrar(falas);
    }

    private void Update()
    {
        if (!temSegundaFase || segundaFase || enemy.Morto) return;
        if (enemy.VidaPercentual > vidaSegundaFase) return;

        segundaFase = true;
        enemy.MultiplicarVelocidade(multiplicadorVelocidade);
        GetComponent<SpriteRenderer>().color = corSegundaFase;

        if (reforco != null)
            FindFirstObjectByType<WaveSpawner>().SpawnarReforco(reforco, quantidadeReforco, enemy.path);

        SegundaFaseComecou?.Invoke(this);
    }
}
