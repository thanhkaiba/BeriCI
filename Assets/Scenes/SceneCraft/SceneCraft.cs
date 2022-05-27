using DG.Tweening;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneCraft : MonoBehaviour
{
    int FRAG_NUM = 10;
    string focus_meta = "";
    string focus_type = ""; // sailor_fragment/poster_fragment
    int cache_cost = 0;

    [SerializeField]
    private Transform tableContent;
    [SerializeField]
    private GameObject tableRow;
    [SerializeField]
    private Transform nodeSailor;
    [SerializeField]
    private ItemRewards focusing_frag;
    [SerializeField]
    private Text cost, userBeri, textError;
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
        Unfocus();
    }
    private void DecreaseBeri(long decrease)
    {
        var oldValue = UserData.Instance.Beri;
        UserData.Instance.Beri -= decrease;
        DoTweenUtils.UpdateNumber(userBeri, oldValue, UserData.Instance.Beri, x => StringUtils.ShortNumber(x, 6));
    }
    public void ReceivedData(ISFSObject packet)
    {
        list_sailor_frag = new List<SailorFragmentData>();
        ISFSArray listS = packet.GetSFSArray("sailor_fragment");
        foreach (ISFSObject obj in listS)
        {
            SailorFragmentData model = new SailorFragmentData(obj);
            list_sailor_frag.Add(model);
        }
        list_poster_frag = new List<PosterFragmentData>();
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
        list_sailor_frag = list_sailor_frag.Where(e => e.quantity > 0).ToList();
        list_poster_frag = list_poster_frag.Where(e => e.quantity > 0).ToList();
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
                    var itemRewards = fragment.GetComponent<ItemRewards>();
                    itemRewards.ShowSailorPiece(frag.meta, (int)frag.quantity);
                    itemRewards.ShowInfocus(false);
                    var button = fragment.gameObject.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => FocusSailor(frag.meta, (int)frag.quantity, itemRewards));
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
                    var itemRewards = fragment.GetComponent<ItemRewards>();
                    itemRewards.ShowPosterPiece(frag.meta, (int)frag.quantity);
                    itemRewards.ShowInfocus(false);
                    var button = fragment.gameObject.GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => FocusPoster(frag.meta, (int)frag.quantity, itemRewards));
                }
            }
        }
    }
    public void BackToCrew()
    {
        LoadServerDataUI.NextScene = "SceneCrew";
        SceneManager.LoadScene("SceneLoadServerData");
    }
    private void FocusSailor(string sailorName, int quantity, ItemRewards fragment)
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
        if (quantity >= FRAG_NUM)
        {
            focusing_frag.ShowSailorPiece(sailorName, FRAG_NUM + "/" + FRAG_NUM, new Color32(255,255,255,255));
            fragment.ShowSailorPiece(sailorName, quantity - FRAG_NUM);
            FlyFragment(fragment, FRAG_NUM);
        }
        else
        {
            focusing_frag.ShowSailorPiece(sailorName, quantity+"/"+ FRAG_NUM, new Color32(255, 66, 66, 255));
            fragment.ShowSailorPiece(sailorName, 0);
            FlyFragment(fragment, quantity);
        }
        fragment.ShowInfocus(true);
        buttonCraft.SetActive(true);
        textUnfocus.SetActive(false);

        cache_cost = GetSailorCost(sailorName);
        cost.text = cache_cost.ToString("N0");
        cost.color = UserData.Instance.Beri >= cache_cost ? new Color(1, 1f, 1f, 1) : new Color(1, 0.6f, 0.6f, 1);
        buttonCraft.GetComponent<Button>().interactable = UserData.Instance.Beri >= cache_cost && quantity >= FRAG_NUM;
        focus_meta = sailorName;
        focus_type = "sailor_fragment";
    }
    private void FocusPoster(string posterName, int quantity, ItemRewards fragment)
    {
        Unfocus();
        textUnfocus.SetActive(false);
        focusing_poster.gameObject.SetActive(true);
        focusing_poster.sprite = Resources.Load<Sprite>($"Icons/Poster/{posterName}_poster");
        focusing_frag.gameObject.SetActive(true);
        if (quantity >= FRAG_NUM)
        {
            focusing_frag.ShowPosterPiece(posterName, FRAG_NUM+"/"+FRAG_NUM, new Color32(255, 255, 255, 255));
            buttonCraft.GetComponent<Button>().interactable = true;
            fragment.ShowPosterPiece(posterName, quantity - FRAG_NUM);
            FlyFragment(fragment, FRAG_NUM);
        }
        else
        {
            focusing_frag.ShowPosterPiece(posterName, quantity + "/"+FRAG_NUM, new Color32(255, 66, 66, 255));
            buttonCraft.GetComponent<Button>().interactable = false;
            fragment.ShowPosterPiece(posterName, 0);
            FlyFragment(fragment, quantity);
        }
        fragment.ShowInfocus(true);
        buttonCraft.SetActive(true);
        cache_cost = GetPosterCost(posterName);
        cost.text = cache_cost.ToString("N0");
        cost.color = UserData.Instance.Beri >= cache_cost ? new Color(1, 1f, 1f, 1) : new Color(1, 0.6f, 0.6f, 1);
        buttonCraft.GetComponent<Button>().interactable = UserData.Instance.Beri >= cache_cost && quantity >= FRAG_NUM;
        focus_meta = posterName;
        focus_type = "poster_fragment";
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
        textError.text = "";
        ShowAllFragment();
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
    private void FlyFragment(ItemRewards fragment, int quantity)
    {
        var go = Instantiate(fragment.gameObject, focusing_frag.transform.parent);
        go.transform.position = fragment.transform.position;
        go.GetComponent<ItemRewards>().SetQuantity("x" + quantity);
        var seq = DOTween.Sequence();
        seq.Append(go.transform.DOMove(focusing_frag.transform.position, 0.3f));
        seq.AppendCallback(() => Destroy(go));
    }
    public void Craft()
    {
        textError.text = "";
        //CraftSuccess(focus_meta, 100);
        //return;
        SFSObject data = new SFSObject();
        data.PutUtfString("meta", focus_meta);
        data.PutUtfString("type", focus_type);
        NetworkController.Send(Action.MERGE_FRAGMENTS, data);
        SceneTransition.Instance.ShowWaiting(true);
    }
    public void CraftFail()
    {
        SceneTransition.Instance.ShowWaiting(false);
        textError.text = "An error has happend";
    }
    public void CraftSuccess(string sailorName, int quality)
    {
        DecreaseBeri(cache_cost);

        SceneTransition.Instance.ShowWaiting(false);
        for (int i = 0; i < FRAG_NUM; i++)
        {
            var pos = nodeSailor.transform.position;
            pos.y += 1.5f;
            var go = Instantiate(focusing_frag.gameObject, focusing_frag.transform.parent);
            go.GetComponent<ItemRewards>().SetQuantity("");
            go.SetActive(false);
            var seq = DOTween.Sequence();
            seq.AppendInterval(0.0f + i*0.1f);
            seq.AppendCallback(() => go.SetActive(true));
            seq.Append(go.transform.DOMove(pos, 0.3f));
            seq.AppendCallback(() => Destroy(go));
        }

        var seq2 = DOTween.Sequence();
        seq2.AppendInterval(0.8f);
        seq2.AppendCallback(() =>
        {
            foreach (Transform child in nodeSailor)
                Destroy(child.gameObject);
            GameObject GO = Instantiate(GameUtils.GetSailorModelPrefab(sailorName), nodeSailor);
            GO.transform.localScale = new Vector3(-0.6f, 0.6f);
            Transform modelObject = GO.transform.FindDeepChild("model");

            NetworkController.Send(Action.GET_USER_CLAIMED_FRAGMENTS);
            ShowCongrat(new SailorModel("", sailorName) { quality = quality, level = 1 });
        });
    }
    [SerializeField]
    private GameObject panelCongrat;
    [SerializeField]
    private IconSailor iconSailor;
    [SerializeField]
    private Text textTitle, textWelcome;
    [SerializeField]
    private Transform btnOk;
    private void ShowCongrat(SailorModel model)
    {
        panelCongrat.SetActive(true);
        iconSailor.PresentData(model);
        var canvasGroup = panelCongrat.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(255, 0.2f).SetRelative();

        textWelcome.text = $"Welcome <color=#fffa73>{model.config_stats.root_name}</color> to our ship";

        var listGO = new List<Transform>() {
            textTitle.transform,
            iconSailor.transform,
            textWelcome.transform,
            btnOk.transform,
        };
        for (int i = 0; i < listGO.Count; i++)
        {
            var seq = DOTween.Sequence();
            var t = listGO[i];
            t.localScale = Vector3.zero;
            seq.AppendInterval(i*0.25f);
            seq.Append(t.DOScale(Vector3.one * 1.2f, 0.5f).SetEase(Ease.OutBack));
        }
    }
    public void CloseCongrat()
    {
        panelCongrat.SetActive(false);
    }
}