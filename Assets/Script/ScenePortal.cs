using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScenePortal : MonoBehaviour
{
    [Header("Scene Tujuan")]
#if UNITY_EDITOR
    [Tooltip("Drag scene tujuan ke sini (SceneAsset dari folder Scenes)")]
    public SceneAsset targetScene;
#endif

    [HideInInspector] public string sceneName; // disimpan otomatis dari SceneAsset

    [Header("UI Interaksi")]
    public GameObject interactUI;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange;

    private void Awake()
    {
#if UNITY_EDITOR
        // simpan nama scene dari SceneAsset agar tetap berfungsi di runtime
        if (targetScene != null)
            sceneName = targetScene.name;
#endif

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey))
            ChangeScene();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    private void ChangeScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("⚠️ Scene tujuan belum dipilih di Inspector!");
            return;
        }

        Debug.Log($"🔄 Memuat scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
