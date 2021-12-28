﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoyalCollectingController : MonoBehaviour {

    /// <summary>
    /// RoyalCollectionSystem Package
    /// </summary>



    // Play collecting sound at begining of the animation or at the end
    public RoyalCollectingAnimation.PLAY_SOUND_MODE _playSoundMode;
    public RoyalCollectingAnimation.EXPANSION_MODE _expansionMode = RoyalCollectingAnimation.EXPANSION_MODE.Going_Up;
    // The emission rate in seconds
	public float emissionRate = 0.2f;
	// The tranform component of the item displayer
	public Transform itemDisplayer;
	// The position where to pop the items
	public Transform popPosition;
	// The prefab of the items to instanciate
	public GameObject itemPrefab;
	// Instance of this class
	[HideInInspector]
	public static RoyalCollectingController _instance;

	// This is a list of instanciated _itemPrefab 
	private List<RoyalCollectingAnimation> _itemList = new List<RoyalCollectingAnimation>();
	// Reference to the AudioSource component
	[SerializeField]
	private List<AudioSource> _audioSource;
	[SerializeField]
	AudioClip audio;

	void Awake() {
		// Setting instance
		_instance = this;
	//	_audioSource = GetComponent<AudioSource> ();
	}

	// Collect some items with animation
	public void CollectItem(int quantity)
    {
		StartCoroutine (PopItems(quantity));
	}

	public void CollectItem(int quantity, int value, System.Action OnComplete)
	{
		int valueExtend = 0;
		if(quantity != 0) valueExtend = (int)(value / quantity);
		StartCoroutine(PopItems(quantity, valueExtend, OnComplete));
	}



	// Collect some items with animation at a fixed position
	public void CollectItemAtPosition(int quantity, Vector3 position) {
        // Set the position
        popPosition.position = position;
		StartCoroutine (PopItems(quantity));
	}

	// Here we pop all the necessary items
	IEnumerator PopItems(int quantity) {
		WaitForSeconds delay = new WaitForSeconds (emissionRate);
		for (int i = 0; i < quantity; i++) {
            RoyalCollectingAnimation animation = null;
			if(i < _itemList.Count) {
				if(!_itemList[i]._animationRunning) {
					// A free object has been found in pool, so we reuse it
					animation = _itemList[i];
				}
			} 
			if(animation == null) {
				// No free object has been found in pool, so we instantiate a new one
				GameObject go = Instantiate (itemPrefab) as GameObject;
				animation = go.GetComponent<RoyalCollectingAnimation>();
				_itemList.Add(animation);
                Destroy(go , 2.2f);//Remove go Object
            }

			// Initialize object
			animation.Initialize(itemDisplayer, popPosition, Vector3.zero, Vector3.one, _playSoundMode, _expansionMode, this);
			// Start animation
			animation.StartAnimation();
			yield return delay;
		}
	}

	IEnumerator PopItems(int quantity, int newValue, System.Action action)
	{
		WaitForSeconds delay = new WaitForSeconds(emissionRate);
		int number = quantity;
		for (int i = 0; i < quantity; i++)
		{
			RoyalCollectingAnimation animation = null;
			if (i < _itemList.Count)
			{
				if (!_itemList[i]._animationRunning)
				{
					// A free object has been found in pool, so we reuse it
					animation = _itemList[i];
				}
			}
			Debug.Log("Animation: " + animation);
			if (animation == null)
			{
				// No free object has been found in pool, so we instantiate a new one
				GameObject go = Instantiate(itemPrefab) as GameObject;
				animation = go.GetComponent<RoyalCollectingAnimation>();
				_itemList.Add(animation);
				Destroy(go, 2.2f);//Remove go Object
			}

			// Initialize object
			animation.Initialize(itemDisplayer, popPosition, Vector3.zero, Vector3.one, _playSoundMode, _expansionMode, this, newValue, ()=> {
				number--;
				// Debug.Log("NUMBER_CHEKED: " + number);
				if( number <= 0)
                {
					action();
					action = null;
                }
			});
			// Start animation
			animation.StartAnimation();
			yield return delay;
		}
	}


	// Play the collecting sound
/*	public void PlayCollectingSound()
    {
		//	_audioSource.Play ();

		for (int i = 0; i < _audioSource.Count; i++)
		{
			if (!_audioSource[i].isPlaying)
			{
				_audioSource[i].volume = Utils.GetInt(Utils.KEY_SOUND, 1); //PlayerPrefs.GetInt("enableSound", 1);
				_audioSource[i].PlayOneShot(audio);
				break;
			}
		}

	}*/
}
