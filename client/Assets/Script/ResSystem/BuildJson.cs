using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildType
{ 
    OneAB = 1,
    EveryAB = 2
}

public class BuildJson 
{
    public string FolderName;
    public ResType ResType;
    public BuildType BuildType;
}
