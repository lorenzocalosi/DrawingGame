using System.IO;
using UnityEngine;

public class SpriteLoader : MonoBehaviour {
	public ScriptableObjectString path;

	private void Start() {
		byte[] png = File.ReadAllBytes(string.Concat(Application.dataPath , Path.DirectorySeparatorChar , "GeneratedSprites" , Path.DirectorySeparatorChar , "Trimmed" , Path.DirectorySeparatorChar , path.s, ".png"));
		Texture2D newSprite = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		newSprite.LoadImage(png);
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Sprite.Create(newSprite, new Rect(0, 0, newSprite.width, newSprite.height), Vector2.one / 2);
	}
}
