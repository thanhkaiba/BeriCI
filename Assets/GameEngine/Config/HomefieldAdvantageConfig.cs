using System.Collections.Generic;

public class HomefieldAdvantageConfig
{
    public List<Advantage> listAdvantage;
    public Advantage GetAdvantage(HomefieldAdvantage type)
    {
        return listAdvantage.Find(ad => ad.type == type);
    }
}
public class Advantage
{
    public HomefieldAdvantageConfig container;

    public HomefieldAdvantage type;
    public List<float> _params;
}

