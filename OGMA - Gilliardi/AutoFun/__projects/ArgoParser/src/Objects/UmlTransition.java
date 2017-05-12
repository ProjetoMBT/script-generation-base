package Objects;

public class UmlTransition extends UmlAssociation
{
    private UmlElement Source;

    private UmlElement Target;

    public UmlElement getSource()
    {
        return Source;
    }

    public void setSource(UmlElement Source)
    {
        this.Source = Source;
    }

    public UmlElement getTarget()
    {
        return Target;
    }

    public void setTarget(UmlElement Target)
    {
        this.Target = Target;
    }
}
