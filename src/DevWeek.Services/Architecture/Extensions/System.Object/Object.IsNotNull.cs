// Copyright (c) 2013 Jonathan Magnan (http://zzzportal.com)
// All rights reserved.
// Licensed under MIT License (MIT)
// License can be found here: https://zextensionmethods.codeplex.com/license

namespace DevWeek.Architecture.Extensions
{
	public static partial class OragonExtensions
	{
		/// <id>624E8C82-F0EB-4699-8198-32FDE2746478</id>
		/// <summary>
		///     An object extension method that query if '@this' is not null.
		/// </summary>
		/// <param name="this">The @this to act on.</param>
		/// <returns>true if not null, false if not.</returns>
		/// <example>
		///     <code>
		///         var list = new int[] {};
		///
		///         if(list.IsNotNull())
		///         {
		///             // Code
		///         }
		///     </code>
		/// </example>
		public static bool IsNotNull(this object @this)
		{
			return @this != null;
		}
	}
}