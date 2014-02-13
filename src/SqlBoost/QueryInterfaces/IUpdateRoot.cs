﻿using System;
using System.Linq.Expressions;

namespace SqlBoost.QueryInterfaces
{
	public interface IUpdateRoot
	{
		IUpdate<T> Update<T>(Expression<Func<T>> set);
	}
}
