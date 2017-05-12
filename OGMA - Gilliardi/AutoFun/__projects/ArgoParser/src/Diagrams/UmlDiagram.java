package Diagrams;

import java.util.ArrayList;
import Objects.*;
import java.util.UUID;

public abstract class UmlDiagram
{
    private String Id= UUID.randomUUID().toString();;

    private String Name;

    private UmlModel ParentModel;

    private ArrayList<UmlBase> UmlObjects = new ArrayList<>();

    private ArrayList<String> UmlStereotypes = new ArrayList<>();

    public UmlElement GetElementById(String id)
    {
        return null;
    }

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

    public UmlModel getParentModel()
    {
        return ParentModel;
    }

    public void setParentModel(UmlModel ParentModel)
    {
        this.ParentModel = ParentModel;
    }

    public ArrayList<UmlBase> getUmlObjects()
    {
        return UmlObjects;
    }

    public void setUmlObjects(ArrayList<UmlBase> UmlObjects)
    {
        this.UmlObjects = UmlObjects;
    }

    public ArrayList<String> getUmlStereotypes()
    {
        return UmlStereotypes;
    }

    public void setUmlStereotypes(ArrayList<String> UmlStereotypes)
    {
        this.UmlStereotypes = UmlStereotypes;
    }
    
    public void addUmlObject(UmlBase obj)
    {
        this.UmlObjects.add(obj);
    }
}
