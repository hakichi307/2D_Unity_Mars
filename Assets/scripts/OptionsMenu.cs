using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class OptionsMenu : MonoBehaviour
{
    public GameObject optionPanel;  // Panel để điều chỉnh âm thanh và nhạc
    public Button optionButton;     // Nút "Option"
    public Button resumeButton;     // Nút "Resume"
    public Button backToMenuButton; // Nút "Back to Menu"
    public Slider volumeSlider;     // Slider cho âm lượng
    public Slider musicSlider;      // Slider cho nhạc nền
    public AudioSource musicSource; // Nguồn nhạc nền
    private float initialVolume;
    // Start is called before the first frame update
    
    
    void Start()
    {
        optionPanel.SetActive(false);

        optionButton.onClick.AddListener(OpenOptionPanel);
        resumeButton.onClick.AddListener(CloseOptionPanel);
        backToMenuButton.onClick.AddListener(BackToMenu);
        // Gán sự kiện cho các slider
        volumeSlider.onValueChanged.AddListener(AdjustVolume);
        musicSlider.onValueChanged.AddListener(AdjustMusic);
        // Lưu trữ âm lượng ban đầu
        initialVolume = AudioListener.volume;
        volumeSlider.value = initialVolume;
        musicSlider.value = musicSource.volume;

    }

    // Hiển thị Option Panel
    void OpenOptionPanel()
    {
        optionPanel.SetActive(true);
        Time.timeScale = 0; // Dừng game khi mở Option
    }

    // Đóng Option Panel và tiếp tục game
    void CloseOptionPanel()
    {
        optionPanel.SetActive(false);
        Time.timeScale = 1; // Tiếp tục game
    }

    // Điều chỉnh âm lượng tổng thể
    void AdjustVolume(float volume)
    {
        AudioListener.volume = volume; // Điều chỉnh âm lượng toàn game
    }

    // Điều chỉnh âm lượng nhạc nền
    void AdjustMusic(float musicVolume)
    {
        musicSource.volume = musicVolume; // Điều chỉnh âm lượng nhạc nền
    }

    // Quay trở lại menu chính
    void BackToMenu()
    {
        Time.timeScale = 1; // Đảm bảo game không bị dừng khi quay về menu
        SceneManager.LoadScene("Menu"); // Thay đổi scene thành menu chính
    }
}
