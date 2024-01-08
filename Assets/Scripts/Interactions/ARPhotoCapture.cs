using UnityEngine;
using UnityEngine.UIElements;
using System.IO;

public class ARPhotoCapture : MonoBehaviour
{
  private VisualElement uiDocument;
  public GameObject UI;

  void Start()
  {
    //UI
    uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
    var button = uiDocument.Q<Button>("PhotoButton");
    uiDocument.Q<VisualElement>("AssetImage").RemoveFromClassList("thumbnail");
    uiDocument.Q<VisualElement>("AssetImage").AddToClassList("thumbnail--big");
    button.clicked += () => SavePhoto();
  }

  void SavePhoto()
  {
    Debug.Log("YOLO SavePhoto");
    Camera camera = Camera.main;
    int width = Screen.width;
    int height = Screen.width;
    RenderTexture rt = new RenderTexture(width, height, 24);
    camera.targetTexture = rt;
    // The Render Texture in RenderTexture.active is the one
    // that will be read by ReadPixels.
    var currentRT = RenderTexture.active;
    RenderTexture.active = rt;

    // Render the camera's view.
    camera.Render();

    // Make a new texture and read the active Render Texture into it.
    Texture2D image = new Texture2D(width, height);
    image.ReadPixels(new Rect(0, 0, width, height), 0, 0);
    image.Apply();

    camera.targetTexture = null;

    // Replace the original active Render Texture.
    RenderTexture.active = currentRT;

    byte[] bytes = image.EncodeToPNG();
    string fileName = "AR_Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
    // string filePath = Path.Combine(Application.persistentDataPath, fileName);
    // File.WriteAllBytes(filePath, bytes);
    NativeGallery.SaveImageToGallery(bytes, "GaleryGuerilla", fileName, (success, path) => onImageSave(success));
    Destroy(rt);
    Destroy(image);
  }

  void onImageSave(bool success)
  {
      uiDocument.Q<Label>("SaveInfoText").text = success ? "Bild erfolgreich in Galerie gespeichert!" : "Ops! Bild konnte nicht gespeichert werden.";
      uiDocument.Q<VisualElement>("SaveInfo").RemoveFromClassList("hidden");
      Invoke("HideUIElement", 5f);
  }

  void HideUIElement()
  {
    uiDocument.Q<VisualElement>("SaveInfo").AddToClassList("hidden");
  }
}
