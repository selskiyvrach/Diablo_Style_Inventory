using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MNS.Utils
{

    public class Fader : MonoBehaviour
    {
        [Header("Fill either of two or both")]
        [SerializeField] Image toFade;
        [SerializeField] CanvasGroup toFade2;
        [SerializeField] float fadeDelay = .2f;
        [SerializeField] float fadeDuration = 1;
        [SerializeField] bool startOnAwake = false;

        private void Start() {
            if(startOnAwake)
                StartFade();
        }

        public void StartFade()
            => StartCoroutine(Fade());

        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(fadeDelay);

            float elapsed = 0;

            while(elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;

                float alpha = 1 - elapsed / fadeDuration;;

                if(toFade != null)
                {
                    Color c = toFade.color;
                    c.a = alpha;
                    toFade.color = c;
                }
                if(toFade2 != null)
                    toFade2.alpha = alpha;

                yield return null;
            }
            gameObject.SetActive(false);
        }


    }

}
