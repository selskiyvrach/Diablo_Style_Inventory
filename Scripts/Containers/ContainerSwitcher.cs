using UnityEngine;

namespace D2Inventory
{

    public class ContainerSwitcher : MonoBehaviour
    {
        [SerializeField] ContainerBase[] firstOption;

        [SerializeField] ContainerBase[] secondOption;

        private bool _firstOptionIsActive = true;

        public void GetChange(out ContainerBase[] active, out ContainerBase[] inactive)
        {
            active = _firstOptionIsActive ? firstOption : secondOption;
            inactive = _firstOptionIsActive ? secondOption : firstOption;
        }

        public void SetSwitchState(bool value)
        {
            if(value == _firstOptionIsActive) return;
            
            _firstOptionIsActive = !_firstOptionIsActive;
            SetActiveOptionActive();
        }

        public void SetActiveOptionActive()
        {
            foreach (var item in firstOption)
                item.SetActive(_firstOptionIsActive);

            foreach (var item in secondOption)
                item.SetActive(!_firstOptionIsActive);
        }
    }
}