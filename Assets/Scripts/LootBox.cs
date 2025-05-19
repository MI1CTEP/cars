using UnityEngine;

namespace MyGame.Gameplay
{
    public sealed class LootBox : MonoBehaviour
    {
        [SerializeField] private LootBoxContent[] _lootBoxContents;

        public LootBoxContent[] GetLootBoxContents => _lootBoxContents;
    }

    [System.Serializable]
    public class LootBoxContent
    {
        public Item item;
        public int chance = 1;
    }
}