using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Controller : MonoBehaviour {

	public float movementSpeed;

	private void Start() {
		byte[] png = File.ReadAllBytes(Application.dataPath + Path.DirectorySeparatorChar + "GeneratedSprites" + Path.DirectorySeparatorChar + "Test.png");
		Texture2D newSprite = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		newSprite.LoadImage(png);
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = Sprite.Create(newSprite, new Rect(0,0,newSprite.width, newSprite.height), Vector2.one/2);
	}

	private void Update() {
		Vector3 movement = Vector3.zero;
		if (Input.GetKey(KeyCode.D)) {
			movement.x += 1;
		}
		if (Input.GetKey(KeyCode.A)) {
			movement.x -= 1;
		}
		if (Input.GetKey(KeyCode.S)) {
			movement.y -= 1;
		}
		if (Input.GetKey(KeyCode.W)) {
			movement.y += 1;
		}
		transform.position += movement.normalized * movementSpeed;
	}

}
