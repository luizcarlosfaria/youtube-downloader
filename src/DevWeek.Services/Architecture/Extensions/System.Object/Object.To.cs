// Copyright (c) 2013 Jonathan Magnan (http://zzzportal.com)
// All rights reserved.
// Licensed under MIT License (MIT)
// License can be found here: https://zextensionmethods.codeplex.com/license

namespace DevWeek.Architecture.Extensions;

	public static partial class OragonExtensions
	{
		public static T To<T>(this object @this)
		{
			return (T)@this;
		}

		public static T SafeTo<T>(this object @this)
			where T : class
		{
			return @this as T;
		}
	}