using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TextDamage : MonoBehaviour
{
    public GameObject iconCrit;
    void Start()
    {
        transform.DOMoveY(transform.position.y + 70, 1.0f).SetEase(Ease.OutCubic);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(1.5f);
        seq.Append(transform.GetComponent<Text>().DOFade(0, 0.2f));
        seq.AppendCallback(() => { Destroy(gameObject); });
        seq.SetTarget(transform).SetLink(gameObject);
    }

    public void Present(float damage, Vector3 p, bool isCrit, Color color, int size = 40)
    {
        if (damage < 0)
            GetComponent<Text>().text = ((int)damage).ToString("N0");
        else
            GetComponent<Text>().text = "+" + ((int)damage).ToString("N0");
        transform.position = p;
        GetComponent<Text>().fontSize = isCrit ? (int)(size*1.1f) : size;
        GetComponent<Text>().color = color;
        if (iconCrit != null) iconCrit.SetActive(isCrit);
    }
    public void AddExp(int exp, Vector3 p, Color color)
    {
        GetComponent<Text>().text = "+ " + exp.ToString("N0") + " exp";
        transform.position = p;
        GetComponent<Text>().color = color;
        if (iconCrit != null) iconCrit.SetActive(false);
    }
}
