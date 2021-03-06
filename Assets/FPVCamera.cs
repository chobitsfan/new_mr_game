using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPVCamera : MonoBehaviour
{
    public GameObject emery;    

    WebCamTexture webcamTexture;
    bool webcam_ok = false;
    static Material lineMaterial;    
    Camera mainCamera;
    BoxCollider emeryCollider;
    Rect emeryRect;
    GUIStyle textStyle;
    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] webCams = WebCamTexture.devices;
        if (webCams.Length > 0)
        {
            webcamTexture = new WebCamTexture();
            webcamTexture.Play();
            webcam_ok = true;
        }
        mainCamera = GetComponent<Camera>();
        emeryCollider = emery.GetComponent<BoxCollider>();
        textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.red;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(emeryRect.x, Screen.height - emeryRect.y + 2, 60, 30), "TARGET", textStyle);
    }

    private void OnPreRender()
    {
        if (webcam_ok)
        {
            Graphics.Blit(webcamTexture, (RenderTexture)null);
        }
    }

    private void OnPostRender()
    {
        emeryRect = GetScreenRectFromBounds(emeryCollider);

        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINE_STRIP);
        GL.Color(Color.red);
        GL.Vertex3(emeryRect.xMin, emeryRect.yMin, 0);
        GL.Vertex3(emeryRect.xMax, emeryRect.yMin, 0);
        GL.Vertex3(emeryRect.xMax, emeryRect.yMax, 0);
        GL.Vertex3(emeryRect.xMin, emeryRect.yMax, 0);
        GL.Vertex3(emeryRect.xMin, emeryRect.yMin, 0);
        GL.End();        

        GL.PopMatrix();
    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
#if true
    private Rect GetScreenRectFromBounds(BoxCollider boxCollider)
    {
        Vector3[] screenBoundsExtents = new Vector3[8];

        var trans = boxCollider.transform;
        var min = boxCollider.center - boxCollider.size * 0.5f;
        var max = boxCollider.center + boxCollider.size * 0.5f;

        // There are 8 corners of a rectangle bounding box. Get the screenspace coordinate of each corner. We are using the
        // array member variable to save allocations when you create a new array each frame.
        screenBoundsExtents[0] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(min.x, min.y, min.z)));
        screenBoundsExtents[1] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(min.x, min.y, max.z)));
        screenBoundsExtents[2] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(min.x, max.y, min.z)));
        screenBoundsExtents[3] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(max.x, min.y, min.z)));
        screenBoundsExtents[4] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(min.x, max.y, max.z)));
        screenBoundsExtents[5] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(max.x, min.y, max.z)));
        screenBoundsExtents[6] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(max.x, max.y, min.z)));
        screenBoundsExtents[7] = mainCamera.WorldToScreenPoint(trans.TransformPoint(new Vector3(max.x, max.y, max.z)));

        // Set these variables for a safe margin distance around the unscaled canvas which we can set things to. These can be located outside of this method, but are shown here for clarity. If left here, it can handle dynamically changing the resolution.
        int margin = 20;                                // Indicates the size of the margin outside of the canvas on all sides we are willing to let the indicators stretch to.
        int minimum = -margin;                          // Beyond the left and bottom of the screen.
        int maximumWidth = Screen.width + margin;       // Beyond the right side of the screen.
        int maximumHeight = Screen.height + margin;     // Beyond the top of the screen.


        // The following section is used instead of Vector2.Min and Max, as they cause drawcalls.

        // These variables will hold the screenspace bounds of the object. We're initializing to the margin to the left and bottom of the canvas.
        float xMin = minimum;
        float xMax = minimum;
        float yMin = minimum;
        float yMax = minimum;

        // Now that we've done that, we find the first screenpoint of one of the bounds that is in front of the camera plane, and setting that as the first to be compared against.
        for (int i = 0; i < screenBoundsExtents.Length; i++)
        {
            if (screenBoundsExtents[i].z > 0)
            {
                xMin = screenBoundsExtents[i].x;
                xMax = screenBoundsExtents[i].x;
                yMin = screenBoundsExtents[i].y;
                yMax = screenBoundsExtents[i].y;
                // Break out of the loop as we don't need to loop further.
                break;
            }
        }

        // To save repeated calculations.
        float widthMiddle = Screen.width * 0.5f;
        float heightMiddle = Screen.height * 0.5f;

        // Now we go through each element of the array again to do various things.
        for (int i = 0; i < screenBoundsExtents.Length; i++)
        {
            // If this particular point is behind the camera.
            if (screenBoundsExtents[i].z <= 0)
            {
                // The following comparisons are the heart of this solution. Due to the way Camera.WorldToScreenPoint works,
                // any point behind the camera gets flipped to the opposite sides.
                // Therefore, if the point is to the left of the middle of the screen width, we put it on the right outside of the canvas.
                // If the height is less than the middle of the screen height, then we put it above the canvas, etc.
                // This allows us to have the indicator appear in the right places as it should when we're very close to or passing through an object.

                // Width checks of the point.
                if (screenBoundsExtents[i].x <= widthMiddle)
                    screenBoundsExtents[i].x = maximumWidth;
                else if (screenBoundsExtents[i].x > widthMiddle)
                    screenBoundsExtents[i].x = minimum;
                // Height checks of the point.
                if (screenBoundsExtents[i].y <= heightMiddle)
                    screenBoundsExtents[i].y = maximumHeight;
                else if (screenBoundsExtents[i].y > heightMiddle)
                    screenBoundsExtents[i].y = minimum;
            }

            // Every point will be checked now that they're put in the appropriate place in screen space, even if they're behind the camera.
            // Find the values which comprise the extents of the bounds in screen space by saving it to the previously declared variables.
            if (screenBoundsExtents[i].x < xMin)
                xMin = screenBoundsExtents[i].x;

            else if (screenBoundsExtents[i].x > xMax)
                xMax = screenBoundsExtents[i].x;

            if (screenBoundsExtents[i].y < yMin)
                yMin = screenBoundsExtents[i].y;

            else if (screenBoundsExtents[i].y > yMax)
                yMax = screenBoundsExtents[i].y;
        }

        // Clamp all the values now, so we don't crash the editor with large screenspace coordinates.
        // This is done here rather than an if (screenBoundsExtents[i].z > 0 block because if all points are in front of the camera,
        // there is a possibility of still crashing the editor.
        xMin = Mathf.Clamp(xMin, minimum, maximumWidth);
        xMax = Mathf.Clamp(xMax, minimum, maximumWidth);
        yMin = Mathf.Clamp(yMin, minimum, maximumHeight);
        yMax = Mathf.Clamp(yMax, minimum, maximumHeight);

        // Now we have the lowest left point and the upper right point of the rect. Calculate the
        // width and height of the rect by subtracting the max minus the min values and return it as a rect.
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
#endif
}
