using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
// using DG.Tweening;

public class MenuController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PixelPerfectCamera pixelPerfectCam;
    [SerializeField] private float zoomFactor = 1.5f;
    [SerializeField] private float zoomDuration = 0.5f;
    public Animator playerAnimator;
    public List<GameObject> canvases;
    private int defaultPPU;

    private void Start()
    {
        if (pixelPerfectCam == null)
            pixelPerfectCam = Camera.main.GetComponent<PixelPerfectCamera>();

        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        defaultPPU = pixelPerfectCam.assetsPPU;
    }

    public void ActivarCanvasMenu(int index)
    {
        if (index >= 0 && index < canvases.Count)
        {
            canvases[index].SetActive(true); 
        }
    }

    public void CerrarCanvasMenu(int index)
    {
        if (index >= 0 && index < canvases.Count)
        {
            canvases[index].SetActive(false);
        }
    }

    public void OnOpenMenu(InputAction.CallbackContext context)
    {
            playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            SmoothZoom(zoomFactor, zoomDuration);
            Time.timeScale = 0;
            playerInput.SwitchCurrentActionMap("UI");
    }

    public void OnCloseMenu(InputAction.CallbackContext context)
    {
            playerAnimator.updateMode = AnimatorUpdateMode.Normal;
            SmoothZoom(1f, zoomDuration);
            Time.timeScale = 1;
            playerInput.SwitchCurrentActionMap("Player");
    }

    private void SmoothZoom(float sizePercentage, float duration)
    {
        int targetPPU = Mathf.RoundToInt(defaultPPU * sizePercentage);
        // DOTween.To(() => pixelPerfectCam.assetsPPU, x => pixelPerfectCam.assetsPPU = x, targetPPU, duration).SetUpdate(true);
    }

        public void SalirJuego()
    {
        Application.Quit();
    }
}