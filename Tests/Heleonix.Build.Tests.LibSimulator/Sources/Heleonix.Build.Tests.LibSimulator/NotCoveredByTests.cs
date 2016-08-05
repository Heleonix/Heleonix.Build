namespace Heleonix.Build.Tests.LibSimulator
{
    /// <summary>
    /// Represents the class, which is not covered by tests.
    /// </summary>
    public class NotCoveredByTests
    {
        #region Fields

        /// <summary>
        /// The _a.
        /// </summary>
        private int _a;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NotCoveredByTests"/> class.
        /// </summary>
        public NotCoveredByTests(int a)
        {
            _a = a;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Increases the specified b.
        /// </summary>
        /// <param name="b">The b.</param>
        public void Increase(int b)
        {
            _a = _a + b;
        }

        #endregion
    }
}