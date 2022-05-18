using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnSort : MonoBehaviour
{
    private void Start()
    {
        ShowTextSortType();
    }
    [SerializeField]
    private Text textSort;
    public void SortListSailor()
    {
        int sortType = PlayerPrefs.GetInt("sort_type", 0);
        sortType = (sortType + 1) % 3;
        PlayerPrefs.SetInt("sort_type", sortType);
        ShowTextSortType();
    }
    private void ShowTextSortType()
    {
        int sortType = PlayerPrefs.GetInt("sort_type", 0);
        switch (sortType)
        {
            case 0:
                textSort.text = "Rank";
                break;
            case 1:
                textSort.text = "Star";
                break;
            default:
                textSort.text = "Level";
                break;
        }
    }
}
