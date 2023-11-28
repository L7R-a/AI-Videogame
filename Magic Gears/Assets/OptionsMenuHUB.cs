using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenuHUB : MonoBehaviour
{
    public AudioMixer AudioMixer;
    public static bool IsPaused = false;
    public GameObject Menu;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(IsPaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }
    public void SetVolume(float volume) {
        AudioMixer.SetFloat("Volume", volume);
    }

    public void Resume() {
        Cursor.lockState = CursorLockMode.Locked;
        Menu.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
    }
    public void Pause() {
        Cursor.lockState = CursorLockMode.None;
        Menu.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void QuitGame() {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
