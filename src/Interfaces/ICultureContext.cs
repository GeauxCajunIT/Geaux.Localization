// // <copyright file="" company="GeauxCajunIT">
// // Copyright (c) GeauxCajunIT. All rights reserved.
// // </copyright>

namespace Geaux.Localization.Interfaces
{
    /// <summary>
    /// Provides access to the current culture identifier for the context.
    /// </summary>
    public interface ICultureContext
    {
        /// <summary>
        /// Gets the name of the culture currently in use for formatting and localization operations.
        /// </summary>
        string CurrentCulture { get; }
    }
}

