using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimatorCtrl : MonoBehaviour
{
    public GameObject modelObject;
    public Slider healthBar;
    public Text healthText;
    public void BaseAttack()
    {
        modelObject.GetComponent<Animator>().SetTrigger("BaseAttack");
    }
    public void Death()
    {
        modelObject.GetComponent<Animator>().SetTrigger("Death");
        StartCoroutine(InActiveDelay(0.6f));
        
    }
    IEnumerator InActiveDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
    public void SetHealthBar(int max, int min)
    {
        healthBar.value = (float) min / max;
        healthText.text = min.ToString();
    }
}
