using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "LifeGame/Sounddata")]
public class Sounddata : ScriptableObject
{
    public string soundName; // Database“à‚ÅŒŸõ‚·‚é‚½‚ß‚Ì–¼‘O
    public AudioClip clip;
    [Range(0, 1)] public float volume = 1.0f;
    public bool loop = true;
}