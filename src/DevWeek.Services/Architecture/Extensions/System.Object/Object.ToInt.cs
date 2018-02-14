// Copyright (c) 2013 Jonathan Magnan (http://zzzportal.com)
// All rights reserved.
// Licensed under MIT License (MIT)
// License can be found here: https://zextensionmethods.codeplex.com/license

using System;

namespace DevWeek.Architecture.Extensions
{
	public static partial class OragonExtensions
	{
		/// <id>9F5259DF-C634-4947-AC43-1EAB9CC7B83E</id>
		/// <summary>
		///     An object extension method that converts the @this to an int.
		/// </summary>
		/// <param name="this">The @this to act on.</param>
		/// <returns>@this as an int.</returns>
		public static int ToInt(this object @this)
		{
			return Convert.ToInt32(@this);
		}
	}
}