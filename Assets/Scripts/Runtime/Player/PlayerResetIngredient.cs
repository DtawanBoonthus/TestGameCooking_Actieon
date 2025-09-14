using Cooking.Utilities;
using UnityEngine;
using VContainer;

namespace Cooking
{
    public class PlayerResetIngredient : MonoBehaviour
    {
        [SerializeField] private GenericButton resetButton = null!;

        [Inject] private readonly IPlayerData playerData = null!;

        private void Start()
        {
            resetButton.SetClickCallback(playerData.ResetIngredient);
        }

        private void OnDestroy()
        {
            resetButton.ClearClickCallback();
        }
    }
}