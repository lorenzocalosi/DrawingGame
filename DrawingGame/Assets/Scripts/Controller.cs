using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {

	public float movementSpeed;

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
		if (Input.GetKey(KeyCode.B)) {
			SceneManager.LoadScene("DrawingScene");
		}
		transform.position += movement.normalized * movementSpeed;
	}

}