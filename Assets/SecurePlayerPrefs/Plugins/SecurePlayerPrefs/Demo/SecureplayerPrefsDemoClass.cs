using System;

[Serializable()]//Important
public class SecureplayerPrefsDemoClass
{
    public string playID { get; set; }
    public int type { get; set; }
    public bool incremental { get; set; }

    public SecureplayerPrefsDemoClass()
    {
        this.playID = "";
        this.type = 0;
        this.incremental = false;
    }
}