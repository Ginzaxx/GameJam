using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class StartMenuController : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject creditsCanvas;
    [SerializeField] private CanvasGroup blackScreenGroup;

    [Header("Duration")]
    public float fadeDuration = 1f;

    [Header("Scene to Load - New Game")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif
    [SerializeField, HideInInspector] private string sceneName;

    [Header("Scene to Load - Continue")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset continueSceneAsset;
#endif
    [SerializeField, HideInInspector] private string continueSceneName;

    private bool isFading = false;
    private string targetSceneToLoad;

    // 🧩 Auto-update scene name saat di Inspector
    void OnValidate()
    {
#if UNITY_EDITOR
        if (sceneAsset != null)
        {
            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
            sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        if (continueSceneAsset != null)
        {
            string scenePath = AssetDatabase.GetAssetPath(continueSceneAsset);
            continueSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }
#endif
    }

    void Start()
    {
        if (blackScreenGroup != null)
            blackScreenGroup.alpha = 0f;
    }

    // === Fade & Load ===
    private IEnumerator FadeInAndLoadScene(string sceneToLoad)
    {
        isFading = true;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            blackScreenGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        blackScreenGroup.alpha = 1f;
        isFading = false;

        SceneManager.LoadScene(sceneToLoad);
    }

    public void OnContinueClicked()
    {
        if (!isFading && !string.IsNullOrEmpty(continueSceneName))
        {
            targetSceneToLoad = continueSceneName;
            StartCoroutine(FadeInAndLoadScene(targetSceneToLoad));
        }
        else
        {
            Debug.LogWarning("Continue scene is not assigned in Inspector!");
        }
    }

    public void OnCreditsClicked()
    {
        creditsCanvas.SetActive(!creditsCanvas.activeSelf);
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
