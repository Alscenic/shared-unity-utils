using UnityEngine;
using UnityEditor;

namespace CGenStudios.UnityUtils.Editor
{
	internal class MipMapBiasMenu
	{
		private static Object[] selection;

		[MenuItem("Assets/Set Mipmap Bias/Recommended (-0.5)",false,1011)]
		private static void SetRecommended()
		{
			SetBias(-0.5f);
		}

		[MenuItem("Assets/Set Mipmap Bias/Soft (0.5)",false,1011)]
		private static void SetSoft()
		{
			SetBias(0.5f);
		}

		[MenuItem("Assets/Set Mipmap Bias/Default (0.0)",false,1011)]
		private static void SetDefault()
		{
			SetBias(0.0f);
		}

		[MenuItem("Assets/Set Mipmap Bias/Sharp (-0.5)",false,1011)]
		private static void SetSharp()
		{
			SetBias(-0.5f);
		}

		[MenuItem("Assets/Set Mipmap Bias/Sharper (-1.0)",false,1011)]
		private static void SetSharper()
		{
			SetBias(-1.0f);
		}

		private static void SetBias(float bias)
		{
			foreach (Texture texture in selection)
			{
				string path = AssetDatabase.GetAssetPath(texture);
				(AssetImporter.GetAtPath(path) as TextureImporter).mipMapBias = bias;
				AssetDatabase.ImportAsset(path);
			}
		}

		[MenuItem("Assets/Set Mipmap Bias/Sharper (-1.0)",true)]
		[MenuItem("Assets/Set Mipmap Bias/Sharp (-0.5)",true)]
		[MenuItem("Assets/Set Mipmap Bias/Default (0.0)",true)]
		[MenuItem("Assets/Set Mipmap Bias/Soft (0.5)",true)]
		[MenuItem("Assets/Set Mipmap Bias/Recommended (-0.5)",true)]
		private static bool ValidateSetBias()
		{
			selection = Selection.GetFiltered(typeof(Texture2D),SelectionMode.DeepAssets);
			return (selection.Length > 0);
		}
	}
}
