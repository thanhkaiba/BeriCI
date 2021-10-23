using System.Collections;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UnityEngine.Localization.Samples
{
    /// <summary>
    /// This example shows how a loading screen can be displayed while Localization Initialization/Preloading is being performed.
    /// </summary>
    public class LoadingManager : MonoBehaviour
    {
        [Header("Pre-Loading UI")]
        [SerializeField]
        private GameObject root;
        [SerializeField]
        private Image background;
        [SerializeField]
        private Text progressText;
        [SerializeField]
        private float crossFadeTime = 0.5f;

        [Header("Loading UI")]
        [SerializeField]
        private Button buttonLogin;



        WaitForSecondsRealtime waitForSecondsRealtime;

        void OnEnable()
        {
            buttonLogin.gameObject.SetActive(false);

            if (waitForSecondsRealtime == null)
                waitForSecondsRealtime = new WaitForSecondsRealtime(crossFadeTime);

            if (!LocalizationSettings.InitializationOperation.IsDone)
                StartCoroutine(Preload());
        }

   
        IEnumerator Preload()
        {
            root.SetActive(true);
           
            background.CrossFadeAlpha(1, crossFadeTime, true);
            progressText.CrossFadeAlpha(1, crossFadeTime, true);

            // Check for localize loading
            ResourceManagement.AsyncOperations.AsyncOperationHandle<LocalizationSettings> operation = LocalizationSettings.InitializationOperation;
            Locale locale = LocalizationSettings.SelectedLocale;
            do
            {
                progressText.text = $"{locale?.Identifier.CultureInfo.NativeName} {operation.PercentComplete * 100}%";
                yield return null;
            }
            while (!operation.IsDone);


            progressText.text = $"{locale?.Identifier.CultureInfo.NativeName} {operation.PercentComplete * 100}%";
            if (operation.Status == ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
            {
                progressText.text = operation.OperationException.ToString();
                progressText.color = Color.red;
            }
            else
            {
                background.CrossFadeAlpha(0, crossFadeTime, true);
                progressText.CrossFadeAlpha(0, crossFadeTime, true);

                waitForSecondsRealtime.Reset();
                yield return waitForSecondsRealtime;

                TextLocalization.Text("HELO", text => Debug.Log("Loading Manager: " + text));
                root.SetActive(false);
                buttonLogin.gameObject.SetActive(true);
            }
        }
    }
}
