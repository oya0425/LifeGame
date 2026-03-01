using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource seSource;
    public AudioSource bgmSource;
    public SoundDatabase db; // ここにDatabaseアセットをセット

    void Awake() { 
    }

    public void PlaySE(string name)
    {
        Sounddata data = db.GetSound(name);
        if (data != null)
        {
            seSource.PlayOneShot(data.clip, data.volume);
        }
    }

    //public void PlaySpecialSE(string name, float pitch)
    //{
    //    Sounddata data = db.GetSound(name);
    //    // 一時的にピッチを1オクターブ上げる
    //    //seSource.pitch = pitch;

    //    //if (data != null)
    //    //{
    //    //    seSource.PlayOneShot(data.clip, data.volume);
    //    //}
    //    if (data != null)
    //    {
    //        // コルーチンを呼び出して、ピッチの管理を任せる
    //        StartCoroutine(PlaySpecialCoroutine(data, pitch));
    //    }
    //    // すぐに元のピッチ（1.0f）に戻しておく
    //    // PlayOneShotはピッチ設定を引き継いで再生されます
    //    //seSource.pitch = 1.0f;
    //}
    //private IEnumerator PlaySpecialCoroutine(Sounddata data, float pitch)
    //{
    //    // ピッチを変更
    //    seSource.pitch = pitch;

    //    // 再生
    //    seSource.PlayOneShot(data.clip, data.volume);

    //    // 音の長さ（秒）だけ待機する
    //    // data.clip.length で音声ファイルの秒数が取れます
    //    // ※ピッチを上げると再生時間は短くなるので pitch で割るのが正確です
    //    yield return new WaitForSeconds(data.clip.length / pitch);

    //    // 鳴り終わったらピッチを戻す
    //    seSource.pitch = 1.0f;
    //}

    public float PlaySpecialSE(string name, float pitch)
    {
        Sounddata data = db.GetSound(name);
        if (data != null)
        {
            seSource.pitch = pitch;
            seSource.PlayOneShot(data.clip, data.volume);

            // 元に戻す処理を予約（少し後に実行されるようにする）
            Invoke(nameof(ResetPitch), data.clip.length / pitch);

            // 再生にかかる時間を計算して返す
            return data.clip.length / pitch;
        }
        return 0f;
    }
    // AudioManager.cs 内
    public void PlaySERanking(string name, float pitch)
    {
        Sounddata data = db.GetSound(name);
        if (data != null)
        {
            seSource.pitch = pitch;
            seSource.PlayOneShot(data.clip, data.volume);

            // 鳴らし終わる頃にピッチを1に戻す（他のSEに影響しないように）
            Invoke(nameof(ResetPitch), data.clip.length / pitch);
        }
    }

    private void ResetPitch()
    {
        seSource.pitch = 1.0f;
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