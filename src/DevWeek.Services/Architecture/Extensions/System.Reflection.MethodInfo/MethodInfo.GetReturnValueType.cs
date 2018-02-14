// Copyright (c) 2013 Jonathan Magnan (http://zzzportal.com)
// All rights reserved.
// Licensed under MIT License (MIT)
// License can be found here: https://zextensionmethods.codeplex.com/license

using System;

namespace DevWeek.Architecture.Extensions
{
	public static partial class OragonExtensions
	{
		public static System.Type GetReturnValueType(this System.Reflection.MethodInfo methodInfo)
		{
			Type outType = null;
			try
			{
				outType = methodInfo.ReturnParameter.ParameterType;
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("Error during get MessageOutputType for method {0} ", methodInfo.Name), ex);
			}
			return outType;
		}
	}
}