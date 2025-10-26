using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject gameOverPanel;

    private static GameOverUI instance;

    private void Awake()
    {
        // Singleton sederhana agar bisa diakses dari mana saja
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        gameOverPanel.SetActive(false);
    }

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

    // Tombol Restart
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Tombol Quit
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // ganti dengan nama scene menu kamu
    }
}
