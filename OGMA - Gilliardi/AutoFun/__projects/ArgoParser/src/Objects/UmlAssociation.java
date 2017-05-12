package Objects;

public class UmlAssociation extends UmlBase
{
    private UmlElement End1;

    private UmlElement End2;

    public UmlElement getEnd1()
    {
        return End1;
    }

    public void setEnd1(UmlElement End1)
    {
        this.End1 = End1;
    }

    public UmlElement getEnd2()
    {
        return End2;
    }

    public void setEnd2(UmlElement End2)
    {
        this.End2 = End2;
    }
    
}
