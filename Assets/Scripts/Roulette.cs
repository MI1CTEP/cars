using UnityEngine;
using DG.Tweening;

namespace MyGame.Gameplay
{
    public sealed class Roulette : MonoBehaviour
    {
        [SerializeField] private Transform _itemsParent;
        [SerializeField] private Transform _reloccatePoint;

        private Transform[] _items;
        private Sequence _seq;
        private readonly float _itemsSpacing = 260f;
        private int _winnerPosition;
        private int _nextItemPosition;
        private int _firstItemId;
        private bool _isPlaying;

        public void Activate(LootBox lootBox)
        {
            int[] ids = CreateIds(lootBox.GetLootBoxContents.Length);
            ChooseWinner(lootBox.GetLootBoxContents, ids);
            InstantiateItems(lootBox.GetLootBoxContents, ids);
            float endPosition = GetEndAnimPosition();
            PlayAnim(endPosition);
            _isPlaying = true;
        }

        private int[] CreateIds(int length)
        {
            int[] ids = new int[length];

            for (int i = 0; i < length; i++)
                ids[i] = i;

            for (int i = 0; i < length; i++)
            {
                int r = Random.Range(0, length);
                (ids[r], ids[i]) = (ids[i], ids[r]);
            }
            return ids;
        }

        private void ChooseWinner(LootBoxContent[] lootBoxContents, int[] ids)
        {
            int overallChance = 0;
            for (int i = 0; i < lootBoxContents.Length; i++)
                overallChance += lootBoxContents[i].chance;

            int randomChance = Random.Range(0, overallChance);
            overallChance = 0;
            for (int i = 0; i < lootBoxContents.Length; i++)
            {
                overallChance += lootBoxContents[ids[i]].chance;
                if (overallChance > randomChance)
                {
                    _winnerPosition = i;
                    return;
                }
            }
        }

        private void InstantiateItems(LootBoxContent[] lootBoxContents, int[] ids)
        {
            _items = new Transform[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                _items[i] = Instantiate(lootBoxContents[ids[i]].item, _itemsParent).transform;
                SetItemPosition(_items[i]);
            }
        }

        private void SetItemPosition(Transform item)
        {
            item.localPosition = new Vector3(_itemsSpacing * _nextItemPosition, 0, 0);
            _nextItemPosition++;
        }

        private float GetEndAnimPosition()
        {
            float endPosition = 0;
            endPosition += _itemsSpacing * _items.Length * 2;
            endPosition += _itemsSpacing * _winnerPosition;
            endPosition += Random.Range(-120f, 120f);
            return endPosition;
        }

        private void PlayAnim(float endPosition)
        {
            if (_seq != null) _seq.Kill();
            _seq = DOTween.Sequence();
            _seq.Insert(0, _itemsParent.DOLocalMoveX(-endPosition, 7f));
        }

        private void Update()
        {
            if (_isPlaying == false)
                return;

            if (_items[_firstItemId % _items.Length].position.x < _reloccatePoint.position.x)
            {
                SetItemPosition(_items[_firstItemId % _items.Length]);
                _firstItemId++;
            }
        }
    }
}