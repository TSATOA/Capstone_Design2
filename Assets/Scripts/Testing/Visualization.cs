using UnityEngine;
using UnityEngine.UI;

public class Visualization : MonoBehaviour
{
    private Image image;
    private Texture2D webcamTexture;
    void Start()
    {
        GameObject canvasObject = new GameObject("Canvas") { layer = 5 };
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvasObject.transform.localScale = new Vector3(0.1f,0.1f,0.1f);

        CanvasScaler canvasScaler = canvasObject.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        canvasObject.AddComponent<GraphicRaycaster>();

        var poseImage = new GameObject("Webcam Image") { layer = 5 };
        poseImage.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
        poseImage.AddComponent<CanvasRenderer>();
        RectTransform rectTransformImage = poseImage.AddComponent<RectTransform>();
        image = poseImage.AddComponent<Image>();

        poseImage.transform.SetParent(canvasObject.transform, false);
        poseImage.transform.localPosition = new Vector3(0,0,0);

        canvas.transform.position = new Vector3(0, 0, 0);
        rectTransformImage.sizeDelta = new Vector2(640, 360);

        webcamTexture = new Texture2D(640,360,TextureFormat.RGBA32,false);

        Sprite sprite = Sprite.Create(webcamTexture, new Rect(0,0,640, 360), new Vector2(0.5f,0.5f));
        image.sprite = sprite;
        image.material.shader = Shader.Find("UI/Unlit/Detail");
    }

    // Update is called once per frame
    void Update()
    {

        // Vector2[] twoDJoints = gameObject.GetComponent<PoseEstimator>().twoDJointsVector;
        // Color[] webcam = gameObject.GetComponent<CharacterControl>().webcamPixels;

        // Debug.Log(webcam[0]);

        // webcamTexture.SetPixels(webcam);
        // webcamTexture.Apply();

    }

    // void initSprites() {

    //     for (int i = 0; i < 17; i++) {

    //         GameObject spriteObject = new GameObject(String.Format("point_{0}", i)){ layer = 6 };
    //         points.Add(spriteObject);
    //         spriteObject.transform.SetParent(canvas.transform, false);
    //         SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
    //         spriteRenderer.sprite = spriteCircle;
    //         spriteRenderer.sortingOrder = 1;
    //         spriteObject.transform.localPosition = new Vector3(0,0,0);
    //         spriteObject.transform.localScale = new Vector3(10,10,10);
    //         spriteRenderer.color = new Color(
    //             Random.Range(0.1f, 0.8f),
    //             Random.Range(0.1f, 0.6f),
    //             Random.Range(0.1f, 0.7f)
    //         );

    //     }

    // }
    
    // void Draw2DPoints(float[] joints) {

    //     int arrlen = joints.Length;

    //     for (int idx = 0; idx < arrlen; idx += 3) {

    //         float x = joints[idx];
    //         float y = joints[idx+1];

    //         if (x >=0 && x < resizedSquareImageDim && y >= 0 && y < resizedSquareImageDim) {
    //             DrawPoint(idx, x, y);
    //         }

    //     }

    // }

    // void DrawPoint(int index, float x, float y) {

    //     GameObject point = points[index / 3];
    //     x = (x-(resizedSquareImageDim / 2)) * ((float)webcamTexture.width / resizedSquareImageDim);
    //     y = (y-(resizedSquareImageDim / 2)) * ((float)webcamTexture.height / resizedSquareImageDim);
    //     point.transform.localPosition = new Vector3(x, -y, 0);

    // }

}
