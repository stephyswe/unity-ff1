public class MonsterHandler
{
    public static string ProcessMonsterName(string m_name)
    {
        if (!m_name.Contains("("))
            return m_name;
        return m_name.Substring(0, m_name.IndexOf("(") - 1);
    }
}
