package Objects;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.UUID;

public abstract class UmlBase
{
    private String Comments;

    private String Id =UUID.randomUUID().toString();

    private String Name;

    private ArrayList<String> Stereotypes;

    private HashMap<String, String> TaggedValues = new HashMap<String, String>();

    public String getComments()
    {
        return Comments;
    }

    public void setComments(String Comments)
    {
        this.Comments = Comments;
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

    public ArrayList<String> getStereotypes()
    {
        return Stereotypes;
    }

    public void setStereotypes(ArrayList<String> Stereotypes)
    {
        this.Stereotypes = Stereotypes;
    }

    public HashMap<String, String> getTaggedValues()
    {
        return TaggedValues;
    }

    public boolean addTag(String key, String value)
    {
        try
        {
            TaggedValues.put(key, value);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;
    }
}
