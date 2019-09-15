using System.IO;
using UnityEngine;

public class ImageLoader : MonoBehaviour {
	public ScriptableObjectString path;

	private void Start() {
		byte[] png = File.ReadAllBytes(string.Concat(Application.persistentDataPath , Path.DirectorySeparatorChar , "GeneratedSprites" , Path.DirectorySeparatorChar , "Trimmed" , Path.DirectorySeparatorChar , path.s, ".png"));
		Texture2D newSprite = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		newSprite.LoadImage(png);
        UnityEngine.UI.Image spriteRenderer = GetComponent<UnityEngine.UI.Image>();
	}
}
