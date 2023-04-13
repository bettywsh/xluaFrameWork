using System;
using System.Collections.Generic;

public partial class ChineseTextConfig
{
    private Dictionary<string, ChineseTextConfigItem> dict = new Dictionary<string, ChineseTextConfigItem>();
    public List<ChineseTextConfigItem> list = new List<ChineseTextConfigItem>();
        
    public void Init(List<ChineseTextConfigItem> items)
    {
		list = items;
        dict = new Dictionary<string, ChineseTextConfigItem>();
        foreach (ChineseTextConfigItem item in list)
        {
            dict.Add(item.Id, item);
        }
    }
    
    public ChineseTextConfigItem GetById(string id)
    {
		ChineseTextConfigItem item = null;	
        dict.TryGetValue(id, out item);
        if (item == null)
        {
            throw new Exception($"Not Find Config,ConfigName:{nameof (ChineseTextConfig)},ConfigId:{id}");
        }
        return item;
    }
	
	public List<ChineseTextConfigItem> GetAll()
	{
		return list;
	}
}

public partial class ChineseTextConfigItem
{
	public string Id;
	public string Value;

}