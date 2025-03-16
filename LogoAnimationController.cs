using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LogoAnimationController : MonoBehaviour
{
    [Header("Escena del Inicio")]
    public string sceneName = "Inicio";

    [Header("Panel de Desvanecimiento")]
    public Image fadePanel;

    [Header("Volume para los efectos")]
    public Volume volume;

    [SerializeField, Header("Duración del Fade")]
    private float fadeDuration = 1.5f;

    [SerializeField, Header("Tiempo inicial del Fade")]
    private float initialElapsedTime = 0f;

//  Postprocesado
    private FilmGrain filmGrain;
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private PaniniProjection paniniProjection;
    private LensDistortion lensDistortion;
// Logo
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (volume != null)
        {
            if (volume.profile.TryGet(out filmGrain))
            {
                filmGrain.active = false;
            }
            else
            {
                Debug.LogWarning("Volume no contiene Film Grain.");
            }

            if (volume.profile.TryGet(out lensDistortion))
            {
                lensDistortion.active = false;
            }
            else
            {
                Debug.LogWarning("Volume no contiene Lens Distortion.");
            }

            if (volume.profile.TryGet(out bloom))
            {
                bloom.active = false;
            }
            else
            {
                Debug.LogWarning("Volume no contiene Bloom.");
            }

            if (volume.profile.TryGet(out chromaticAberration))
            {
                chromaticAberration.active = false;
            }
            else
            {
                Debug.LogWarning("Volume no contiene Chromatic Aberration.");
            }

            if (volume.profile.TryGet(out paniniProjection))
            {
                paniniProjection.active = false;
            }
            else
            {
                Debug.LogWarning("Volume no contiene Panini Projection.");
            }
        }
        else
        {
            Debug.LogError("El Volume no está asignado en el inspector.");
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(false);
        }

        StartCoroutine(WaitForAnimation());
    }

    private IEnumerator WaitForAnimation()
    {
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            yield return StartCoroutine(FadeOut());
        }

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOut()
    {
        Color panelColor = fadePanel.color;
        float elapsedTime = initialElapsedTime;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = panelColor;
            yield return null;
        }
    }

    public void ActivateEffects()
    {
        if (filmGrain != null)
        {
            filmGrain.active = true;
        }
        if (bloom != null)
        {
            bloom.active = true;
        }
        if (chromaticAberration != null)
        {
            chromaticAberration.active = true;
        }
        if (paniniProjection != null)
        {
            paniniProjection.active = true;
        }
        if (lensDistortion != null)
        {
            lensDistortion.active = true;
        }

        Debug.Log("Efectos activados: Film Grain, Bloom, Chromatic Aberration, Panini Projection");
    }
}
