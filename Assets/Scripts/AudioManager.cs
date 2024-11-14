using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    public AudioClip[] menuMusicClips;
    public AudioClip[] levelMusicClips;

    [Header("Audio Settings")]
    public float transitionTime = 1f;
    private float baselineLoudness;
    private float musicVolume;

    private AudioSource _audioSource;
    private AudioClip[] _currentPlaylist;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Audio source setup
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = false;

        // Subscribe to scene change event
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void Start()
    {
        // Load baseline loudness
        baselineLoudness = PlayerPrefs.HasKey("BaselineLoudness") ? PlayerPrefs.GetFloat("BaselineLoudness") : 0.1f;
        musicVolume = Mathf.Min(baselineLoudness * 1.05f, 0.5f);  // Slightly above baseline, capped at 0.5f

        PlayMusicBasedOnScene();
    }

    private void OnDestroy()
    {
        // Unsubscribe from event to avoid memory leaks
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void Update()
    {
        // If the current track has finished, play the next one
        if (!_audioSource.isPlaying)
        {
            PlayNextClip();
        }
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        PlayMusicBasedOnScene();
    }

    private void PlayMusicBasedOnScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "IntroMenu" || currentSceneName == "PreLevelInstructions")
        {
            PlayMenuMusic();
        }
        else
        {
            PlayLevelMusic();
        }
    }

    public void PlayMenuMusic()
    {
        _currentPlaylist = menuMusicClips;
        PlayNextClip();
    }

    public void PlayLevelMusic()
    {
        _currentPlaylist = levelMusicClips;
        PlayNextClip();
    }

    private void PlayNextClip()
    {
        if (_currentPlaylist == null || _currentPlaylist.Length == 0) return;

        AudioClip nextClip = _currentPlaylist[Random.Range(0, _currentPlaylist.Length)];
        StartCoroutine(FadeInNewTrack(nextClip));
    }

    private IEnumerator FadeInNewTrack(AudioClip newClip)
    {
        // Fade out the current clip
        for (float volume = _audioSource.volume; volume > 0; volume -= Time.deltaTime / transitionTime)
        {
            _audioSource.volume = volume;
            yield return null;
        }

        // Switch to the new clip
        _audioSource.clip = newClip;
        _audioSource.volume = 0;
        _audioSource.Play();

        // Fade in to the new clip up to the calculated music volume
        for (float volume = 0; volume < musicVolume; volume += Time.deltaTime / transitionTime)
        {
            _audioSource.volume = volume;
            yield return null;
        }
    }
}
