using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public AudioSource buttonAudioSource;  // Audio source chứa âm thanh click
    public AudioClip clickSound;           // Âm thanh click
    public float delayBeforeAction = 0.3f; // Thời gian chờ trước khi thực hiện hành động

    // Start is called before the first frame update
    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    private IEnumerator PlayGameWithSound()
    {
        PlayClickSound();
        yield return new WaitForSeconds(delayBeforeAction);  // Chờ âm thanh phát xong
        SceneManager.LoadScene(1);  // Tải scene game sau khi âm thanh phát xong
    }

    // Coroutine để chơi âm thanh click rồi sau đó thoát game
    private IEnumerator ExitGameWithSound()
    {
        PlayClickSound();
        yield return new WaitForSeconds(delayBeforeAction);  // Chờ âm thanh phát xong
        Application.Quit();  // Thoát game sau khi âm thanh phát xong
    }

    // Hàm phát âm thanh click
    private void PlayClickSound()
    {
        if (buttonAudioSource != null && clickSound != null)
        {
            buttonAudioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.LogWarning("Missing AudioSource or click sound!");
        }
    }
}
