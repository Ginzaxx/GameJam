using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // <- untuk Button

public class GameOverUI : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Panel Game Over utama yang akan diaktifkan ketika pemain mati.")]
    public GameObject gameOverPanel;

    [Header("Buttons (Hubungkan di Inspector)")]
    [Tooltip("Tombol untuk me-restart level saat ini.")]
    public Button restartButton;
    [Tooltip("Tombol untuk kembali ke Main Menu.")]
    public Button quitButton;

    [Header("Main Menu Scene Name")]
    [Tooltip("Nama scene main menu. Harus sesuai di Build Settings.")]
    public string mainMenuScene = "MainMenu";

    private static GameOverUI instance;

    private void Awake()
    {
        // Singleton sederhana agar bisa diakses dari mana saja
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Matikan panel saat awal
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        // ✅ Tambahkan event listener otomatis
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    // 🧩 Fungsi dipanggil dari script lain
    public static void Show()
    {
        if (instance == null)
        {
            Debug.LogWarning("⚠️ GameOverUI belum ada di scene!");
            return;
        }

        instance.gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // hentikan waktu
        Debug.Log("💀 GAME OVER SCREEN DITAMPILKAN");
    }

    // 🔁 Tombol Restart
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 🚪 Tombol Quit
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }
}
