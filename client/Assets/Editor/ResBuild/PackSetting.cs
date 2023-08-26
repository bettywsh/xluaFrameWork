using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PackSetting
{

	/// <summary>
	/// 打包平台
	/// </summary>
	public BuildTarget Target;

	/// <summary>
	/// 是否是热更新
	/// </summary>
	public bool IsHotfix;
}
