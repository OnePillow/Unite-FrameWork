
using UnityEngine;
using UnityEditor;

//在Unity编辑器中添加菜单

public class ExportAssetBundles
{
	[MenuItem("Assets/Build AssetBundle From Selection")]
	static void ExportResourceRGB2()
	{
		// 打开保存面板，获得用户选择的路径
		string path = EditorUtility.SaveFilePanel("Save Resource", "", Selection.activeObject.name, "bundle");
		if (path.Length != 0)
		{
			// 选择的要保存的对象
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			//打包
#if UNITY_WEBGL
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.WebGL);
#else
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);
#endif
		}
	}

	[MenuItem("Assets/PackageWebResources")]
	static void AssetBundlePack()
    {
		string path = EditorUtility.OpenFolderPanel("Save Resource", "", Selection.activeObject.name);
		var objs = Resources.LoadAll("");
		foreach (var obj in objs)
        {
			if (obj is not GameObject) continue;
			var all = new Object[] { obj };
#if UNITY_WEBGL
			BuildPipeline.BuildAssetBundle(obj, all, path + "/" + obj.name + ".bundle", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.WebGL);
#else
			BuildPipeline.BuildAssetBundle(obj, all, path + "/" + obj.name + ".bundle", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows);
#endif
		}
    }
}
