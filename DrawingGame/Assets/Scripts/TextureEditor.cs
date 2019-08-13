using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TextureEditor : MonoBehaviour {

	public enum PencilShape {
		Square,
		Circle
	}

	public Image image;
	public Camera mainCamera;
	public int radius;
	public PencilShape shape;
	public Color color;
	public float precision;
	public Text debugText;
	public ScriptableObjectString path;

	//Button References
	[Header("Buttons")]
	public List<Button> colorButtons;
	public Button drawButton;
	public Button eraseButton;
	public Button clearButton;
	public Button squareButton;
	public Button circleButton;
	public Button saveButton;
	public Button loadButton;
	public Button testButton;

	//Hotkeys
	[Header("KeybaordShortcuts")]
	public KeyCode saveShortcut;
	public KeyCode loadShortcut;
	public KeyCode clearShortcut;
	public KeyCode testShortcut;

	private Color lastColor;
	private Texture2D editedTexture;
	private Vector2 lastPosition;

	//Paths
	string topFolderPath;
	string roughFolderPath;
	string trimmedFolderPath;

	//Debug
	private string mode;
	private string currentColor;
	private string xPosition;
	private string yPosition;

	#region Functions

	#region Unity Callbacks

	private void Start() {
		topFolderPath = string.Concat(Application.dataPath , Path.DirectorySeparatorChar , "GeneratedSprites");
		roughFolderPath = string.Concat(topFolderPath , Path.DirectorySeparatorChar, "Rough");
		trimmedFolderPath = string.Concat(topFolderPath , Path.DirectorySeparatorChar, "Trimmed");
		lastPosition = new Vector2(-1, -1);
		// Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
		editedTexture = new Texture2D(Mathf.FloorToInt((image.rectTransform.anchorMax.x - image.rectTransform.anchorMin.x) * Screen.width), 
									  Mathf.FloorToInt((image.rectTransform.anchorMax.y - image.rectTransform.anchorMin.y) * Screen.height),
									  TextureFormat.RGBA32, false);
		ClearTexture();
		image.sprite = Sprite.Create(editedTexture, new Rect(0, 0, editedTexture.width, editedTexture.height), new Vector2(0.5f, 0.5f));
		SetButtons();
	}

	private void Update() {
		Vector3 viewPortMousePosition = mainCamera.ScreenToViewportPoint(Input.mousePosition);
		if (viewPortMousePosition.x > image.rectTransform.anchorMin.x && viewPortMousePosition.x < image.rectTransform.anchorMax.x &&
			viewPortMousePosition.y > image.rectTransform.anchorMin.y && viewPortMousePosition.y < image.rectTransform.anchorMax.y) {
			int x = Mathf.FloorToInt(((viewPortMousePosition.x - image.rectTransform.anchorMin.x) / (image.rectTransform.anchorMax.x - image.rectTransform.anchorMin.x)) * editedTexture.width);
			int y = Mathf.FloorToInt(((viewPortMousePosition.y - image.rectTransform.anchorMin.y) / (image.rectTransform.anchorMax.y - image.rectTransform.anchorMin.y)) * editedTexture.height);
			Vector2 drawPosition = new Vector2(x, y);
			xPosition = x.ToString();
			yPosition = y.ToString();
			if (Input.GetMouseButton(0)) {
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
			}
		}
		else {
			lastPosition = new Vector2Int(-1, -1);
		}
		if (Input.GetMouseButtonUp(0)) {
			lastPosition = new Vector2Int(-1, -1);
		}
		CheckHotkeys();
		debugText.text = string.Format("Tool: {0}\nShape: {1}\nColor: #{2}\nX: {3}\nY: {4}", color == Color.clear ? "Erase" : "Draw", shape.ToString(), ColorUtility.ToHtmlStringRGB(color), xPosition, yPosition);
	}

	#endregion

	#region Texture Manipulation

	private void Draw(Vector2 position) {
		float xLimit = position.x + radius;
		xLimit = xLimit < 0 ? 0 : xLimit > editedTexture.width ? editedTexture.width : xLimit;
		float yLimit = position.y + radius;
		yLimit = yLimit < 0 ? 0 : yLimit > editedTexture.height ? editedTexture.height : yLimit;
		for (int i = Mathf.FloorToInt(position.x - radius); i <= xLimit; i++) {
			i = i < 0 ? 0 : i > editedTexture.width ? editedTexture.width : i;
			for (int j = Mathf.FloorToInt(position.y - radius); j <= yLimit; j++) {
				j = j < 0 ? 0 : j > editedTexture.height ? editedTexture.height : j;
				if ((shape == PencilShape.Circle && (Mathf.Pow(i - position.x, 2) + Mathf.Pow(j - position.y, 2)) <= Mathf.Pow(radius, 2)) || shape == PencilShape.Square) {
					if (editedTexture.GetPixel(i, j) != color) {
						editedTexture.SetPixel(i, j, color);
					}
				}
			}
		}
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
		for (int i = 0; i < maxX - minX; i++) {
			for (int j = 0; j < maxY - minY; j++) {
				newTexture.SetPixel(i, j, texture.GetPixel(i + minX, j + minY));
			}
		}
		return newTexture;
	}

	private void ClearTexture() {
		Color[] textureColor = new Color [editedTexture.width * editedTexture.height];
		for (int i = 0; i < textureColor.Length; i++) {
			textureColor[i] = Color.clear;
		}
		editedTexture.SetPixels(textureColor);
		editedTexture.Apply();
	}

	#endregion

	#region Texture Saving/Loading

	public bool SaveSprite(string name) {
		if (!Directory.Exists(topFolderPath)) {
			Directory.CreateDirectory(topFolderPath);
		}
		if (!Directory.Exists(roughFolderPath)) {
			Directory.CreateDirectory(roughFolderPath);
		}
		if (!Directory.Exists(trimmedFolderPath)) {
			Directory.CreateDirectory(trimmedFolderPath);
		}
		File.WriteAllBytes(string.Concat(roughFolderPath, Path.DirectorySeparatorChar, path.s, ".png"), editedTexture.EncodeToPNG());
		File.WriteAllBytes(string.Concat(trimmedFolderPath, Path.DirectorySeparatorChar, path.s, ".png"), Trim(editedTexture).EncodeToPNG());
		return true;
	}

	public bool LoadSprite(string name) {
		if (Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites" + Path.DirectorySeparatorChar + "Rough") && File.Exists(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites" + Path.DirectorySeparatorChar + "Rough" + Path.DirectorySeparatorChar + name + ".png")) {
			byte[] rough = File.ReadAllBytes(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites" + Path.DirectorySeparatorChar + "Rough" + Path.DirectorySeparatorChar + name + ".png");
			editedTexture.LoadImage(rough);
			editedTexture.Apply();
			return true;
		}
		return false;
	}

	#endregion

	#region Other

	private void CheckHotkeys() {
		if (Input.GetKeyDown(saveShortcut)) {
			SaveSprite(path.s);
		}
		if (Input.GetKeyDown(testShortcut)) {
			SceneManager.LoadScene("PlayingScene");
		}
		if (Input.GetKeyDown(clearShortcut)) {
			ClearTexture();
		}
		if (Input.GetKeyDown(loadShortcut)) {
			LoadSprite(path.s);
		}
	}

	private void SetButtons() {
		foreach (Button button in colorButtons) {
			button.onClick.AddListener(() => { SetPencilColor(button.transform.GetChild(0).GetComponent<Image>().color); });
		}
		drawButton.onClick.AddListener(() => { SetPencilColor(lastColor); });
		eraseButton.onClick.AddListener(() => { SetPencilColor(Color.clear); });
		clearButton.onClick.AddListener(() => { ClearTexture(); });
		squareButton.onClick.AddListener(() => { SetPencilShape(PencilShape.Square); });
		circleButton.onClick.AddListener(() => { SetPencilShape(PencilShape.Circle); });
		saveButton.onClick.AddListener(() => { SaveSprite("Test"); });
		loadButton.onClick.AddListener(() => { LoadSprite("Test"); });
		clearButton.onClick.AddListener(() => { ClearTexture(); });
		testButton.onClick.AddListener(() => { SceneManager.LoadScene("PlayingScene"); });
	}

	public void SetPencilSize(float pencilSize) {
		radius = Mathf.RoundToInt(pencilSize);
	}

	public void SetPencilColor(Color color) {
		lastColor = color == Color.clear ? lastColor : color;
		this.color = color;
	}

	public void SetPencilShape(PencilShape shape) {
		this.shape = shape;
	}

	#endregion

	#endregion
}
