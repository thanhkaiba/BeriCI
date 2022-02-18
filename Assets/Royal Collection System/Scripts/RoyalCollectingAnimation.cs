using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoyalCollectingAnimation : MonoBehaviour
{
    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>


    public enum EXPANSION_MODE
    {
        Going_Up,
        Explosive
    }

    // Factor to adujst upper translation during animation
    public float moveUpFactor = 8.0f;

    // Factor to adujst horizontal translation during animation
    public float _moveHorizontalFactor = 3.0f;

    // Factor to adjust scaling of the item while approching destination
    public float _scaleDiminutionFactor = 2.0f;

    // Duration of the expansion animation in seconds
    public float _expansionDuration = 1.0f;

    // Parameter to adujst animation speed
    public float _animationSpeed = 2.0f;

    // Reference to the image component of the object
    public Image _image;

    //[SerializeField]
    //AudioSource audioSource;

    // Flag telling if animation is running or not for this item
    [HideInInspector] public bool _animationRunning = false;

    // Reference to the collecting Effect Controller
    private RoyalCollectingController Royal_collectinController;

    // The transform component of the currency displayer
    private Transform _itemDisplayerTransform;

    // Reference to the ItemDisplayer component, showing the amount of item collected
    [SerializeField]
    private RoyalItemDisplayer _itemDisplayer;


    // Defines the expansion mode
    private EXPANSION_MODE _expansionMode;

    private bool isCoin = false;

    private int newValue = 0;

    //private int number = 0;
    private System.Action OnCompleted;

    public void Initialize(Transform destination, Transform parent, Vector3 localPosition, Vector3 localScale, EXPANSION_MODE expansionMode,
        RoyalCollectingController collectingEffectController, int newValue, System.Action action)
    {
        _itemDisplayerTransform = destination;
        transform.SetParent(parent);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        _expansionMode = expansionMode;
        Royal_collectinController = collectingEffectController;
        this.newValue = newValue;
        this.OnCompleted = action;
        //this.number = num;
    }

    // Initialize this item
    public void Initialize(Transform destination, Transform parent, Vector3 localPosition, Vector3 localScale, EXPANSION_MODE expansionMode,
        RoyalCollectingController collectingEffectController)
    {
        _itemDisplayerTransform = destination;
        _itemDisplayer = _itemDisplayerTransform.GetComponent<RoyalItemDisplayer>();
        transform.SetParent(parent);
        transform.localPosition = localPosition;
        transform.localScale = localScale;
        _expansionMode = expansionMode;
        Royal_collectinController = collectingEffectController;
    }

    // Start the animation for this item
    public void StartAnimation()
    {
        _animationRunning = true;
        _image.enabled = true;
        StartCoroutine("CollectAnimation");
    }

    // Main loop during animation
    IEnumerator CollectAnimation()
    {
        float t = 0;
        float speed = 1.0f;



        // 1st step : Move up animation
        Vector3 direction;
        if (_expansionMode == EXPANSION_MODE.Going_Up)
        {
            direction = new Vector3((Random.value - 0.5f) * _moveHorizontalFactor, moveUpFactor, 0.0f);
        }
        else
        {
            direction = new Vector3((Random.value - 0.5f) * _moveHorizontalFactor, (Random.value - 0.5f) * moveUpFactor,
                0.0f);
        }

        while (t < _expansionDuration)
        {
            t += Time.deltaTime * _animationSpeed;
            if (_expansionMode == EXPANSION_MODE.Going_Up)
            {
                transform.position += Vector3.Scale(direction, new Vector3(1, speed, 1));
            }
            else
            {
                transform.position += Vector3.Scale(direction, new Vector3(speed, speed, 1));
            }

            speed = Mathf.Exp(-4 * t / _expansionDuration);
            yield return new WaitForFixedUpdate();
        }

        // 2nd step : Move to destination
        t = 0;
        Vector3 pos = transform.position;
        Vector3 scale = transform.localScale / _scaleDiminutionFactor;
        while (t < 1.0f)
        {
            t += Time.deltaTime * _animationSpeed;
            transform.position = Vector3.Lerp(pos, _itemDisplayerTransform.position, t);
            transform.localScale = Vector3.Lerp(transform.localScale, scale, t);
            yield return new WaitForFixedUpdate();
        }



        // Adding the gem
        //_itemDisplayer.AddItem (1);
        if (_itemDisplayer != null) _itemDisplayer.AddItem();
        _animationRunning = false;

        //this.number--;
        //Debug.Log("NUMBER: " + number);
        //if (this.number <= 0)
        //{

        Debug.Log("OnCompleted: " + OnCompleted);
        if (OnCompleted != null)
        {
            OnCompleted();
            OnCompleted = null;
        }

        //}
        // Hide this item until next reuse
        _image.enabled = false;
        yield return null;
    }
}