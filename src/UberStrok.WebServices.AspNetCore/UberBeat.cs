using System.Collections.Generic;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;

public class UberBeat
{
    public HashSet<string> HDD = new HashSet<string>();

    public HashSet<string> BIOS = new HashSet<string>();

    public HashSet<string> MAC = new HashSet<string>();

    public HashSet<string> MOTHERBOARD = new HashSet<string>();

    public HashSet<string> UNITY = new HashSet<string>();

    public static void CopyUberBeat(UberBeat uberbeat, ref UserDocument Document)
    {
        Document.BIOS = uberbeat.BIOS;
        Document.HDD = uberbeat.HDD;
        Document.MAC = uberbeat.MAC;
        Document.MOTHERBOARD = uberbeat.MOTHERBOARD;
        Document.UNITY = uberbeat.UNITY;
    }
}
