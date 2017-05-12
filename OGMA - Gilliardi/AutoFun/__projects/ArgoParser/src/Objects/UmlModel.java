package Objects;

import Diagrams.UmlDiagram;
import java.util.ArrayList;

public class UmlModel
{
    private ArrayList<UmlDiagram> diagrams;

    public String getId()
    {
        return Id;
    }

    public void setId(String Id)
    {
        this.Id = Id;
    }

    public String getName()
    {
        return Name;
    }

    public void setName(String Name)
    {
        this.Name = Name;
    }

    private String Id;

    private String Name;

    public UmlModel()
    {
        diagrams = new ArrayList<>();
        //  id = new UID().toString();
    }

    public ArrayList<UmlDiagram> getDiagrams()
    {
        return diagrams;
    }

    public void setDiagrams(ArrayList<UmlDiagram> diagrams)
    {
        this.diagrams = diagrams;
    }
    
    public void addDiagram(UmlDiagram diagram)
    {
        this.diagrams.add(diagram);
    }
}
