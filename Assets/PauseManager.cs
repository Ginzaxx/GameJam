using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel;    // Panel pause menu
    public Button pauseButton;           // Tombol pause utama (UI)
    public Button resumeButton;          // Tombol resume di menu
    public Button settingsButton;        // Tombol settings di menu
    public Button mainMenuButton;        // Tombol kembali ke main menu

    [Header("Button Icons")]
    public Sprite pauseSprite;   // Icon || (game berjalan)
    public Sprite playSprite;    // Icon ▶ (game di-pause)

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu"; // Nama scene main menu

    private bool isPaused = false;

    private void Start()
    {
        // Panel tidak aktif saat awal
        pauseMenuPanel.SetActive(false);

        // Pasang listener tombol (tidak perlu OnClick manual)
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        // Deteksi input ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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
        // Di sini bisa kamu tambahkan logika buka panel Settings
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("Kembali ke Main Menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
