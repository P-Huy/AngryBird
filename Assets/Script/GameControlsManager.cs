using UnityEngine;
using UnityEngine.UI;

public class GameControlsManager : MonoBehaviour
{
    [Header("Nút điều khiển")]
    [SerializeField] private Button tatAmThanhButton; // Nút Tắt âm thanh
    [SerializeField] private Button batAmThanhButton; // Nút Bật âm thanh
    [SerializeField] private Button dungButton; // Nút Dừng
    [SerializeField] private Button tiepTucButton; // Nút Tiếp tục

    [Header("Âm thanh")]
    [SerializeField] private AudioSource[] gameAudioSources; // Danh sách các nguồn âm thanh của game

    private bool isGamePaused = false;

    private void Start()
    {
        // Apply mute state to all audio sources when the game starts
        UpdateAudioSourcesMuteState();

        // Add button listeners
        tatAmThanhButton.onClick.AddListener(TatAmThanh);
        batAmThanhButton.onClick.AddListener(BatAmThanh);
        dungButton.onClick.AddListener(DungGame);
        tiepTucButton.onClick.AddListener(TiepTucGame);
    }

    public void TatAmThanh()
    {
        Debug.Log("TatAmThanh được gọi");
        foreach (AudioSource audio in gameAudioSources)
        {
            if (audio != null) // Check if the audio source is still valid
            {
                audio.mute = true;
            }
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetMute(true); // Tắt âm thanh
        }
        else
        {
            Debug.LogWarning("SoundManager instance is not set.");
        }

        tatAmThanhButton.gameObject.SetActive(false);
        batAmThanhButton.gameObject.SetActive(true);
    }

    public void BatAmThanh()
    {
        Debug.Log("BatAmThanh được gọi");
        foreach (AudioSource audio in gameAudioSources)
        {
            if (audio != null) // Check if the audio source is still valid
            {
                audio.mute = false;
            }
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetMute(false); // Bật âm thanh
        }
        else
        {
            Debug.LogWarning("SoundManager instance is not set.");
        }

        batAmThanhButton.gameObject.SetActive(false);
       tatAmThanhButton.gameObject.SetActive(true);
    }


    public void DungGame()
    {
        Debug.Log("DungGame được gọi");
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            dungButton.gameObject.SetActive(false);
            tiepTucButton.gameObject.SetActive(true);
        }
    }

    public void TiepTucGame()
    {
        Debug.Log("TiepTucGame được gọi");
        if (isGamePaused)
        {
            Time.timeScale = 1;
            isGamePaused = false;
            tiepTucButton.gameObject.SetActive(false);
            dungButton.gameObject.SetActive(true);
        }
    }



    private void UpdateAudioSourcesMuteState()
    {
        bool isMuted = SoundManager.instance.isMuted; // Access the current mute state

        // Apply mute state to each audio source
        foreach (AudioSource audio in gameAudioSources)
        {
            if (audio != null)
            {
                audio.mute = isMuted;
            }
        }

        // Update button visibility based on mute state
        batAmThanhButton.gameObject.SetActive(isMuted);
        tatAmThanhButton.gameObject.SetActive(!isMuted);
    }


}
