using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel;    // Panel pause menu
    public Button pauseButton;           // Tombol pause utama
    public Button resumeButton;          // Tombol resume di menu
    public Button settingsButton;        // Tombol settings di menu
    public Button exitButton;            // Tombol exit di menu

    [Header("Button Icons")]
    public Sprite pauseSprite;   // Icon || (game berjalan)
    public Sprite playSprite;    // Icon ▶ (game di-pause)

    private bool isPaused = false;

    private void Start()
    {
        // Pastikan panel tidak aktif saat mulai
        pauseMenuPanel.SetActive(false);

        // Hubungkan tombol lewat kode
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        pauseButton.image.sprite = playSprite;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
        pauseButton.image.sprite = pauseSprite;
        isPaused = false;
    }

    private void OpenSettings()
    {
        Debug.Log("Buka menu Settings...");
        // Tambahkan logika buka panel Settings di sini
    }

    private void ExitGame()
    {
        Debug.Log("Keluar dari game...");
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
