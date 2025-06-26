using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cyberspeed.CardMatch.Cards;

namespace Cyberspeed.CardMatch.Game
{
    public class BoardBuilder
    {
       public event Action<Card> OnCardClicked;

        private readonly Card _cardPrefab;
        private readonly GridLayoutGroup _grid;
        private readonly Vector2Int _pairRange;
        private readonly int _maxRows;
        private readonly int _maxColumns;
        
        public int TotalPairs {get; private set;}

        public BoardBuilder(Card cardPrefab, GridLayoutGroup grid, Vector2Int pairRange, int maxRows, int maxColumns)
        {
            _cardPrefab = cardPrefab;
            _grid = grid;
            _pairRange = pairRange;
            _maxRows = maxRows;
            _maxColumns = maxColumns;
        }

        public void BuildBoard()
        {
            var validPairOptions = GetValidPairOptions(_pairRange.x, _pairRange.y);
            if (validPairOptions.Count == 0)
            {
                Debug.LogError("No valid board layout found within the given constraints.");
                return;
            }

            TotalPairs = validPairOptions[UnityEngine.Random.Range(0, validPairOptions.Count)];
            var totalCards = TotalPairs * 2;

            var validLayouts = GetValidLayouts(totalCards);
            var layout = validLayouts[UnityEngine.Random.Range(0, validLayouts.Count)];

            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _grid.constraintCount = layout.cols;

            var symbols = GenerateShuffledSymbols(totalCards);
            for (var i = 0; i < totalCards; i++)
            {
                var card = UnityEngine.Object.Instantiate(_cardPrefab, _grid.transform);
                card.Initialize(symbols[i]);
                card.OnCardClicked += OnCardClicked;
            }
        }
        
        private List<int> GetValidPairOptions(int minPair, int maxPair)
        {
            List<int> valid = new();
            for (var pairCount = minPair; pairCount <= maxPair; pairCount++)
            {
                var totalCards = pairCount * 2;
                var layouts = GetValidLayouts(totalCards);
                if (layouts.Count > 0)
                    valid.Add(pairCount);
            }
            return valid;
        }

        private List<(int rows, int cols)> GetValidLayouts(int totalCards)
        {
            List<(int, int)> layouts = new();
            for (var i = 1; i <= totalCards; i++)
            {
                if (totalCards % i != 0) continue;
                
                var rows = i;
                var cols = totalCards / i;
                if (rows <= _maxRows && cols <= _maxColumns)
                {
                    layouts.Add((rows, cols));
                }
            }
            
            return layouts;
        }

        private static List<int> GenerateShuffledSymbols(int count)
        {
            var symbols = new List<int>();
            for (var i = 0; i < count / 2; i++)
            {
                symbols.Add(i);
                symbols.Add(i);
            }

            for (var i = 0; i < symbols.Count; i++)
            {
                var j = UnityEngine.Random.Range(i, symbols.Count);
                (symbols[i], symbols[j]) = (symbols[j], symbols[i]);
            }

            return symbols;
        }
    }
}