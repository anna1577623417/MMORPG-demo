using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GameDefine : MonoBehaviour {
    public const int BagMaxItemPerPage = 30;
    public const int GuildMaxMemberCount = 40;
    public const int MaxChatRecordNums = 20;
    public const int MaxChatRecordTime = 600;

    public static string GetDescription(Enum value) {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute?.Description ?? value.ToString();
    }

    public enum ClassType {
        [Description("无职业")]
        None = 0,

        [Description("战士")]
        Warrior = 1,

        [Description("法师")]
        Wizar = 2,

        [Description("弓箭手")]
        Archer = 3
    }
    public enum GuildPosition {
        [Description("")]
        None = 0,

        [Description("会长")]
        President = 1,

        [Description("副会长")]
        VicePresident = 2,
    }

}
