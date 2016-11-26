namespace Heleonix.Build
{
    /// <summary>
    /// Represents the executable result.
    /// </summary>
    public class ExeResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets the exit code.
        /// </summary>
        public int ExitCode { get; set; }

        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        public string Error { get; set; }

        #endregion

        #region object Members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => "ExitCode: " + ExitCode;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) => (obj as ExeResult)?.ExitCode == ExitCode;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => ExitCode;

        #endregion
    }
}