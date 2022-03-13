using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

// #if UNITY_ANDROID && !UNITY_EDITOR
//     public static AndroidJavaClass unityplayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
//     public static AndroidJavaObject currentActivity = unityplayer.GetStatic<AndroidJavaObject>("currentActivity");
//     public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "Vibrator");
// #else
//     public static AndroidJavaClass unityplayer;
//     public static AndroidJavaObject currentActivity;
//     public static AndroidJavaObject vibrator;
// #endif

    public Manager GameManager;
    public AudioClip enemyHit, ballHit, charge, powerUp;
    AudioSource enemyHitSource, ballHitSource, chargeSource, powerUpSource;

    void Start() {
        //Subscribe to events
        GameEvents.current.OnEnemyHit += PlayEnemyHit;
        GameEvents.current.OnBallHit += PlayBallHit;
        GameEvents.current.OnChargeStart += PlayCharge;
        GameEvents.current.OnChargeStop += StopCharge;
        GameEvents.current.OnPowerUp += PlayPowerUp;

        AudioSource[] sources = gameObject.GetComponents<AudioSource>();
        enemyHitSource = sources[0];
        ballHitSource = sources[1];
        chargeSource = sources[2];
        powerUpSource = sources[3];
    }

    public void PlayEnemyHit() {
        if (GameManager.SoundEnabled < 0) return;
        enemyHitSource.PlayOneShot(enemyHit);
        if (GameManager.HapticsEnabled < 0) return;
        Vibrations.Vibrate(250);
    }
    public void PlayBallHit() {
        if (GameManager.SoundEnabled < 0) return;
        ballHitSource.PlayOneShot(ballHit);
    }
    public void PlayCharge() {
        if (GameManager.SoundEnabled < 0) return;
        chargeSource.clip = charge;
        if (!chargeSource.isPlaying) chargeSource.Play();
    }
    public void StopCharge() {
        if (GameManager.SoundEnabled < 0) return;
        chargeSource.Stop();
    }
    public void PlayPowerUp() {
        if (GameManager.SoundEnabled < 0) return;
        powerUpSource.PlayOneShot(powerUp);
    }

//     bool IsAndroid() {
// #if UNITY_ANDROID && !UNITY_EDITOR
//          return true;
// #else
//          return false;
// #endif
//     }

//     void Vibrate(long milliseconds) {
//         if (IsAndroid()) {
//             vibrator.Call("vibrate", milliseconds);
//         } else {
//             Handheld.Vibrate();
//         }
//     }

//     void Cancel() {
//         if (IsAndroid()) {
//             vibrator.Call("cancel");
//         }
//     }
}
