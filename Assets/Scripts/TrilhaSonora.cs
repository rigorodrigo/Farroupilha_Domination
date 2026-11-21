using UnityEngine;

public class TrilhaSonora : MonoBehaviour
{
    [SerializeField] private AudioClip [] musicas;
    [SerializeField] private bool ordemAleatoria = true;
    private AudioSource audioSource;
    private int indice = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (musicas.Length == 0) return; // se não houver músicas no array, não toca nada
        if(ordemAleatoria)
        {
           indice = Random.Range(0, musicas.Length);
        }
        TocarMusica();
    }

    // Update is called once per frame
    void Update()
    {
        if (musicas.Length == 0) return;
        if (!audioSource.isPlaying)
        {
            indice ++;
            if (indice >= musicas.Length) indice = 0; // se o index passar o número de músicas, volta ao início
            TocarMusica();
        }
    }

    void TocarMusica()
    {
        audioSource.clip = musicas[indice];
        audioSource.Play();
    }
}
