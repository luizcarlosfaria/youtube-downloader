// Copyright (c) 2013 Jonathan Magnan (http://zzzportal.com)
// All rights reserved.
// Licensed under MIT License (MIT)
// License can be found here: https://zextensionmethods.codeplex.com/license

using System;
using System.Collections;
using System.Collections.Generic;

namespace DevWeek.Architecture.Extensions
{
	public static partial class OragonExtensions
	{
		/// <summary>
		/// Perform ForEach in enumerable Itens
		/// </summary>
		/// <typeparam name="T">Generic type parameter</typeparam>
		/// <param name="this">An IEnumerable that contains the elements to concatenate.</param>
		/// <param name="predicate">Expression to execute</param>
		/// <returns>Same Enumerable</returns>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, Action<T> predicate)
		{
			foreach (var item in @this)
				predicate(item);

			return @this;
		}

		/// <summary>
		/// Perform ForEach in enumerable Itens
		/// </summary>
		/// <param name="this">An IEnumerable that contains the elements to concatenate.</param>
		/// <param name="predicate">Expression to execute</param>
		/// <returns>Same Enumerable</returns>
		public static IEnumerable ForEach(this IEnumerable @this, Action<object> predicate)
		{
			foreach (object item in @this)
				predicate(item);

			return @this;
		}
	}
}