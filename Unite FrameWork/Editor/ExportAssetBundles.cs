
using UnityEngine;
using UnityEditor;

//��Unity�༭������Ӳ˵�

public class ExportAssetBundles
{
	[MenuItem("Assets/Build AssetBundle From Selection")]
	static void ExportResourceRGB2()
	{
		// �򿪱�����壬����û�ѡ���·��
		string path = EditorUtility.SaveFilePanel("Save Resource", "", Selection.activeObject.name, "bundle");
		if (path.Length != 0)
		{
			// ѡ���Ҫ����Ķ���
			Object[] selection = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
			//���
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
