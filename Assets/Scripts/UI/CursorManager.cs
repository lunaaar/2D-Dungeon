using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public GameObject gamemanager;
    public List<Sprite> sprites;
    private bool loseFocuse = false;
    
    // Start is called before the first frame update
    void Start()
    {
        SetCursor(gamemanager.GetComponent<GameManager>().getSelectedCursor());
    }

    private void Update()
    {

        if (0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y)
        {
            loseFocuse = true;
        } else if (loseFocuse)
        {
            SetCursor(gamemanager.GetComponent<GameManager>().getSelectedCursor());
            loseFocuse = false;
        }
    }

    private void SetCursor(int cursorNumber)
    {
        Cursor.SetCursor(sprite2Texture(sprites[cursorNumber]), new Vector2(45, 45), CursorMode.ForceSoftware);
    }

    // From https://answers.unity.com/questions/651984/convert-sprite-image-to-texture.html
    private Texture2D sprite2Texture(Sprite currentSprite)
    {
        var croppedTexture = new Texture2D((int)currentSprite.rect.width, (int)currentSprite.rect.height);
        Debug.Log($"{currentSprite.rect.width} {currentSprite.rect.height}");
        var pixels = currentSprite.texture.GetPixels((int)currentSprite.textureRect.x,
                                                (int)currentSprite.textureRect.y,
                                                (int)currentSprite.textureRect.width,
                                                (int)currentSprite.textureRect.height);
        Debug.Log($"{pixels.Length}");
        croppedTexture.SetPixels(pixels);
        croppedTexture.Apply();
        return croppedTexture;
    }

}
