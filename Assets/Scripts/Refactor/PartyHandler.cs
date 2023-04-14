using System.Collections.Generic;

public class PartyHandler
{
    private List<PartyMember> party;

    public PartyHandler(List<PartyMember> party)
    {
        this.party = party;
    }

    public float GetPartyAverageLevel()
    {
        float level = 0;
        float count = 0;
        foreach (PartyMember p in party)
        {
            if (p.HP > 0)
            {
                level += (float)p.level;
                count += 1f;
            }
        }
        return (level / count);
    }
}
