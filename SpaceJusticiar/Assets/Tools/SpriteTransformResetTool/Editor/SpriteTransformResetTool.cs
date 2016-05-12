// SpriteTransformResetTool by UnityCoder.com
using UnityEditor;
using UnityEngine;

namespace unitycodercom_spriteTransformReset
{

	public class SpriteTransformResetTool : EditorWindow 
	{

		private const string appName = "Sprite Transform Reset Tool";

		private float pixelsToReset=100;
		private bool resetCollider = true;
		
		[MenuItem ("Window/PolygonColliderOptimizer/"+appName, false, 10)]
		static void Init () {
			SpriteTransformResetTool window = (SpriteTransformResetTool)EditorWindow.GetWindow (typeof (SpriteTransformResetTool));
			window.titleContent = new GUIContent(appName);
			window.minSize = new Vector2(300,150);
		}
		
		void OnGUI () 
		{
			GUILayout.Label (appName, EditorStyles.boldLabel);
			EditorGUILayout.Space();

			if (GUILayout.Button("Reset transform for selected objects",GUILayout.Height(30)))
			{
				foreach (GameObject go in Selection.gameObjects)
				{
					// if sprite
					if (go.GetComponent<SpriteRenderer>())
					{
						
						// TODO: comparing floats?
						if (go.transform.localScale.x!=1)
						{
							
							string assetPath = AssetDatabase.GetAssetPath (go.GetComponent<SpriteRenderer>().sprite);
							TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
							// WARNING, only X is used!!
							float xRatio = 1/go.transform.localScale.x;
							
							// register undo, for all?
							Undo.RecordObject (go.transform,"Reset transform");
							Undo.RecordObject (importer,"Reset transform");
							
							// set new units
							// TODO: round to int?
							importer.spritePixelsPerUnit *= xRatio;
							
							// reset transform size to 1,1,1
							go.transform.localScale = Vector3.one;
							// reimport
							AssetDatabase.ImportAsset(assetPath);

							// reset collider
							if (resetCollider)
							{
								ResetColliders(go);
							}
							
						} // if scale != 1
					} // if spriterenderer
				} // foreach selected
			} // button pressed

			resetCollider = GUILayout.Toggle(resetCollider,"Reset colliders (to fix size)");

			
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Set selected sprites PixelsToUnits"))
			{
				// TODO: float field here to give value
				
				foreach (GameObject go in Selection.gameObjects)
				{
					
					// if sprite
					if (go.GetComponent<SpriteRenderer>())
					{
						string assetPath = AssetDatabase.GetAssetPath (go.GetComponent<SpriteRenderer>().sprite);
						TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
						// register undo, for all?
						Undo.RecordObject (importer,"Reset transform");
						// set new units
						importer.spritePixelsPerUnit = pixelsToReset;
						// reimport
						AssetDatabase.ImportAsset(assetPath);

						// reset collider
						if (resetCollider)
						{
							ResetColliders(go);
						}

					}
					
				} // foreach
				
				
			} // button pressed
			
			pixelsToReset = EditorGUILayout.FloatField(Mathf.Clamp(pixelsToReset,0.001f,999999f));
			
			GUILayout.EndHorizontal();

			EditorGUILayout.Space();


			// show selected object info
			if (Selection.gameObjects.Length>0)
			{
				
				if (Selection.gameObjects[0].GetComponent<SpriteRenderer>())
				{
					
					string assetPath = AssetDatabase.GetAssetPath (Selection.gameObjects[0].GetComponent<SpriteRenderer>().sprite);
					TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
					
					GUILayout.Label("Selected object: "+importer.spritePixelsPerUnit+" (pixels to units)");	
				}
			}else{
				GUILayout.Label("");	
			}


		} // ongui




		// reset colliders, only 1 collider, doesnt work with multiple same type colliders
		void ResetColliders(GameObject go)
		{
			// circle collider
			if (go.GetComponent<CircleCollider2D>()!=null)
			{
				bool isTrigger = go.GetComponent<CircleCollider2D>().isTrigger;
				PhysicsMaterial2D physMat = go.GetComponent<CircleCollider2D>().sharedMaterial;
				DestroyImmediate(go.GetComponent<CircleCollider2D>());
				go.AddComponent<CircleCollider2D>();
				go.GetComponent<CircleCollider2D>().isTrigger = isTrigger;
				go.GetComponent<CircleCollider2D>().sharedMaterial = physMat;
			} // if circlecollider2d
			
			// box collider
			if (go.GetComponent<BoxCollider2D>()!=null)
			{
				bool isTrigger = go.GetComponent<BoxCollider2D>().isTrigger;
				PhysicsMaterial2D physMat = go.GetComponent<BoxCollider2D>().sharedMaterial;
				DestroyImmediate(go.GetComponent<BoxCollider2D>());
				go.AddComponent<BoxCollider2D>();
				go.GetComponent<BoxCollider2D>().isTrigger = isTrigger;
				go.GetComponent<BoxCollider2D>().sharedMaterial = physMat;
			} // if BoxCollider2D
			
			
			// polygon collider
			if (go.GetComponent<PolygonCollider2D>()!=null)
			{
				bool isTrigger = go.GetComponent<PolygonCollider2D>().isTrigger;
				PhysicsMaterial2D physMat = go.GetComponent<PolygonCollider2D>().sharedMaterial;
				DestroyImmediate(go.GetComponent<PolygonCollider2D>());
				go.AddComponent<PolygonCollider2D>();
				go.GetComponent<PolygonCollider2D>().isTrigger = isTrigger;
				go.GetComponent<PolygonCollider2D>().sharedMaterial = physMat;
			} // if PolygonCollider2D


		} // ResetCollider


		// update editor window 
	    void OnInspectorUpdate() 
		{
	    	Repaint();
	    }
		
	} //class

} // namespace

