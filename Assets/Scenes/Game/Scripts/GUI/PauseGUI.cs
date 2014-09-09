using UnityEngine;
using System.Collections;

public class PauseGUI : MonoBehaviour {
	
	void OnResume() {
		enabled = false;
	}
	
	void OnPause() {
		enabled = true;
	}
	

	void OnGUI() {
		GUILayout.BeginArea( new Rect(0, 0, Screen.width, Screen.height) );
		GUILayout.FlexibleSpace();
			DrawResumeButton();
			DrawSunSlider();
			DrawHelpText();
			DrawQuitButton();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
	
	private void DrawResumeButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Resume", GUILayout.ExpandWidth(false))) {
			SendMessage("OnResume", SendMessageOptions.DontRequireReceiver);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void DrawSunSlider() {
		const float min = (float) LightComputer.MIN_LIGHT / LightComputer.MAX_LIGHT;
		const float max = 1;
		float light = RenderSettings.ambientLight.r;
		
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal(GUI.skin.box);
				GUILayout.Label("Sun");
				light = GUILayout.HorizontalSlider( light, min, max, GUILayout.Width(Screen.width/2f) );
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		light = Mathf.Clamp(light, min, 1f);
		RenderSettings.ambientLight = new Color(light, light, light, 1f);
	}
	
	private void DrawHelpText() {
		string text = "Esc - Pause/Resume\n" +
						"E - Open the inventory";
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Box(text, GUILayout.ExpandWidth(false));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
	private void DrawQuitButton() {
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Quit", GUILayout.ExpandWidth(false))) {
			Application.Quit();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}
	
}
