namespace Cyberspeed.CardMatch.Enums
{
    /// <summary>
    /// Represents the possible states of a card.
    /// </summary>
    public enum CardState
    {
        /// <summary>
        /// The card is face up, showing its symbol.
        /// </summary>
        Revealed,
        /// <summary>
        /// The card is face down.
        /// </summary>
        Hidden,
        /// <summary>
        /// The card has been matched and is no longer interactable.
        /// </summary>
        Disabled
    }
}

