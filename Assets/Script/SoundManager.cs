using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public bool isMuted = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ SoundManager khi chuyển cảnh
            LoadMuteState(); // Tải trạng thái âm thanh từ PlayerPrefs
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Chỉ giữ lại một SoundManager duy nhất
        }
    }

    private void LoadMuteState()
    {
        // Lấy trạng thái tắt âm thanh đã lưu
        isMuted = PlayerPrefs.GetInt("isMuted", 0) == 1;
    }

    public void SetMute(bool mute)
    {
        isMuted = mute;
        PlayerPrefs.SetInt("isMuted", mute ? 1 : 0); // Lưu trạng thái tắt âm thanh
        PlayerPrefs.Save();
    }

    public void PlayClip(AudioClip clip, Vector3 position)
    {
        if (!isMuted)
        {
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }

    public void PlayRandomClip(AudioClip[] clips, Vector3 position)
    {
        if (!isMuted)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioSource.PlayClipAtPoint(clips[randomIndex], position);
        }
    }
}
