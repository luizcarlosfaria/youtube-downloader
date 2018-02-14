// Copyright (c) 2013 Jonathan Magnan (http://zzzportal.com)
// All rights reserved.
// Licensed under MIT License (MIT)
// License can be found here: https://zextensionmethods.codeplex.com/license

using System;
using System.Linq;

namespace DevWeek.Architecture.Extensions
{
	public static partial class OragonExtensions
	{
		public static System.Type GetUniqueAndExpectedInputParameterType(this System.Reflection.MethodInfo methodInfo)
		{
			Type inType = null;
			try
			{
				inType = methodInfo.GetParameters().Single().ParameterType;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("Error during get MessageInputParameterType for method {0} ", methodInfo.Name), ex);
			}
			return inType;
		}
	}
}