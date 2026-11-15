using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TowerVisual : MonoBehaviour
{
    [Header("Poses")]
    [SerializeField] private Sprite parado;
    [SerializeField] private Sprite preparando;
    [SerializeField] private Sprite disparando;
    [SerializeField] private Sprite recuperando;

    [Header("Duração de cada pose (segundos)")]
    [SerializeField] private float tempoPreparando = 0.3f;
    [SerializeField] private float tempoDisparando = 0.15f;
    [SerializeField] private float tempoRecuperando = 0.25f;

    private SpriteRenderer sr;

    public bool Ocupado { get; private set; }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = parado;
    }

    public void Atacar(Vector2 alvo, Action aoDisparar)
    {
        if (Ocupado) return;
        StartCoroutine(SequenciaDeAtaque(alvo, aoDisparar));
    }

    private IEnumerator SequenciaDeAtaque(Vector2 alvo, Action aoDisparar)
    {
        Ocupado = true;
        sr.flipX = alvo.x < transform.position.x;

        sr.sprite = preparando;
        yield return new WaitForSeconds(tempoPreparando);

        sr.sprite = disparando;
        aoDisparar?.Invoke();
        yield return new WaitForSeconds(tempoDisparando);

        sr.sprite = recuperando;
        yield return new WaitForSeconds(tempoRecuperando);

        sr.sprite = parado;
        Ocupado = false;
    }
}
