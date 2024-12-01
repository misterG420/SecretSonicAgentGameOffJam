using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicManager : MonoBehaviour
{
    [SerializeField] private string[] allowedScenes;
    private AudioSource audioSource;
    private bool isFading = false;
    private float fadeDuration = 2f;
    private float fadeTimer = 0f;
    private float initialVolume;

    private static MenuMusicManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        audioSource = GetComponent<AudioSource>();
        initialVolume = audioSource.volume;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        CheckCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float fadeProgress = fadeTimer / fadeDuration;
            audioSource.volume = Mathf.Lerp(initialVolume, 0f, fadeProgress);

            if (fadeTimer >= fadeDuration)
            {
                audioSource.volume = 0f;
                audioSource.Stop();
                isFading = false;
                Destroy(gameObject);
            }
        }
    }

    public void FadeOutMusic()
    {
        if (!isFading)
        {
            isFading = true;
            fadeTimer = 0f;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckScene(scene.name);
    }

    private void CheckCurrentScene()
    {
        CheckScene(SceneManager.GetActiveScene().name);
    }

    private void CheckScene(string sceneName)
    {
        if (System.Array.Exists(allowedScenes, allowedScene => allowedScene == sceneName)) return;

        FadeOutMusic();
    }
}
