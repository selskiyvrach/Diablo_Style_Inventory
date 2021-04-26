using UnityEngine;


namespace MNS.Utils
{
    

    public class ParallaxBackgroundScroller : MonoBehaviour
    {
        [SerializeField] Vector2 levelMoveRatio;
        [SerializeField] bool infiniteHorizontal;
        [SerializeField] bool infiniteVertical;

        private Transform camTransform;
        private Vector2 previosCamPos;
        private float textureUnitSizeX;
        private float textureUnitSizeY;
        private static bool _inited;

        private void Awake() {
            camTransform = Camera.main.transform;
        }

        private void Start()
        {
            if(_inited == false)
            {
                _inited = true;
                transform.parent.transform.position = Camera.main.transform.position;
            }
            previosCamPos = camTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture texture = sprite.texture;
            textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
            textureUnitSizeY = texture.height / sprite.pixelsPerUnit;

        }

        private void LateUpdate()
        {
            Vector2 deltaMovement = new Vector2(camTransform.position.x - previosCamPos.x, camTransform.position.y - previosCamPos.y);
            MoveLayer(deltaMovement);
            previosCamPos = camTransform.position;

            CheckForExceedindTheLayer();
        }

        private void MoveLayer(Vector2 deltaMovement)
        {
            Vector2 deltaForLayer = new Vector2(deltaMovement.x * levelMoveRatio.x, deltaMovement.y * levelMoveRatio.y);
            transform.position = new Vector2(transform.position.x + deltaForLayer.x, transform.position.y + deltaForLayer.y);   
        }

        private void CheckForExceedindTheLayer()
        {
            if(infiniteHorizontal)
            {
                if(Mathf.Abs(camTransform.position.x - transform.position.x) >= textureUnitSizeX)
                {
                    float offsetPosX = (camTransform.position.x - transform.position.x) % textureUnitSizeX;
                    transform.position = new Vector2(camTransform.position.x + offsetPosX, transform.position.y);
                }
            }
            
            if(infiniteVertical)
            {
                if(Mathf.Abs(camTransform.position.y - transform.position.y) >= textureUnitSizeY)
                {
                    float offsetPosY = (camTransform.position.y - transform.position.y) % textureUnitSizeY;
                    transform.position = new Vector2(transform.position.x, camTransform.position.y + offsetPosY);
                }
            }
        }
    }
}
