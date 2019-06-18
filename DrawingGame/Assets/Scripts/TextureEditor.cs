using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextureEditor : MonoBehaviour {

	public Image image;
	public Camera mainCamera;
	public int radius;
	public float precision;

	private Texture2D editedTexture;
	private Vector2 lastPosition;

	private void Start() {
		lastPosition = new Vector2(-1, -1);
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		editedTexture = new Texture2D(Mathf.FloorToInt((image.rectTransform.anchorMax.x - image.rectTransform.anchorMin.x) * Screen.width), 
									  Mathf.FloorToInt((image.rectTransform.anchorMax.y - image.rectTransform.anchorMin.y) * Screen.height),
									  TextureFormat.RGBA32, false);
		Color[] textureColor = new Color [editedTexture.width * editedTexture.height];
		for (int i = 0; i < textureColor.Length; i++) {
			textureColor[i] = Color.clear;
		}
		editedTexture.SetPixels(textureColor);
		image.sprite = Sprite.Create(editedTexture, new Rect(0, 0, editedTexture.width, editedTexture.height), new Vector2(0.5f, 0.5f));
	}

	private void Update() {
		if (Input.GetMouseButton(0)) {
			Vector3 viewPortMousePosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
			if (viewPortMousePosition.x > image.rectTransform.anchorMin.x && viewPortMousePosition.x < image.rectTransform.anchorMax.x && 
				viewPortMousePosition.y > image.rectTransform.anchorMin.y && viewPortMousePosition.y < image.rectTransform.anchorMax.y) {
				int x = Mathf.FloorToInt(((viewPortMousePosition.x - image.rectTransform.anchorMin.x) / (image.rectTransform.anchorMax.x - image.rectTransform.anchorMin.x)) * editedTexture.width);
				int y = Mathf.FloorToInt(((viewPortMousePosition.y - image.rectTransform.anchorMin.y) / (image.rectTransform.anchorMax.y - image.rectTransform.anchorMin.y)) * editedTexture.height);
				Vector2 drawPosition = new Vector2(x, y);
				if (lastPosition.x > -1 && lastPosition.y > -1) {
					float t = 0;
					while (t <= 1) {
						Draw(Vector2.Lerp(lastPosition, drawPosition, t));
						t += precision;
					}
				}
				else {
					Draw(drawPosition);
				}
				lastPosition = new Vector2(x, y);
				editedTexture.Apply();
				image.sprite = Sprite.Create(editedTexture, new Rect(0, 0, editedTexture.width, editedTexture.height), new Vector2(0.5f, 0.5f));
			}
		}
		if (Input.GetMouseButtonUp(0)) {
			lastPosition = new Vector2Int(-1, -1);
		}
		if (Input.GetKeyDown(KeyCode.S)) {
			Save("Test");
		}
		if (Input.GetKeyDown(KeyCode.N)) {
			SceneManager.LoadScene("PlayingScene");
		}
	}

	private void Draw(Vector2 position) {
		for (float i = position.x - radius; i <= position.x + radius; i++) {
			i = i < 0 ? 0 : i > editedTexture.width ? editedTexture.width : i;
			for (float j = position.y - radius; j < position.y + radius; j++) {
				j = j < 0 ? 0 : j > editedTexture.height ? editedTexture.height : j;
				editedTexture.SetPixel(Mathf.FloorToInt(i), Mathf.FloorToInt(j), Color.black);
			}
		}
	}

	public bool Save(string name) {
		if (!Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites")) {
			Directory.CreateDirectory(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites");
		}
		File.WriteAllBytes(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites" + Path.DirectorySeparatorChar + name + ".png", Trim(editedTexture).EncodeToPNG());
		return true;
	}

	private Texture2D Trim(Texture2D texture) {
		int minX = texture.width, minY = texture.height, maxX = 0, maxY = 0;
		for (int i = 0; i < texture.width; i++) {
			for (int j = 0; j < texture.height; j++) {
				if (texture.GetPixel(i, j) != Color.clear) {
					minX = i < minX ? i : minX;
					minY = j < minY ? j : minY;
					maxX = i > maxX ? i : maxX;
					maxY = j > maxY ? j : maxY;
				}
			}
		}
		Texture2D newTexture = new Texture2D(maxX-minX, maxY-minY, TextureFormat.ARGB32, false);
		for (int i = 0; i < maxX-minX; i++) {
			for (int j = 0; j < maxY-minY; j++) {
				newTexture.SetPixel(i, j, texture.GetPixel(i + minX, j + minY));
			}
		}
		return newTexture;
	}
}
