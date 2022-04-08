using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIUpgradeShip : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeBackground;
    [SerializeField] private GameObject ship, tempShip, shipShadow;
    [SerializeField] private Camera mainCamera, sailCamera, helmCamera;
    [SerializeField] private GameObject zoomButtonGroup, upgradeSail, upgradeHelm;
    [SerializeField] private Button backButton;

    private Vector3 originCameraPosition;
    private float originCameraFOV;

    // Start is called before the first frame update
    void Start()
    {
        // Init variable
        float animTime = 1f;
        float seqDelay = 0.5f;
        originCameraPosition = mainCamera.transform.position;
        originCameraFOV = mainCamera.fieldOfView;
        upgradeSail.SetActive(false);
        upgradeHelm.SetActive(false);

        // Wave animation
        Sequence waveSequence = DOTween.Sequence();
        waveSequence.Append(
            fadeBackground.DOFade(0f, animTime).SetEase(Ease.InOutSine)
        ).Append(
            fadeBackground.DOFade(1f, animTime * 1.5f).SetEase(Ease.InOutSine)
        ).SetDelay(seqDelay);
        waveSequence.SetLoops(-1, LoopType.Restart);

        // Ship animation
        Vector3 oldShipTransform = ship.transform.position;

        Sequence shipSequenceMove = DOTween.Sequence();
        shipSequenceMove.Append(
            ship.transform.DOMove(tempShip.transform.position, animTime).SetEase(Ease.InOutSine)
        ).Append(
            ship.transform.DOMove(oldShipTransform, animTime * 1.5f).SetEase(Ease.InOutSine)
        ).SetDelay(seqDelay);
        shipSequenceMove.SetLoops(-1, LoopType.Restart);

        Sequence shipSequenceRotate = DOTween.Sequence();
        shipSequenceRotate.Append(
            ship.transform.DORotate(new Vector3(0,0,2), animTime).SetEase(Ease.InOutSine)
        ).Append(
            ship.transform.DORotate(new Vector3(0, 0, -2), animTime * 1.5f).SetEase(Ease.InOutSine)
        ).SetDelay(seqDelay);
        shipSequenceRotate.SetLoops(-1, LoopType.Restart);

        //// Ship shadow animation
        //Vector3 oldShadowScale = ship.transform.localScale;

        //Sequence shipShadowScale = DOTween.Sequence();
        //shipShadowScale.Append(
        //    shipShadow.transform.DOScale(shipShadow.transform.localScale + new Vector3(0.05f, 0.05f, 0.05f), animTime).SetEase(Ease.InOutSine)
        //).Append(
        //    shipShadow.transform.DOScale(oldShadowScale, animTime * 1.5f).SetEase(Ease.InOutSine)
        //).SetDelay(seqDelay);
        //shipShadowScale.SetLoops(-1, LoopType.Restart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Show upgrade sail scene
    public void zoomInSail()
    {
        // Zoom sail
        mainCamera.transform.DOMove(sailCamera.transform.position, 0.8f).SetEase(Ease.InOutSine);
        mainCamera.DOFieldOfView(sailCamera.fieldOfView, 0.8f).SetEase(Ease.InOutSine);

        // Update UI button
        zoomButtonGroup.SetActive(false);
        upgradeSail.SetActive(true);

        // Update BackButton event
        backButton.onClick.AddListener(zoomOut);
    }

    // Show upgrade helm scene
    public void zoomInHelm()
    {
        // Zoom sail
        mainCamera.transform.DOMove(helmCamera.transform.position, 0.8f).SetEase(Ease.InOutSine);
        mainCamera.DOFieldOfView(helmCamera.fieldOfView, 0.8f).SetEase(Ease.InOutSine);

        // Update UI button
        zoomButtonGroup.SetActive(false);
        upgradeHelm.SetActive(true);

        // Update BackButton event
        backButton.onClick.AddListener(zoomOut);
    }

    // Hide upgrade saile scene
    private void zoomOut()
    {
        // Zoom sail
        mainCamera.transform.DOMove(originCameraPosition, 0.8f).SetEase(Ease.InOutSine);
        mainCamera.DOFieldOfView(originCameraFOV, 0.8f).SetEase(Ease.InOutSine);

        // Hide button
        zoomButtonGroup.SetActive(true);
        upgradeSail.SetActive(false);
        upgradeHelm.SetActive(false);
    }
}
