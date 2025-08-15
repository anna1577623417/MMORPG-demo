using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


[System.Serializable]
public class ItemSelectEvent : UnityEvent<ListView.ListViewItem> {

}

/// <summary>
/// 用来管理同一类型的不同列表
/// 也就是主线任务列表和支线任务列表
/// </summary>
public class ListView : MonoBehaviour {
    public UnityAction<ListViewItem> onItemSelected;
    //内置了一个列表元素类，用来处理点击事件 

    public class ListViewItem : MonoBehaviour,IPointerDownHandler {
        private bool selected;
        public bool Selected {
            get { return selected; }
            set {// 仅当状态变化时更新
                if (selected != value) {
                    selected = value;
                    OnSelected(selected);
                }
            }
        }
        public virtual void OnSelected(bool selected) {
        
        }

        public ListView owner;//列表项所属的列表

        /// <summary>
        /// 负责管理owner（列表）的选中项
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData) {
            Debug.Log("已点击任务项");
            // 【修改点1】直接设置owner的选中项
            if (owner != null)
                owner.SelectedItem = this;
        }
    }

    List<ListViewItem> items = new List<ListViewItem>();

    public ListViewItem selectedItem = null;


    /// <summary>
    /// 统一处理选中状态逻辑，包括1.取消重复点击项的选择状态2.选中新项的同时取消旧项选择
    /// </summary>
    public ListViewItem SelectedItem {
        get { return selectedItem; }
        private set {
            // 【修改点2】重构选中逻辑
            if (selectedItem == value) {
                // 点击已选中的项：取消选中
                if (selectedItem != null) {
                    selectedItem.Selected = false;
                }
                selectedItem = null;
                onItemSelected?.Invoke(null);
            } else {
                // 点击新项：取消旧项，选中新项
                if (selectedItem != null) {//取消旧项的选择状态
                    selectedItem.Selected = false;//取消高亮
                }
                selectedItem = value;//选中新项
                if (selectedItem != null) {
                    selectedItem.Selected = true;//新项高亮
                }
                onItemSelected?.Invoke(selectedItem);
            }
        }
    }

    public void AddItem(ListViewItem item) {
        item.owner = this;//区分主线和支线列表
        this.items.Add(item);
    }

    public void RemoveAll() {
        foreach (var it in items) {
            Destroy(it.gameObject);
        }
        items.Clear();
    }
}
