using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerTutorialLevelController : MonoBehaviour
{
    private float baselineLoudness;
    public Slider loudnessSlider;
    public float amplificationFactor = 35f;
    private float revealTimer = 0f;
    public float resetTime = 2f;

    private AudioClip microphoneClip;
    private Coroutine revealWaveCoroutine = null;
    private bool isMapRevealed = false;
    private bool hasInteractedWithOperator = false;

    private float revealRadius = 2f;
    private float revealSpeed = 12f;
    private float revealDelay = 0.02f;

    public Animator playerAnimator;
    private float shoutThreshold = 0.7f;
    public SpriteRenderer playerSpriteRenderer;
    public Sprite originalSprite;

    void Start()
    {
      //  Debug.Log("PlayerTutorialLevelController has been activated now");

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (playerSpriteRenderer != null)
        {
            originalSprite = playerSpriteRenderer.sprite;
        }

        // Disable features initially
        if (loudnessSlider != null)
        {
            loudnessSlider.gameObject.SetActive(false);
        }

        ShowAllMapObjects();
    }

    void Update()
    {
        if (hasInteractedWithOperator)
        {
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Shouting") && stateInfo.normalizedTime >= 1f)
            {
                // Reset the trigger after the shout animation is done playing
                playerAnimator.ResetTrigger("Shout");
            }
        }
    }


    private void ShowAllMapObjects()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MapObject"))
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                Color color = renderer.color;
                color.a = 1f;
                renderer.color = color;
            }
        }
        isMapRevealed = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasInteractedWithOperator && collision.CompareTag("Operator"))
        {
            hasInteractedWithOperator = true;
        }
    }

 
}
