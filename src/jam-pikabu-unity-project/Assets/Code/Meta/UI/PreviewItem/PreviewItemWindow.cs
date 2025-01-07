using Code.Gameplay.Windows;
using TMPro;
using UnityEngine.UI;

namespace Code.Meta.UI.PreviewItem
{
    public class PreviewItemWindow : BaseWindow
    {
        public Image Icon;
        public TMP_Text Name;
        public TMP_Text Description;

        public void Init(in PreviewItemWindowParameters parameters)
        {
            Icon.sprite = parameters.Icon;
            Name.text = parameters.Name.GetLocalizedString();
            Description.text = parameters.Description.GetLocalizedString();
        }
    }
}