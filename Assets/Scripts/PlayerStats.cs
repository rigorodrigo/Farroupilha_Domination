using UnityEngine;


public class PlayerStats : MonoBehaviour
{

    public int pila;
    [SerializeField] private int pilaInicial = 50;  // o player começa com um valor
    [SerializeField] private int pilaPorInimigo = 15; // quanto ganha por inimigo derrotado

    private void OnEnable() {Enemy.Died += EnemyDied; }
    private void OnDisable() {Enemy.Died -= EnemyDied; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pila = pilaInicial;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void GanharPila()
    {
        pila += pilaPorInimigo;
        Debug.Log(pila);
    }

    public bool GastarPila (int valor)
    {
        if (pila < valor) return false;  // Tá pobre fi

        pila -= valor;
        
        Debug.Log($"Compra realizada com sucesso! Compra de {valor} pila, total restante: {pila} pila");

        return true;
    }


    void EnemyDied(Enemy enemy)
    {
        GanharPila();
    }
}
