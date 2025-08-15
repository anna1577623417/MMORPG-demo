using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

//superLink
[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("LinkOpener");
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, null);

        if (linkIndex == -1) return;

        TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
        string linkId = linkInfo.GetLinkID();

        if (string.IsNullOrEmpty(linkId)) {
            Debug.LogWarning("链接ID为空");
            return;
        }

        //// 安全分割参数
        //string[] strings = linkId.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        //if (strings.Length < 2 || !int.TryParse(strings[0], out int targetId)) {
        //    Debug.LogError($"链接参数格式错误: {linkId}");
        //    return;
        //}


        string[] strings = linkId.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

        // 兼容 C# 6 的变量声明方式
        int targetId;
        bool parseSuccess = int.TryParse(strings[0], out targetId);

        if (strings.Length < 2 || !parseSuccess) {
            Debug.LogError($"链接参数格式错误: {linkId}");
            return;
        }

        // 安全获取UI实例
        if (UIManager.Instance == null) {
            Debug.LogError("UIManager 实例未初始化");
            return;
        }

        UIPopCharacterMenu menu = UIManager.Instance.Show<UIPopCharacterMenu>();
        if (menu == null) {
            Debug.LogError("角色菜单加载失败");
            return;
        }

        // 处理玩家名中的冒号（如有）
        string targetName = string.Join(":", strings, 1, strings.Length - 1);
        menu.targetId = targetId;
        menu.targetName = targetName;
    }
}