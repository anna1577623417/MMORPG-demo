// 添加 XML 转义工具类
using Models;
using SkillBridge.Message;
using System.Text;

public static class TmpEscapeUtility {
    public static string EscapeXml(string input) {
        if (string.IsNullOrEmpty(input)) return input;

        StringBuilder sb = new StringBuilder(input.Length);
        foreach (char c in input) {
            switch (c) {
                case '<': sb.Append("&lt;"); break;
                case '>': sb.Append("&gt;"); break;
                case '&': sb.Append("&amp;"); break;
                case '"': sb.Append("&quot;"); break;
                case '\'': sb.Append("&apos;"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }
}