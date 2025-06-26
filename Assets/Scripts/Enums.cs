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

    /// <summary>
    /// Represents the different states of the game session.
    /// </summary>
    public enum GameState
    {
        Menu,
        GameInProgress,
        Paused,
        GameOver
    }

    /// <summary>
    /// Represents the different types of sound effects.
    /// </summary>
    public enum SoundType
    {
        GameStart,
        CardFlip,
        MatchSuccess,
        MatchFail,
        CardPointerEnter
    }
}

