using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAnimatorCtrl : MonoBehaviour
{
    public GameObject modelObject;
    public Slider healthBar;
    public Text healthText;
    public Slider speedBar;
    public Text speedText;
    public Slider furyBar;
    public Text furyText;
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
    public void SetSpeedBar(int max, int min)
    {
        speedBar.value = (float)min / max;
        speedText.text = min + "/" + max;
    }
    public void SetFuryBar(int max, int min)
    {
        furyBar.gameObject.SetActive(max != 0);
        if (max != 0)
        {
            furyBar.value = (float)min / max;
            furyText.text = min + "/" + max;
        }
    }
}
