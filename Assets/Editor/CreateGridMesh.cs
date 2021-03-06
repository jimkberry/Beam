using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
 
 
public class CreateGridMesh : ScriptableWizard
{
 
    public enum Orientation
    {
        Horizontal,
        Vertical
    }
 
    public enum AnchorPoint
    {
        TopLeft,
        TopHalf,
        TopRight,
        RightHalf,
        BottomRight,
        BottomHalf,
        BottomLeft,
        LeftHalf,
        Center
    }
 
    public int widthSegments = 1;
    public int lengthSegments = 1;
    public float width = 1.0f;
    public float length = 1.0f;
    public Orientation orientation = Orientation.Horizontal;
    public AnchorPoint anchor = AnchorPoint.Center;
    public bool addCollider = false;
    public bool createAtOrigin = true;
    public bool twoSided = false;
    public string optionalName;
 
    static Camera cam;
    static Camera lastUsedCam;
 
 
    [MenuItem("GameObject/Create Other/Custom Grid Mesh...")]
    static void CreateWizard()
    {
        cam = Camera.current;
        // Hack because camera.current doesn't return editor camera if scene view doesn't have focus
        if (!cam)
            cam = lastUsedCam;
        else
            lastUsedCam = cam;
        ScriptableWizard.DisplayWizard("Create Grid Mesh",typeof(CreateGridMesh));
    }
 
 
    void OnWizardUpdate()
    {
        widthSegments = Mathf.Clamp(widthSegments, 1, 254);
        lengthSegments = Mathf.Clamp(lengthSegments, 1, 254);
    }
 
 
    void OnWizardCreate()
    {
        GameObject grid = new GameObject();
 
        if (!string.IsNullOrEmpty(optionalName))
            grid.name = optionalName;
        else
            grid.name = "Grid";
 
        if (!createAtOrigin && cam)
            grid.transform.position = cam.transform.position + cam.transform.forward*5.0f;
        else
            grid.transform.position = Vector3.zero;
 
		Vector2 anchorOffset;
		string anchorId;
		switch (anchor)
		{
		case AnchorPoint.TopLeft:
			anchorOffset = new Vector2(-width/2.0f,length/2.0f);
			anchorId = "TL";
			break;
		case AnchorPoint.TopHalf:
			anchorOffset = new Vector2(0.0f,length/2.0f);
			anchorId = "TH";
			break;
		case AnchorPoint.TopRight:
			anchorOffset = new Vector2(width/2.0f,length/2.0f);
			anchorId = "TR";
			break;
		case AnchorPoint.RightHalf:
			anchorOffset = new Vector2(width/2.0f,0.0f);
			anchorId = "RH";
			break;
		case AnchorPoint.BottomRight:
			anchorOffset = new Vector2(width/2.0f,-length/2.0f);
			anchorId = "BR";
			break;
		case AnchorPoint.BottomHalf:
			anchorOffset = new Vector2(0.0f,-length/2.0f);
			anchorId = "BH";
			break;
		case AnchorPoint.BottomLeft:
			anchorOffset = new Vector2(-width/2.0f,-length/2.0f);
			anchorId = "BL";
			break;			
		case AnchorPoint.LeftHalf:
			anchorOffset = new Vector2(-width/2.0f,0.0f);
			anchorId = "LH";
			break;			
		case AnchorPoint.Center:
		default:
			anchorOffset = Vector2.zero;
			anchorId = "C";
			break;
		}
 
        MeshFilter meshFilter = (MeshFilter)grid.AddComponent(typeof(MeshFilter));
        grid.AddComponent(typeof(MeshRenderer));
 
        string gridAssetName = grid.name + widthSegments + "x" + lengthSegments + "W" + width + "L" + length + (orientation == Orientation.Horizontal? "H" : "V") + anchorId + ".asset";
        Mesh m = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Editor/" + gridAssetName,typeof(Mesh));
 
        if (m == null)
        {
            m = new Mesh();
            m.name = grid.name;
 
            int hCount2 = widthSegments+1;
            int vCount2 = lengthSegments+1;
  
            int numVertices = hCount2 * vCount2;
 
            List<Vector3> vertices = new List<Vector3>();
            List<int> indices = new List<int>();
 
            float uvFactorX = 1.0f/widthSegments;
            float uvFactorY = 1.0f/lengthSegments;
            float scaleX = width/widthSegments;
            float scaleY = length/lengthSegments;
            for (float y = 0.0f; y < vCount2; y++)
            {
                for (float x = 0.0f; x < hCount2; x++)
                {
                    if (orientation == Orientation.Horizontal)
                    {
                        vertices.Add(new Vector3(x*scaleX - width/2f - anchorOffset.x, 0.0f, y*scaleY - length/2f - anchorOffset.y));
                    }
                    else
                    {
                        vertices.Add(new Vector3(x*scaleX - width/2f - anchorOffset.x, y*scaleY - length/2f - anchorOffset.y, 0.0f));
                    }
                }
            }
 
  
            for (int y = 0; y < lengthSegments; y++)
            {
                for (int x = 0; x < widthSegments; x++)
                {
                    indices.Add( (  y    * hCount2) + x);
                    indices.Add( ( (y+1) * hCount2) + x);

                    indices.Add( (  y    * hCount2) + x);
                    indices.Add( (  y    * hCount2) + x+1);                       
                }         

                indices.Add( (  y    * hCount2) + lengthSegments);                                       
                indices.Add( ( (y+1) * hCount2) + lengthSegments);                     

                indices.Add( ( widthSegments * hCount2) + y);                                       
                indices.Add( ( widthSegments * hCount2) + y+1); 

            }
 
            m.vertices = vertices.ToArray();
            m.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

            //m.RecalculateNormals();
 
            AssetDatabase.CreateAsset(m, "Assets/Editor/" + gridAssetName);
            AssetDatabase.SaveAssets();
        }
 
        meshFilter.sharedMesh = m;
        m.RecalculateBounds();
  
        if (addCollider)
            grid.AddComponent(typeof(BoxCollider));
 
        Selection.activeObject = grid;
    }
}
