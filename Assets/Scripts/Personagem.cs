using UnityEngine;

[CreateAssetMenu(fileName = "Personagem", menuName = "Farroupilha/Personagem")]
public class Personagem : ScriptableObject
{
    public string nome;
    public Sprite retrato;
}

[System.Serializable]
public class Fala
{
    public Personagem personagem;
    [TextArea(2, 4)] public string texto;
}
