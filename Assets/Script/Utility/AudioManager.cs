using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource seSource;
    public AudioSource bgmSource;
    public SoundDatabase db; // ここにDatabaseアセットをセット

    void Awake() { if (instance == null) instance = this; }

    public void PlaySE(string name)
    {
        Sounddata data = db.GetSound(name);
        if (data != null)
        {
            seSource.PlayOneShot(data.clip, data.volume);
        }
    }
    public void PlayBGM(string name)
    {
        Sounddata data = db.GetBGM(name);
        if (data != null && data.clip != null)
        {
            // 既に同じ曲が流れている場合は何もしない
            if (bgmSource.clip == data.clip && bgmSource.isPlaying) return;

            bgmSource.clip = data.clip;
            bgmSource.volume = data.volume;
            bgmSource.loop = data.loop;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}