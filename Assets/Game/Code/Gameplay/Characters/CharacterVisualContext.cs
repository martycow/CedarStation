using System;

[Serializable]
public class CharacterVisualContext
{
    public bool HoodieOn { get; private set; }
    
    public CharacterVisualContext(bool hoodieOn)
    {
        HoodieOn = hoodieOn;
    }
    
    public void TurnOnHoodie()
    {
        HoodieOn = true;
    }

    public void TurnOffHoodie()
    {
        HoodieOn = false;
    }
}