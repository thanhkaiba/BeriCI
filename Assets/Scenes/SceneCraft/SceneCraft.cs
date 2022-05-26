using Piratera.Utils;
using Sfs2X.Entities.Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCraft : MonoBehaviour
{
    [SerializeField]
    private Transform tableContent;
    [SerializeField]
    private GameObject tableRow;
    [SerializeField]
    private Transform nodeSailor;
    [SerializeField]
    private ItemRewards focusing_frag;
    [SerializeField]
    private Text cost, userBeri;
    [SerializeField]
    private GameObject buttonCraft, textUnfocus;
    [SerializeField]
    private SpriteRenderer focusing_poster;
    public class SailorFragmentData
    {
        public string meta;
        public long quantity;
        public SailorFragmentData(ISFSObject data)
        {
            meta = data.GetUtfString("meta");
            quantity = data.GetLong("quantity");
        }
    }
    public class PosterFragmentData
    {
        public string meta;
        public long quantity;
        public PosterFragmentData(ISFSObject data)
        {
            meta = data.GetUtfString("meta");
            quantity = data.GetLong("quantity");
        }
    }
    private List<SailorFragmentData> list_sailor_frag = new List<SailorFragmentData>();
    private List<PosterFragmentData> list_poster_frag = new List<PosterFragmentData>();
    private void Start()
    {
        NetworkController.Send(Action.GET_USER_CLAIMED_FRAGMENTS);
        userBeri.text = StringUtils.ShortNumber(UserData.Instance.Beri, 6);
        GameEvent.UserBeriChanged.AddListener(OnBeriChanged);
        Unfocus();
    }
    private void OnDestroy()
    {
        GameEvent.UserBeriChanged.RemoveListener(OnBeriChanged);
    }
    private void OnBeriChanged(long oldValue, long newValue)
    {
        DoTweenUtils.UpdateNumber(userBeri, oldValue, newValue, x => StringUtils.ShortNumber(x, 6));
    }
    public void ReceivedData(ISFSObject packet)
    {
        ISFSArray listS = packet.GetSFSArray("sailor_fragment");
        foreach (ISFSObject obj in listS)
        {
            SailorFragmentData model = new SailorFragmentData(obj);
            list_sailor_frag.Add(model);
        }
        ISFSArray listP = packet.GetSFSArray("poster_fragment");
        foreach (ISFSObject obj in listP)
        {
            PosterFragmentData model = new PosterFragmentData(obj);
            list_poster_frag.Add(model);
        }
        ShowAllFragment();
    }
    private void ShowAllFragment()
    {
        int COL_NUM = 5;
        int rowSailorNum = (int)Mathf.Ceil(list_sailor_frag.Count / (float)COL_NUM);
        int rowPosterNum = (int)Mathf.Ceil(list_poster_frag.Count / (float)COL_NUM);
        int totalRow = rowSailorNum + rowPosterNum;
        int childCount = tableContent.childCount;

        if (rowSailorNum + rowPosterNum < childCount)
            for (int i = childCount - 1; i >= totalRow; i++)
                Destroy(tableContent.GetChild(i).gameObject);

        for (int i = 0; i < rowSailorNum; i++)
        {
            GameObject row;
            if (i >= tableContent.childCount)
                row = Instantiate(tableRow, tableContent);
            else
                row = tableContent.GetChild(i).gameObject;
            for (int j = 0; j < COL_NUM; j++)
            {
                int idx = i * COL_NUM + j;
                var fragment = row.transform.Find("frag_" + j);
                if (idx >= list_sailor_frag.Count) fragment.gameObject.SetActive(false);
                else
                {
                    fragment.gameObject.SetActive(true);
                    SailorFragmentData frag = list_sailor_frag[idx];
                    fragment.GetComponent<ItemRewards>().ShowSailorPiece(frag.meta, (int)frag.quantity);
                    var button = fragment.gameObject.AddComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => FocusSailor(frag.meta));
                }
            }
        }
        for (int i = 0; i < rowPosterNum; i++)
        {
            GameObject row;
            if (i + rowSailorNum >= tableContent.childCount)
                row = Instantiate(tableRow, tableContent);
            else
                row = tableContent.GetChild(i + rowSailorNum).gameObject;
            for (int j = 0; j < COL_NUM; j++)
            {
                int idx = i * COL_NUM + j;
                var fragment = row.transform.Find("frag_" + j);
                if (idx >= list_poster_frag.Count) fragment.gameObject.SetActive(false);
                else
                {
                    fragment.gameObject.SetActive(true);
                    PosterFragmentData frag = list_poster_frag[idx];
                    fragment.GetComponent<ItemRewards>().ShowPosterPiece(frag.meta, (int)frag.quantity);
                    var button = fragment.gameObject.AddComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => FocusPoster(frag.meta));
                }
            }
        }
    }
    public void BackToCrew()
    {
        LoadServerDataUI.NextScene = "SceneCrew";
        SceneManager.LoadScene("SceneLoadServerData");
    }
    private void FocusSailor(string sailorName)
    {
        Unfocus();
        foreach (Transform child in nodeSailor)
        {
            Destroy(child.gameObject);
        }
        GameObject GO = Instantiate(GameUtils.GetSailorModelPrefab(sailorName), nodeSailor);
        GO.transform.localScale = new Vector3(-0.6f, 0.6f);
        Transform modelObject = GO.transform.FindDeepChild("model");

        var skel = modelObject.GetComponent<SkeletonMecanim>();
        if (!skel) return;
        Spine.Skeleton skeleton = skel.skeleton;
        skeleton.SetColor(new Color32(255, 255, 255, 180));
        focusing_frag.gameObject.SetActive(true);
        focusing_frag.ShowSailorPiece(sailorName, 10);
        buttonCraft.SetActive(true);
        textUnfocus.SetActive(false);

        cost.text = GetSailorCost(sailorName).ToString("N0");
    }
    private void FocusPoster(string posterName)
    {
        Unfocus();
        textUnfocus.SetActive(false);
        focusing_poster.gameObject.SetActive(true);
        focusing_poster.sprite = Resources.Load<Sprite>($"Icons/Poster/{posterName}_poster");
        focusing_frag.gameObject.SetActive(true);
        focusing_frag.ShowPosterPiece(posterName, 10);
        buttonCraft.SetActive(true);
        cost.text = GetPosterCost(posterName).ToString("N0");
    }
    private void Unfocus()
    {
        foreach (Transform child in nodeSailor)
        {
            Destroy(child.gameObject);
        }
        focusing_poster.gameObject.SetActive(false);
        focusing_frag.gameObject.SetActive(false);
        buttonCraft.SetActive(false);
        textUnfocus.SetActive(true);
    }
    private int GetPosterCost(string posterName)
    {
        switch (posterName)
        {
            case "normal":
                return 12000;
            case "premium":
                return 24000;
            case "legendary":
                return 64000;
        }
        return 0;
    }
    private int GetSailorCost(string sailorName)
    {
        SailorModel model = new SailorModel("", sailorName);
        return (int)(10000 * Mathf.Pow(2, (int)model.config_stats.rank));
    }
}