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

    [Header("Audio")]
    [Tooltip("Nama sound di SoundManager (misalnya: 'Lift')")]
    public string liftSfxName = "Lift";
    [Tooltip("Volume SFX lift")]
    [Range(0f, 1f)] public float liftSfxVolume = 1f;

    private bool playerInRange;
    private bool isTransitioning = false;

    private void Awake()
    {
#if UNITY_EDITOR
        if (targetScene != null)
            sceneName = targetScene.name;
#endif

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactKey) && !isTransitioning)
        {
            StartCoroutine(PlayLiftAndChangeScene());
        }
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

    private System.Collections.IEnumerator PlayLiftAndChangeScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("⚠️ Scene tujuan belum dipilih di Inspector!");
            yield break;
        }

        isTransitioning = true;

        SoundManager.PlaySound("liftS");

        // ⏱️ Tunggu 1 detik sebelum ganti scene
        yield return new WaitForSeconds(1f);

        Debug.Log($"🔄 Memuat scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
