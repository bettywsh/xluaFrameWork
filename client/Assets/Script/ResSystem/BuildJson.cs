using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildType
{ 
    OneAB = 1,
    EveryFileAB = 2,
    EveryFolderAB = 3
}

public class BuildJson 
{
    public string FolderName;
    public BuildType BuildType;
}
