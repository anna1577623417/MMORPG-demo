using Assets.Scripts.Managers;
using Common.Data;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem :UIWindow {
    public Text title;
    public GameObject itemPrefab;

    [SerializeField] private TabView Tabs;
    [SerializeField] private ListView listMain;
    [SerializeField] private ListView listBranch;

    ListView PreListSelected=null;//记录当前哪一个列表的任务项被点击

    [SerializeField] private UIQuestInfo questInfo;

    private bool showAvailableList = false;

     void Start () {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefeshUI();
        //QuestManager.Instance.OnQuestChanged += RefeshUI;
    }

    private void OnQuestSelected(ListView.ListViewItem item) {
        if (PreListSelected==null) {
            PreListSelected = item.owner;//首次点击任务项，赋值
        } else if (item.owner != PreListSelected) {//item.owner != ListSelected处理主线和支线列表的选择状态切换
            //进入这一个语块时，意味着当前点击的列表和上次点击的列表不同，如上次点击主线任务，这次点击支线任务
            Debug.Log("当前选中："+item.owner.ToString()+ "之前选中："+PreListSelected.ToString());
            PreListSelected.selectedItem.Selected = false;//取消之前列表的选择物体的选择状态
            PreListSelected.selectedItem = null;//并置空选择物体

            PreListSelected = item.owner;//更新被选中的列表
        }
        UIQuestItem questItem = item as UIQuestItem;//转换类型
        this.questInfo.SetQuestInfo(questItem.quest);//每次点击选中任务后，更新任务信息
    }
    private void OnSelectTab(int index) {
        showAvailableList = index == 1;//是否显示可接任务,默认显示进行中的任务
        RefeshUI();
    }

    private void RefeshUI() {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    private void InitAllQuestItems() {
        //根据showAvailableList来筛选questitem，实现用两个按钮来实现不同类型的任务列表的切换
        //进行中和可接任务的切换，这两种任务分别又会出现在主线支线两个列表中
        //这两个列表有实体ui对象（prefab），而进行中和可接任务列表没有实体ui对象，
        //但是通过筛选我们可以实现两个按钮显示不同类别的物件(questItem)
        //showAvailableList->OnSelectTab(UIQuestSystem)->OnTabSelect(TabView)
        foreach (var kv in QuestManager.Instance.allQuests) {
            if (showAvailableList) {
                if (kv.Value.Info != null) {//可用且已接受
                    continue;
                }
            } else {
                if (kv.Value.Info == null) {//不可用且未接
                    continue;
                }
            }

            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main) {
                this.listMain.AddItem(ui);
            } else {
                this.listBranch.AddItem(ui); 
            }
        }
    }

    private void ClearAllQuestList() {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    private void OnDestroy() {
        this.listMain.onItemSelected -= this.OnQuestSelected;
        this.listBranch.onItemSelected -= this.OnQuestSelected;
        this.Tabs.OnTabSelect -= OnSelectTab;

        if (PreListSelected != null && PreListSelected.selectedItem != null) {
            PreListSelected.selectedItem.Selected = false;
            PreListSelected.selectedItem = null;
        }
        PreListSelected = null; // 强制置空，确保下次初始化时重新赋值
    }
}

